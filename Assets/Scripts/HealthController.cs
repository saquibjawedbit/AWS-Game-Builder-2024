using GoSystem;
using GoSystem.Control;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class HealthController : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnHealthChanged))]
    private float health = 100f;

    private GoRagdollController ragdollController;
    [SerializeField] private Slider Slider;
    [SerializeField] private GameObject gameOver;

    private void Start()
    {
        ragdollController = GetComponent<GoRagdollController>();
        if (isLocalPlayer)
        {
            Slider.value = health / 100f;
        }
    }

    [ClientRpc]
    public void TakeDamage(float damage, Vector3 force, Vector3 hitpoint)
    {
        health -= damage;

        if (health <= 0)
        {
            Die(force, hitpoint);
        }
    }

    private void OnHealthChanged(float oldValue, float newValue)
    {
        if (isLocalPlayer)
        {
            Slider.value = newValue / 100f;
        }
    }

    [ClientRpc]
    private void Die(Vector3 force, Vector3 hitpoint)
    {
        ragdollController.TriggerRagdoll(force, hitpoint);

       
            gameOver.SetActive(true);
            Time.timeScale = 0;
        
    }
}