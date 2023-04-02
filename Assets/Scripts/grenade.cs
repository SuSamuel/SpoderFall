using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class grenade : MonoBehaviour
{
    public float delay = 3f; // Time in seconds before the grenade explodes
    public float explosionForce = 700f; // Force of the explosion
    public float explosionRadius = 5f; // Radius of the explosion
    public int damage = 50;
    public GameObject explosionEffect; // Prefab for the explosion effect (e.g., particle system)
    public AudioClip explosionSound; // Reference to the explosion audio clip
    public float audioVolume = 50f; // Volume of the audio clip

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
            Destroy(effectInstance, 4f);
        }
        
        PlaySound(explosionSound, audioVolume, transform.position);
        // Apply explosion force to nearby objects
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider nearbyObject in colliders)
        {
            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }
            PlayerHealth playerhealth = nearbyObject.GetComponent<PlayerHealth>();
            if (playerhealth != null)
            {
                playerhealth.DecreaseHP(damage);
            }
        }

        // Destroy the grenade
        Destroy(gameObject, explosionSound.length);
    }
    void PlaySound(AudioClip clip, float volume, Vector3 position)
    {
        GameObject audioObject = new GameObject("ExplosionAudio");
        audioObject.transform.position = position;
        AudioSource audioSource = audioObject.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.spatialBlend = 1f; // Ensure the sound is 3D
        audioSource.Play();

        Destroy(audioObject, clip.length);
    }
}

