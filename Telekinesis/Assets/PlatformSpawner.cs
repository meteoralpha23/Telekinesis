using System.Collections;
using UnityEngine;

public class PlatformSpawner : MonoBehaviour
{
    public GameObject platformPrefab; // Assign your platform prefab
    public Transform spawnPoint; // The position where the platform starts
    public Transform destination; // The final position where the platform should rise to
    public float delayBetweenCubes = 0.2f; // Delay before the next cube rises
    public float moveSpeed = 2f; // Speed at which each cube rises

    private GameObject platformInstance; // The spawned platform

    public void StartSpawning()
    {
        StartCoroutine(SpawnPlatform());
    }

    IEnumerator SpawnPlatform()
    {
        // Instantiate the platform at the spawn point
        platformInstance = Instantiate(platformPrefab, spawnPoint.position, Quaternion.identity);

        // Move each cube one by one
        foreach (Transform cube in platformInstance.transform)
        {
            StartCoroutine(MoveCube(cube)); // Move this cube
            yield return new WaitForSeconds(delayBetweenCubes); // Wait before moving the next cube
        }
    }

    IEnumerator MoveCube(Transform cube)
    {
        Vector3 startPos = cube.position;
        Vector3 endPos = new Vector3(cube.position.x, destination.position.y, cube.position.z);

        while (Vector3.Distance(cube.position, endPos) > 0.01f)
        {
            cube.position = Vector3.Lerp(cube.position, endPos, moveSpeed * Time.deltaTime);
            yield return null; // Wait for the next frame
        }

        cube.position = endPos; // Ensure exact positioning
    }
}
