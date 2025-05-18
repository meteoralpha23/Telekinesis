using UnityEngine;

public class Stickcube : MonoBehaviour
{
    public Transform stickPosition;
    public PlatformSpawner platformSpawner;
    public CanvasTrigger canvasTrigger; // Reference to CanvasTrigger script

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("stick"))
        {
            if (stickPosition != null)
            {
                other.transform.position = stickPosition.position;
            }

            other.transform.SetParent(transform);

            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true;
            }

            if (platformSpawner != null)
            {
                platformSpawner.StartSpawning();
            }

            // Disable UI Canvas when cube sticks to the wall
            if (canvasTrigger != null)
            {
                canvasTrigger.StickToWall();
            }
        }
    }
}
