/*
Samuel Su
251103293
CS4483B
This script moves handles all the movement for the player character.
Inspired from Dani's movement tutorial:
https://www.youtube.com/watch?v=XAC8U9-dTZU
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerMovement : MonoBehaviour
{
    public Transform mainCamera;
    public Transform orientation;
    public PlayerInput playerInput;
    private Rigidbody rb;

    private float sens = 50f;
    private float sensMultiplier = 1f;
    private float xRotation;

    public float speed = 4500;
    public float maxSpeed = 20;
    public float counter = 0.175f;
    public float slidecounter = 0.2f;

    public LayerMask ground;

    private float threshold = 0.01f;
    public float maxSlopeAngle = 35f;

    private Vector3 crouch = new Vector3(1, 0.5f, 1);
    private Vector3 playerScale;
    
    private bool canJump = true;
    private float jumpCooldown = 0.25f;
    public float jumpForce = 550f;

    float x, y;
    public bool jumping, sprinting, crouching, wallRunning, swinging, grappling;
    
    public float slideForce = 400;

    private Vector3 normalVector = Vector3.up;
    private Vector3 wallNormalVector;

    private float desiredX;

    private bool cancellingGrounded;
    private float wallRotation;
    private float wallRotationVel;
    private bool cancelling;
    public float wallGravity = 1f;
    private bool canRun = true;
    public bool surfing;
    private float runRotation;
    private bool cancellingWall;
    private bool cancellingSurf;
    public bool grounded;


    void Awake() {
        rb = GetComponent<Rigidbody>();
    }
    
    void Start() {
        playerScale = transform.localScale;
    }

    private void LateUpdate(){
        WallRun();
    }
    
    private void FixedUpdate() {
        //get the inputs from the player
        x = playerInput.GetX();
        y = playerInput.GetY();
        jumping = playerInput.GetJump();
        crouching = playerInput.GetCrouch();
        Movement();
    }

    private void Update() {
        Look();
    }

    public void StartCrouch() {
        //lower the player (crouch)
        transform.localScale = crouch;
        transform.position = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
        //add some speed if the player is moving on the ground
        if (rb.velocity.magnitude > 0.5f) {
            if (grounded) {
                rb.AddForce(orientation.transform.forward * slideForce);
            }
        }
    }

    //reset the player's position (uncrouch)
    public void StopCrouch() {
        transform.localScale = playerScale;
        transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
    }

    //handles all the movement of the player
    private void Movement() {
        //add extra gravity to make sure ground and gravity is working
        rb.AddForce(Vector3.down * Time.deltaTime * 10);
        
        //Find actual velocity relative to where player is looking
        Vector2 mag = FindVelRelativeToLook();
        float xMag = mag.x, yMag = mag.y;

        //Counteract sliding and sloppy movement
        CounterMovement(x, y, mag);
        
        //If holding jump && ready to jump, then jump
        if (canJump && jumping) Jump();

        //Set max speed
        float maxSpeed = this.maxSpeed;

        //If sliding down a ramp, add force down so player stays grounded and also builds speed
        if (crouching && grounded && canJump) {
            rb.AddForce(Vector3.down * Time.deltaTime * 3000);
            return;
        }
        
        //If speed is larger than maxspeed, cancel out the input so you don't go over max speed
        if (x > 0 && xMag > maxSpeed) x = 0;
        if (x < 0 && xMag < -maxSpeed) x = 0;
        if (y > 0 && yMag > maxSpeed) y = 0;
        if (y < 0 && yMag < -maxSpeed) y = 0;

        //Some multipliers
        float multiplier = 1f, multiplierV = 1f;
        
        // Restrict the Movement in air
        if (!grounded) {
            multiplier = 0.5f;
            multiplierV = 0.5f;
        }
        // Have special restrictions on player movement when swinging
        if (swinging){
            multiplier = 0.5f;
            multiplierV = 0.8f;
        }
        // do not allow the player to move as much when grappling
        else if (grappling){
            multiplier = 0.1f;
            multiplierV = 0.1f;
        }
        // Movement while sliding
        if (grounded && crouching) multiplierV = 0f;

        //Apply forces to move player
        rb.AddForce(orientation.transform.forward * y * speed * Time.deltaTime * multiplier * multiplierV);
        rb.AddForce(orientation.transform.right * x * speed * Time.deltaTime * multiplier);
    }

    private void Jump() {
        if (grounded && canJump) {
            canJump = false;

            //Add jump forces
            rb.AddForce(Vector2.up * jumpForce * 1.5f);
            rb.AddForce(normalVector * jumpForce * 0.5f);
            
            //If jumping while falling, reset y velocity.
            Vector3 vel = rb.velocity;
            if (rb.velocity.y < 0.5f)
                rb.velocity = new Vector3(vel.x, 0, vel.z);
            else if (rb.velocity.y > 0) 
                rb.velocity = new Vector3(vel.x, vel.y / 2, vel.z);
            
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }
    
    private void ResetJump() {
        canJump = true;
    }
    
    //moves the camera around based on mouse movement
    private void Look() {
        //if the game is paused, dont allow the player to look around
        if(ControllerScript.paused == true){
            return;
        }
        
        //get how much the mouse moved
        float mouseX = Input.GetAxis("Mouse X") * sens * Time.fixedDeltaTime * sensMultiplier;
        float mouseY = Input.GetAxis("Mouse Y") * sens * Time.fixedDeltaTime * sensMultiplier;

        //Find current look rotation
        Vector3 rot = mainCamera.transform.localRotation.eulerAngles;
        desiredX = rot.y + mouseX;
        
        //Rotate, and also make sure we dont over- or under-rotate.
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        //Perform the rotations
        //Find the rotation the camera should perform when wallrunning
        FindWallRotation();
        //dampen/smoothen the rotation the camera will perform
        wallRotation = Mathf.SmoothDamp(wallRotation, runRotation, ref wallRotationVel, 0.2f);
        //rotation the camera based on the mouse movements, but also based on the position of hte wall if wallrunning
        mainCamera.transform.localRotation = Quaternion.Euler(xRotation, desiredX, wallRotation);
        //rotate the orientation gameobject to keep track of our orientation
        orientation.transform.localRotation = Quaternion.Euler(0f, desiredX, 0f);
    }

    //applies friction
    private void CounterMovement(float x, float y, Vector2 mag) {
        //if in the air, jumping, swinging or grappling, there should not be friction
        if (!grounded || jumping || swinging || grappling) return;

        //Slow down sliding
        if (crouching) {
            rb.AddForce(speed * Time.deltaTime * -rb.velocity.normalized * slidecounter);
            return;
        }

        //Counter movement
        if (Math.Abs(mag.x) > threshold && Math.Abs(x) < 0.05f || (mag.x < -threshold && x > 0) || (mag.x > threshold && x < 0)) {
            rb.AddForce(speed * orientation.transform.right * Time.deltaTime * -mag.x * counter);
        }
        if (Math.Abs(mag.y) > threshold && Math.Abs(y) < 0.05f || (mag.y < -threshold && y > 0) || (mag.y > threshold && y < 0)) {
            rb.AddForce(speed * orientation.transform.forward * Time.deltaTime * -mag.y * counter);
        }
        
        //Limit diagonal running. This will also cause a full stop if sliding fast and un-crouching
        if (Mathf.Sqrt((Mathf.Pow(rb.velocity.x, 2) + Mathf.Pow(rb.velocity.z, 2))) > maxSpeed) {
            float fallspeed = rb.velocity.y;
            Vector3 n = rb.velocity.normalized * maxSpeed;
            rb.velocity = new Vector3(n.x, fallspeed, n.z);
        }
    }

    //Finds the actual velocity based on the direction the player is looking
    public Vector2 FindVelRelativeToLook() {
        //get the angle the player us looking at
        float lookAngle = orientation.transform.eulerAngles.y;
        //get the angle the player is moving at
        float moveAngle = Mathf.Atan2(rb.velocity.x, rb.velocity.z) * Mathf.Rad2Deg;

        float u = Mathf.DeltaAngle(lookAngle, moveAngle);
        float v = 90 - u;

        //get the magnitude of their velocity based on their direction
        float magnitude = rb.velocity.magnitude;
        float yMag = magnitude * Mathf.Cos(u * Mathf.Deg2Rad);
        float xMag = magnitude * Mathf.Cos(v * Mathf.Deg2Rad);
        return new Vector2(x:xMag, y:yMag);
    }

    //checks to see if the angle of the normal vector of the object we are touching and up is less than the maxslopangle
    //if so the object we are touching is the floor
    private bool IsFloor(Vector3 v) {
        float angle = Vector3.Angle(Vector3.up, v);
        return angle < maxSlopeAngle;
    }

    //checks to see if the angle of the normal vector of the object we are touching and up is less than the 80 degrees
    //if so we check if the angle is greater than maxSlopeAngle
    //if so this means we should be sliding down the object
    private bool IsSurf(Vector3 v) {
        float angle = Vector3.Angle(Vector3.up, v);
        if (angle < 80f){
            return angle > maxSlopeAngle;
        }
        return false;
    }

    //checks if the angle of the normal vector of the wall we are touch and up is not 81 degrees
    private bool IsWall(Vector3 v){
        return Math.Abs(81f - Vector3.Angle(Vector3.up, v)) > 0.1f;
    }

    //when we stay in the collision of another object
    private void OnCollisionStay(Collision other) {
        //Make sure we are only checking for walkable layers
        int layer = other.gameObject.layer;
        if (ground != (ground | (1 << layer))) return;

        //Iterate through every collision in a physics update
        for (int i = 0; i < other.contactCount; i++) {
            Vector3 normal = other.contacts[i].normal;
            //check if its the floor
            if (IsFloor(normal)) {
                if (wallRunning){
                    wallRunning = false;
                }
                //the player is grounded
                grounded = true;
                cancellingGrounded = false;
                normalVector = normal;
                CancelInvoke(nameof(StopGrounded));
            }
            //check if the collision is with a wall with the layer "Ground"
            if (IsWall(normal) && layer == LayerMask.NameToLayer("Ground")){
                //start wall running
                StartRun(normal);
                cancellingWall = false;
                CancelInvoke(nameof(StopWall));
            }
            //check if the collision is with an object we should surf on
            if (IsSurf(normal)){
                //the play should surf
                surfing = true;
                cancellingSurf = false;
                CancelInvoke(nameof(StopSurf));
            }
        }

        //Invoke ground/wall cancel, since we can't check normals with CollisionExit
        float delay = 3f;
        if (!cancellingGrounded) {
            cancellingGrounded = true;
            Invoke(nameof(StopGrounded), Time.deltaTime * delay);
        }
        if (!cancellingWall){
            cancellingWall = true;
            Invoke(nameof(StopWall), Time.deltaTime * delay);
        }
        if (!cancellingSurf){
            cancellingSurf = true;
            Invoke(nameof(StopSurf), Time.deltaTime * delay);
        }
    }

    //finds which direction the camera should rotate in when wall running
    private void FindWallRotation(){
        //if not wallrunning, we need no rotation
        if (!wallRunning){
            runRotation = 0f;
            return;
        }
        float number = 0f;
        float current = mainCamera.transform.rotation.eulerAngles.y;
        //Find the angle of rotation between (0,0,1) and the normal vector of a wall in the up plane
        number = Vector3.SignedAngle(new Vector3(0f, 0f, 1f), wallNormalVector, Vector3.up);
        //find the shortest distance between the current angle and the signed angle of the wall
        float next = Mathf.DeltaAngle(current, number);

        //calculate when we should end the run and when we can keep wallrunning
        runRotation = -(next/90f) * 15f;
        if (!canRun){
            return;
        }
        if ((Math.Abs(runRotation) < 4f && y > 0f && Math.Abs(x) < 0.1f) || (Math.Abs(runRotation) > 22f && y < 0f && Math.Abs(x) < 0.1f)){
            if (!cancelling){
                cancelling = true;
                CancelInvoke("CancelWallRun");
                Invoke("CancelWallRun", 0.2f);
            }
        }
        else{
            cancelling = false;
            CancelInvoke("CancelWallRun");
        }
    }

    //cancel any active wallrunning
    private void CancelWallRun(){
        //place a cooldown
        Invoke("ReadyWallRun", 0.5f);
        //place a force to simulate jumping
        rb.AddForce(wallNormalVector*5f, ForceMode.Impulse);
        canRun = false;
    }

    //allow wallrunning again
    private void ReadyWallRun(){
        canRun = true;
    }
    
    //for all active wallruns
    private void WallRun(){
        //if wallrunning
        if (wallRunning){
            //push the player into the wall
            rb.AddForce(-wallNormalVector * Time.deltaTime * speed);
            //apply a force of gravity
            rb.AddForce(Vector3.up * Time.deltaTime * rb.mass * 100f * wallGravity);
            //if the player jumps
            if (jumping){
                //cancel wall running
                CancelWallRun();
            }
        }
    }

    //start wallrunning
    private void StartRun(Vector3 wallNormal){
        //if the player is in the air and can run
        if (!grounded && canRun){
            wallNormalVector = wallNormal;
            float wallForce = 20f;
            //if not currently wallrunning
            if (!wallRunning){
                //keep the players velocity is the x and z direction the same
                rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
                //add a upwards force to simulate running up the wall (results in an arc)
                rb.AddForce(Vector3.up * wallForce, ForceMode.Impulse);
            }
            wallRunning = true;
        }
    }

    //set grounded to false
    private void StopGrounded() {
        grounded = false;
    }

    //set wallrunning to false
    private void StopWall(){
        wallRunning = false;
    }

    //set surfing to false
    private void StopSurf(){
        surfing = false;
    }
}
