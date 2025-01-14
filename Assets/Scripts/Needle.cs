using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Needle : MonoBehaviour
{
    public GameObject needlePrefab; // Reference to the needle prefab
    public float spawnInterval = 4.0f; // Interval in seconds

    void Start()
    {
        // Start the coroutine to spawn needles
        StartCoroutine(SpawnNeedle());
    }

    IEnumerator SpawnNeedle()
    {
        while (true)
        {
            // Instantiate the needle at the spawner's position and rotation
            Instantiate(needlePrefab, transform.position, transform.rotation);

            // Wait for the specified interval before spawning the next needle
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
