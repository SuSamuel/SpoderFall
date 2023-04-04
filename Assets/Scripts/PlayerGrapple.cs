/*
Samuel Su
251103293
CS4483B
This script allows the player to use the grappling hook that slingshots the character to the object touched
Derived from https://www.youtube.com/watch?v=TYzZsBl3OI0
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGrapple : MonoBehaviour
{

    public Transform player;
    private PlayerSwing playerSwing;
    private LineRenderer lr;
    public PlayerMovement playerMovement;
    public Transform mainCamera;
    public Transform gunTip;
    public LayerMask ground;
    AudioSource grappleSound;

    public float maxDistance;
    public float grappleDelay;

    private Vector3 endPoint;
    public float cooldown;
    private float timer;
    
    private bool grappling;
    public float overshoot;

    private Vector3 currentPosition;

    // Start is called before the first frame update
    void Start()
    {
        playerSwing = GetComponent<PlayerSwing>();
        lr = GetComponent<LineRenderer>();
        grappleSound = GetComponent<AudioSource>();
    }

    //update the visuals on the line renderer after all the updates
    private void LateUpdate(){
        //if grappling
        if (grappling){
            //use lerp to simulate a travel time
            currentPosition = Vector3.Lerp(currentPosition, endPoint, Time.deltaTime * 8f);
            //draw the line
            lr.positionCount = 2;
            lr.SetPosition(1, currentPosition);
            lr.SetPosition(0, gunTip.position);
        }
    }

    // Update is called once per frame
    void Update()
    {   
        // if the user pressed the right mouse button
        if (Input.GetKeyDown(KeyCode.E) && ControllerScript.paused == false){
            //grapple
            StartGrapple();
        }
        //if the cooldown timer is greater than one, decrement the timer
        if (timer > 0){
            timer -= Time.deltaTime;
        }
    }

    //starting the grapple
    public void StartGrapple(){
        //if its on cooldown do nothing
        if (timer > 0 ){
            return;
        }
        // play sound effect
        grappleSound.Play();
        //set grapple to true
        grappling = true;
        //stop swinging
        playerSwing.StopGrapple();

        //create a raycast to check if the player hit anything
        RaycastHit target;
        //if anything with the ground layer is infront of the camera and within the max distance
        if (Physics.Raycast(mainCamera.position, mainCamera.forward, out target, maxDistance, ground)){
            //if so, we hit something grappleable and the endpoint is the coordinates of the target we hit
            endPoint = target.point;
            currentPosition = gunTip.position;
            //we can continue grappling
            Invoke(nameof(Grapple), grappleDelay);
        }
        //if not we just stop grappling
        else{
            endPoint = mainCamera.position + mainCamera.forward * maxDistance;
            Invoke(nameof(EndGrapple), grappleDelay);
        }
    }

    //the act of grappling
    public void Grapple(){
        //create a variable holding the bottom of the player character's coordinates
        Vector3 lowest = new Vector3(player.position.x, player.position.y - 1f, player.position.z);
        //fidn the difference in y values between the player and the target location
        float differenceY = endPoint.y - lowest.y;
        //store the highest value the player will be shot up
        //the overshoot value is how high the grapple will pull the character at teh start
        float highest = differenceY + overshoot;
        //if the endpoint is below the character
        if (differenceY < 0){
            //the highest the player will travel is just the overshoot value
            highest = overshoot;
        }

        //call the jump method with the highest point we calculated
        JumpTo(highest);

        //if we are still grappling after 1 second, just end the grapple
        Invoke(nameof(EndGrapple), 1f);
    }

    //ending the grapple
    public void EndGrapple(){  
        //set the grappling bools to false
        grappling = false;
        playerMovement.grappling = false;
        //set up the cooldown
        timer = cooldown;
        //remove the line in the line renderer
        lr.positionCount = 0;
    }

    Vector3 grappleVelocity;

    //make the player character jump to the target
    public void JumpTo(float height){
        //set grappling to true
        playerMovement.grappling = true;
        //calculate the velocity we need to reach teh endpoint with the given max height
        grappleVelocity = CalculateJumpVelocity(player.position, height);

        //change the velocity of the player
        Invoke(nameof(ChangeVelocity), 0.1f);
        //end grapple after 1 second
        Invoke(nameof(EndGrapple), 1f);
    }

    //change the velocity of the player to the grappling velocity
    public void ChangeVelocity(){
        player.GetComponent<Rigidbody>().velocity = grappleVelocity;
    }

    //calculate the velocity we need to teach the target
    public Vector3 CalculateJumpVelocity(Vector3 start, float height){
        //get the gravity of the game
        float gravity = Physics.gravity.y;
        //find how much we need to be displaced in the y direction
        float displacementY = endPoint.y - start.y;
        //find how much we need to be displaced in the x and z direction
        Vector3 displacementXZ = new Vector3(endPoint.x - start.x, 0f, endPoint.z - start.z);

        //calculate the velocity needed to reach the given height with gravity
        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * height);
        //calculate the velocity needed to to reach the target's corrdinates
        Vector3 velocityXZ = displacementXZ / (Mathf.Sqrt(-2 * height / gravity) + Mathf.Sqrt(2 * (displacementY - height) / gravity));

        //return the velocity needed to reach the target
        return velocityXZ + velocityY;
    }

}
