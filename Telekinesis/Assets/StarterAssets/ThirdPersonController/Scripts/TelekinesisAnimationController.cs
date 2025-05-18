/*
using System.Collections;
using UnityEngine;

public class TelekinesisAnimationController : MonoBehaviour
{
    private Animator animator;
    public GameObject vision;

    [SerializeField] private Transform hand;
    [SerializeField] private float modifier;
    private Vector3 pullForce;

    [SerializeField] private Transform heldObject;
    [SerializeField] private float positionDistanceThreshold;
    [SerializeField] private float velocityDistanceThreshold;
    [SerializeField] private float maxVelocity;
    [SerializeField] private float throwVelocity;
    [SerializeField] private float delay=1.0f;
    [SerializeField] private float throw_delay=2.0f;

    private bool isCasting = false;
    private bool isWalking = false;
    private Outline currentOutline;
    public KeyCode castKey = KeyCode.F;

    private bool isHighlightingEnabled = true;
    private bool isZoomedIn = false;

    private CameraController cameraController;

    void Start()
    {
        animator = GetComponent<Animator>();
        cameraController = GetComponent<CameraController>();
    }

    void Update()
    {
        HighlightObject();
        if (heldObject == null)
        {
            if (Input.GetKey(castKey))
            {
                if (!isZoomedIn)
                {
                    cameraController.ZoomIn();
                    isZoomedIn = true;
                }

                SetCastingState(true);
                StartCoroutine(Telekinesis());
            }
        }
        else if (!Input.GetKey(castKey) && isCasting)
        {
            SetCastingState(false);
        }

        if (Input.GetMouseButtonDown(1) && heldObject != null)
        {
            StartCoroutine(ThrowObject());
        }

        HandleMovementAnimation();
    }

    private void SetCastingState(bool isCasting)
    {
        animator.SetBool("IsCasting", isCasting);
        this.isCasting = isCasting;
    }

    private void HighlightObject()
    {
        if (!isHighlightingEnabled)
            return;

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
        currentOutline.OutlineColor = Color.yellow;
        currentOutline.OutlineWidth = 5f;
        currentOutline.enabled = true;
    }


    private IEnumerator Telekinesis()
    {
        isCasting = true;
        isHighlightingEnabled = false;

        if (heldObject != null)
        {
            yield break;
        }

        yield return new WaitForSeconds(delay);

        RaycastHit hit;
        if (Physics.Raycast(vision.transform.position, vision.transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity))
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
        yield return new WaitForSeconds(delay);

        while (true)
        {
            if (Input.GetMouseButton(1))
            {
                break;
            }

            float distanceToHand = Vector3.Distance(heldobject_transform.position, hand.position);

            if (distanceToHand < positionDistanceThreshold)
            {
                heldobject_transform.position = Vector3.Lerp(heldobject_transform.position, hand.position, Time.deltaTime * 10f);

                if (distanceToHand < 0.01f)
                {
                    heldobject_transform.position = hand.position;
                    heldobject_transform.parent = hand;
                    rb.constraints = RigidbodyConstraints.FreezePosition;
                    heldObject = heldobject_transform;
                    break;
                }
            }
            else
            {
                Vector3 pullDirection = hand.position - heldobject_transform.position;
                pullForce = pullDirection.normalized * modifier;

                if (rb.velocity.magnitude < maxVelocity && distanceToHand > velocityDistanceThreshold)
                {
                    rb.AddForce(pullForce, ForceMode.Force);
                }
                else
                {
                    rb.velocity = pullDirection.normalized * maxVelocity;
                }
            }

            yield return null;
        }

        isCasting = false;
        isHighlightingEnabled = true;
    }

    private IEnumerator ThrowObject()
    {
        animator.SetBool("IsThrowing", true);
        yield return new WaitForSeconds(throw_delay);

        if (heldObject != null)
        {
            heldObject.transform.parent = null;
            Rigidbody heldRb = heldObject.GetComponent<Rigidbody>();
            heldRb.constraints = RigidbodyConstraints.None;

            Vector3 throwDirection = vision.transform.forward;
            heldRb.velocity = throwDirection * throwVelocity;

            heldObject = null;
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

    private void HandleMovementAnimation()
    {
        bool isMoving = Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0;
        isWalking = isMoving || heldObject != null;
        animator.SetBool("IsWalking", isWalking);
    }
}
*/