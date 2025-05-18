using UnityEngine;

public class CanvasTrigger : MonoBehaviour
{
    public GameObject uiCanvas; // Assign your Canvas in the Inspector
    private bool isStuck = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isStuck)
        {
            uiCanvas.SetActive(true);
        }
    }

    public void StickToWall()
    {
        isStuck = true;
        uiCanvas.SetActive(false);
    }
}
