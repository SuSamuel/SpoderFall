using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GrenadeThrower : MonoBehaviour
{
    public GameObject grenadePrefab; // Reference to the grenade prefab
    public float throwForce = 20f; // Force applied to the grenade when thrown
    public float throwCooldown = 2f; // Time interval between each throw
    public int maxGrenades = 5;  // Set the maximum number of grenades

    public Camera playerCamera; // Reference to the player's camera
    public Rigidbody playerRigidbody; // Reference to the player's Rigidbody
    public TMP_Text grenadeCountText;
    public AudioClip throwSound; // Reference to the explosion audio clip
    private AudioSource audioSource;

    private float lastThrowTime; // The time when the last grenade was thrown
    private int remainingGrenades; // Track the remaining grenades
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        remainingGrenades = maxGrenades;
        UpdateGrenadeCountText();
        playerCamera = Camera.main;
    }
    void Update()
    {
        if (ControllerScript.paused == false){
            // throw grenade
            if (Input.GetKeyDown(KeyCode.G) && Time.time >= lastThrowTime + throwCooldown && remainingGrenades > 0) // Check if the cooldown has passed
            {
                ThrowGrenade();
                lastThrowTime = Time.time; // Update the last throw time
                remainingGrenades--; // Decrement the remaining grenades
                UpdateGrenadeCountText();
            }
        }
    }

    void ThrowGrenade()
    {
        audioSource.clip = throwSound;
        audioSource.Play();
        Vector3 spawnPosition = transform.position + playerCamera.transform.forward * 5f; // Adjust the multiplier to create more distance from the player
        GameObject grenadeInstance = Instantiate(grenadePrefab, spawnPosition, transform.rotation);
        Rigidbody rb = grenadeInstance.GetComponent<Rigidbody>();

        // Use the forward direction of the player's camera to determine the throw direction
        Vector3 throwDirection = playerCamera.transform.forward;
        
       // Project the player's velocity onto the throw direction to get the velocity in the direction of throwing
        Vector3 playerVelocityInThrowDirection = Vector3.Project(playerRigidbody.velocity, throwDirection);

        // Calculate the final throw force based on the player's speed in the direction of throwing
        float playerSpeedInThrowDirection = playerVelocityInThrowDirection.magnitude;
        float finalThrowForce = throwForce + playerSpeedInThrowDirection;

        rb.AddForce(throwDirection * finalThrowForce, ForceMode.VelocityChange);
    }
    void UpdateGrenadeCountText()
    {
        grenadeCountText.text = $"Grenades: {remainingGrenades}";
    }

}
