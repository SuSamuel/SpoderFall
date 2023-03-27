using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class grenade : MonoBehaviour
{
    public float delay = 3f; // Time in seconds before the grenade explodes
    public float explosionForce = 700f; // Force of the explosion
    public float explosionRadius = 5f; // Radius of the explosion
    public GameObject explosionEffect; // Prefab for the explosion effect (e.g., particle system)

    private bool hasExploded = false;
    private float countdown;

    void Start()
    {
        countdown = delay;
    }

    void Update()
    {
        countdown -= Time.deltaTime;

        if (countdown <= 0f && !hasExploded)
        {
            Explode();
            hasExploded = true;
        }
    }

    private void Explode()
    {
        // Instantiate explosion effect
        if (explosionEffect != null)
        {
            GameObject effectInstance = Instantiate(explosionEffect, transform.position, transform.rotation);
            Destroy(effectInstance, 5f);
        }

        // Apply explosion force to nearby objects
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider nearbyObject in colliders)
        {
            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }
        }

        // Destroy the grenade
        Destroy(gameObject);
    }
}

