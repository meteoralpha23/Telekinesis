using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class InteractableObject : MonoBehaviour
{
    [SerializeField] private float massScale = 1.0f;
    [SerializeField] private float shockwaveForce = 10f;
    [SerializeField] private float liftHeight = 0.5f;

    [Header("Shockwave Settings")]
    public SphereCollider shockwaveCollider;

    private Rigidbody rb;
    private bool liftEffectPlayed = false; // ✅ Add this flag

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.mass *= massScale;

        if (shockwaveCollider != null)
        {
            shockwaveCollider.isTrigger = true;
        }
    }

    public void ApplyReactionForce(Vector3 force)
    {
        if (rb != null)
        {
            rb.AddForce(force, ForceMode.Impulse);
        }
    }

    public void TriggerShockwaveEffect()
    {
        if (shockwaveCollider == null)
        {
            Debug.LogWarning("Shockwave Collider is not assigned!");
            return;
        }

        float shockwaveRadius = shockwaveCollider.radius * Mathf.Max(transform.lossyScale.x, transform.lossyScale.y, transform.lossyScale.z);
        Collider[] colliders = Physics.OverlapSphere(transform.position, shockwaveRadius);

        foreach (Collider nearbyObject in colliders)
        {
            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 direction = (nearbyObject.transform.position - transform.position).normalized;
                rb.AddForce(direction * shockwaveForce, ForceMode.Impulse);
            }
        }

        Debug.Log("Shockwave triggered with radius: " + shockwaveRadius);
    }

    // ✅ New Method to Play Lift Effect Only Once
    public void PlayLiftEffectOnce(Vector3 position, TelekinesisEffect effect)
    {
        if (!liftEffectPlayed)
        {
            effect.PlayLiftEffect(position);
            liftEffectPlayed = true; // Mark as played
        }
    }

    // ✅ Reset if you want to reuse the object
    public void ResetLiftEffect()
    {
        liftEffectPlayed = false;
    }
}
