/*
Samuel Su
251103293
CS4483B
This script allows the player to use a rope to swing from a point
Inspired by https://www.youtube.com/watch?v=Xgh4v1w5DxU
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSwing : MonoBehaviour
{   

    public Rigidbody rb;
    public Transform orientation;
    public Transform player;
    public Transform gunTip;
    private PlayerGrapple playerGrapple;
    public Transform mainCamera;
    private LineRenderer lr;

    private Vector3 endPoint;
    public LayerMask ground;

    private float maxDistance = 100f;

    private SpringJoint joint;
    public float jointSpring;
    public float jointDamper;
    public float jointMass;

    public float horizontalForce;
    public float forwardForce;

    private Vector3 currentPosition;
    private bool checkSwing = false;

    //get the components
    void Awake()
    {
        playerGrapple = GetComponent<PlayerGrapple>();
        lr = GetComponent<LineRenderer>();
    }

    //draw the rope only after all the updates are already done
    void LateUpdate(){
        DrawRope();
    }
    void Update()
    {   
        //if the left mouse button is pressed down
        if (Input.GetKeyDown(KeyCode.Q)){
            //start swinging
            StartGrapple();
        }
        //if the left mouse button is released
        if (Input.GetKeyUp(KeyCode.Q)){
            //stop swinging
            StopGrapple();
        }

        //if we are swinging, allow for some extra movement to feel better
        if (checkSwing){
            Movement();
        }
    }

    //starting the swing
    private void StartGrapple(){
        //end all grappling first
        playerGrapple.EndGrapple();
        //set our swinging bool to true
        player.GetComponent<PlayerMovement>().swinging = true;
        //create a raycast to see if the player hit anything that can be swung on
        RaycastHit target;
        //see if there is anything with the ground layer infront of the player within maxdistance
        if (Physics.Raycast(mainCamera.position, mainCamera.forward, out target, maxDistance, ground)){
            //if so the endpoint is the coordinates of the target we hit
            endPoint = target.point;
            //add a spring joint component to act as a rope
            joint = player.gameObject.AddComponent<SpringJoint>();
            //do not automatically configure the anchors
            joint.autoConfigureConnectedAnchor = false;
            //the anchor should be the player and the target coordinates we hit
            joint.connectedAnchor = endPoint;
            
            //calculate the distane from the endpoint to the player
            float pointDistance = Vector3.Distance (player.position, endPoint);

            //set the max and min values for the joint
            joint.maxDistance = pointDistance * 0.8f;
            joint.minDistance = pointDistance * 0.25f;

            //set up the characteristics for the joint
            joint.spring = jointSpring;
            joint.damper = jointDamper;
            joint.massScale = jointMass;

            //set the starting position from the line as the guntip (will allow for some simple animation)
            currentPosition = gunTip.position;
            //set that we are swinging
            checkSwing = true;
        }
        //stop all swinging if we dont hit anything
        else{
            Invoke(nameof(StopGrapple), 0.1f);
        }
    }

    //stop any active swinging
    public void StopGrapple(){
        //set the bool values to false
        checkSwing = false;
        player.GetComponent<PlayerMovement>().swinging = false;
        //destroy the line from the line renderer
        lr.positionCount = 0;
        //remove the spring joint component from the player
        Destroy(joint);
    }
    
    //draw a line from the player to the target
    private void DrawRope(){
        //if we do not have a spring joint then we do not need a line
        if (!joint){
            return;
        }

        //use lerp to simulate a travel time for the line
        currentPosition = Vector3.Lerp(currentPosition, endPoint, Time.deltaTime * 8f);
        //draw the line
        lr.positionCount = 2;
        lr.SetPosition(0, gunTip.position);
        lr.SetPosition(1, currentPosition);
    }  

    //if the player tries to move while swinging
    private void Movement(){
        //add a little force in the direction the player wants
        if (Input.GetKey(KeyCode.D)){
            rb.AddForce(orientation.right * horizontalForce * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.A)){
            rb.AddForce(-orientation.right * horizontalForce * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.W)){
            rb.AddForce(orientation.forward * forwardForce * Time.deltaTime);
        }
    }
}