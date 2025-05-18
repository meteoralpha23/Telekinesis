using System.Collections;
using UnityEngine;

public class TelekinesisController : MonoBehaviour
{
    [System.Serializable]
    public class HandInteractionSettings
    {
        public Transform hand;
        public float modifier;
        public Vector3 pullForce;
    }

    [System.Serializable]
    public class HeldObjectSettings
    {
        public Transform heldObject;
        public float positionDistanceThreshold;
        public float velocityDistanceThreshold;
        public float maxVelocity;
        public float throwVelocity;
        public float delay = 1.0f;
        public float throw_delay = 2.0f;
        public float liftOffDuration = 0.1f;
        public float liftHeight = 0.1f;
        public float torqueStrength = 10f;
        public float rotationSmoothingSpeed = 10f;
    }

    [Header("Hand & Object Interaction")]
    public HandInteractionSettings handInteraction;

    [Header("Held Object Settings")]
    public HeldObjectSettings heldObjectSettings;

    [Header("Highlighting & Vision")]
    public GameObject vision;
    public KeyCode castKey = KeyCode.F;
    private Outline currentOutline;
    private bool isHighlightingEnabled = true;

    [Header("State Management")]
    private bool isCasting = false;
    private bool isWalking = false;
    private bool isZoomedIn = false;

    [Header("Camera Settings")]
    private CameraController cameraController;

    [Header("Music")]
    public SoundManager soundManager;

    [Header("Telekinesis Effects")]
    public TelekinesisEffect telekinesisEffect; // Reference to TelekinesisEffect script

    private Animator animator;

    private void Awake()
    {
        soundManager = GameObject.FindGameObjectWithTag("Audio")?.GetComponent<SoundManager>();
        if (soundManager == null)
        {
            Debug.LogError("SoundManager not found or missing Audio tag.");
        }
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        cameraController = GetComponent<CameraController>();
    }

    void Update()
    {
        HighlightObject();
        HandleCasting();
        HandleThrowing();
        HandleMovementAnimation();
    }

    private void HandleCasting()
    {
        if (heldObjectSettings.heldObject == null && Input.GetKey(castKey))
        {
            if (heldObjectSettings.heldObject == null && Input.GetKey(castKey))
            {
                RaycastHit hit;
                if (Physics.Raycast(vision.transform.position, vision.transform.forward, out hit, Mathf.Infinity))
                {
                    if (hit.transform.GetComponent<InteractableObject>() != null)
                    {
                        soundManager.PlayTeleknesisStart();
                        if (!isZoomedIn)
                        {
                            cameraController.ZoomIn();
                            isZoomedIn = true;
                        }

                      
                        telekinesisEffect.PlayLiftEffect(hit.transform.position);

                        SetCastingState(true);
                        StartCoroutine(Telekinesis());
                    }
                }
            }
            else if (!Input.GetKey(castKey) && isCasting)
            {
                SetCastingState(false);
            }
        }
        else if (!Input.GetKey(castKey) && isCasting)
        {
            SetCastingState(false);
        }
    }

    private void SetCastingState(bool isCasting)
    {
        animator.SetBool("IsCasting", isCasting);
        this.isCasting = isCasting;
    }

    private IEnumerator Telekinesis()
    {
        isCasting = true;
        isHighlightingEnabled = false;

        if (heldObjectSettings.heldObject != null)
        {
            yield break;
        }

        yield return new WaitForSeconds(heldObjectSettings.delay);

        RaycastHit hit;
        if (Physics.Raycast(vision.transform.position, vision.transform.forward, out hit, Mathf.Infinity))
        {
            Rigidbody rb = hit.transform.GetComponent<Rigidbody>();
            if (rb != null && hit.transform.GetComponent<InteractableObject>() != null)
            {
                StartCoroutine(PullObject(hit.transform, rb));
            }
        }
    }

    private IEnumerator PullObject(Transform heldobject_transform, Rigidbody rb)
    {
        yield return new WaitForSeconds(heldObjectSettings.delay);

        Vector3 originalPosition = heldobject_transform.position;
        Vector3 liftOffPosition = originalPosition + Vector3.up * heldObjectSettings.liftHeight;
        Quaternion originalRotation = heldobject_transform.rotation;
        Quaternion targetRotation = originalRotation;
        InteractableObject interactableObject = heldobject_transform.GetComponent<InteractableObject>();
        if (interactableObject != null)
        {
            interactableObject.PlayLiftEffectOnce(originalPosition, telekinesisEffect);
            interactableObject.TriggerShockwaveEffect();
        }
        float liftOffTimer = 0f;

        while (liftOffTimer < heldObjectSettings.liftOffDuration)
        {
            heldobject_transform.position = Vector3.Lerp(originalPosition, liftOffPosition, liftOffTimer / heldObjectSettings.liftOffDuration);
            if (liftOffTimer == 0)
            {
                Vector3 torque = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * heldObjectSettings.torqueStrength;
                rb.AddTorque(torque, ForceMode.Force);
            }
            liftOffTimer += Time.deltaTime;
            yield return null;
        }
        telekinesisEffect.StopLiftEffect();
        heldobject_transform.position = liftOffPosition;

        telekinesisEffect.StartHoldEffect(heldobject_transform);

        // Play holding sound
        soundManager.PlayHoldingSound();

        while (true)
        {
            if (Input.GetMouseButton(1))
            {
                break;
            }

            float distanceToHand = Vector3.Distance(heldobject_transform.position, handInteraction.hand.position);

            if (distanceToHand < heldObjectSettings.positionDistanceThreshold)
            {
                heldobject_transform.position = Vector3.Lerp(heldobject_transform.position, handInteraction.hand.position, Time.deltaTime * 20f);

                targetRotation = handInteraction.hand.rotation;
                heldobject_transform.rotation = Quaternion.Slerp(heldobject_transform.rotation, targetRotation, Time.deltaTime * heldObjectSettings.rotationSmoothingSpeed);

                if (distanceToHand < 0.01f)
                {
                    heldobject_transform.position = handInteraction.hand.position;
                    heldobject_transform.parent = handInteraction.hand;
                    rb.constraints = RigidbodyConstraints.FreezePosition;
                    heldObjectSettings.heldObject = heldobject_transform;
                    break;
                }
            }
            else
            {
                Vector3 pullDirection = handInteraction.hand.position - heldobject_transform.position;
                handInteraction.pullForce = pullDirection.normalized * Mathf.Abs(handInteraction.modifier);

                if (rb.velocity.magnitude < heldObjectSettings.maxVelocity && distanceToHand > heldObjectSettings.velocityDistanceThreshold)
                {
                    rb.AddForce(handInteraction.pullForce, ForceMode.Force);
                }
                else
                {
                    rb.velocity = pullDirection.normalized * heldObjectSettings.maxVelocity;
                }
            }

            yield return null;
        }

        isCasting = false;
        isHighlightingEnabled = true;
    }

    private void HandleThrowing()
    {
        if (Input.GetMouseButtonDown(1) && heldObjectSettings.heldObject != null)
        {
            StartCoroutine(ThrowObject());
        }
    }

    private IEnumerator ThrowObject()
    {
        animator.SetBool("IsThrowing", true);
        yield return new WaitForSeconds(heldObjectSettings.throw_delay);

        if (heldObjectSettings.heldObject != null)
        {
            // Stop holding sound
            soundManager.StopHoldingSound();

            telekinesisEffect.PlayThrowEffect(heldObjectSettings.heldObject.transform.position);

            soundManager.PlayThrowSound();
            heldObjectSettings.heldObject.transform.parent = null;
            Rigidbody heldRb = heldObjectSettings.heldObject.GetComponent<Rigidbody>();
            heldRb.constraints = RigidbodyConstraints.None;

            Vector3 throwDirection = vision.transform.forward;
            heldRb.velocity = throwDirection * heldObjectSettings.throwVelocity;

            heldObjectSettings.heldObject = null;
            isZoomedIn = false;
            cameraController.ZoomOut();
            StartCoroutine(ResetThrowingAnimation());
        }
    }

    private IEnumerator ResetThrowingAnimation()
    {
        yield return new WaitForSeconds(0.1f);
        animator.SetBool("IsThrowing", false);
    }

    private void HighlightObject()
    {
        if (!isHighlightingEnabled || heldObjectSettings.heldObject != null)
        {
            DisableCurrentOutline();
            return;
        }

        RaycastHit hit;
        bool hitDetected = Physics.Raycast(vision.transform.position, vision.transform.forward, out hit, Mathf.Infinity);

        Outline detectedOutline = null;
        if (hitDetected)
        {
            detectedOutline = hit.transform.GetComponent<Outline>();
        }

        if (currentOutline == detectedOutline)
            return;

        DisableCurrentOutline();

        if (detectedOutline != null)
        {
            EnableOutline(detectedOutline);
        }
    }

    private void DisableCurrentOutline()
    {
        if (currentOutline != null)
        {
            currentOutline.enabled = false;
            currentOutline = null;
        }
    }

    private void EnableOutline(Outline outline)
    {
        currentOutline = outline;
        currentOutline.OutlineColor = Color.white;
        currentOutline.OutlineWidth = 3f;
        currentOutline.enabled = true;
    }

    private void HandleMovementAnimation()
    {
        isWalking = heldObjectSettings.heldObject != null;
        animator.SetBool("IsWalking", isWalking);
    }
}
