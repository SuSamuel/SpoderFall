using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : MonoBehaviour
{
    public PlayerHealth player;
    void OnCollisionEnter(Collision collision)
    
    {
        if(collision.collider.name =="Player")
        {
            player = collision.collider.GetComponent<PlayerHealth>();
            player.DecreaseHP(10);
        }
    }
}


