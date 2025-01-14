using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Traps : MonoBehaviour
{

    public float damage = 100.0f;
  
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            HealthController player = other.GetComponent<HealthController>();
            if (player != null)
            {
                Vector3 hitPoint = other.ClosestPoint(transform.position);
                Vector3 force = (other.transform.position - transform.position).normalized * 10.0f; // Example force value
                player.TakeDamage(damage, force, hitPoint); // Example damage value
            }
        }
    }
}
