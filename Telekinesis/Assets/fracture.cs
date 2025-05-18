using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fracture : MonoBehaviour
{


    

    public GameObject fracturedVersion; // Assign fractured wall prefab in the Inspector

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Interact"))
        {
            Instantiate(fracturedVersion, transform.position, transform.rotation);
            Destroy(gameObject); // Destroy the original object
            Destroy(other.gameObject); // Destroy the original object
        }
    }
}
