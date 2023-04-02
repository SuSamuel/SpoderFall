using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class healingitems : MonoBehaviour
{
    public int healAmount = 10;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Heal the player
            PlayerHealth playerhealth = other.GetComponent<PlayerHealth>();
            playerhealth.IncreaseHP(healAmount);
            
            // Destroy the healing item
            Destroy(gameObject);
        }
    }
}
