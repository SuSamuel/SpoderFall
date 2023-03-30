/*
Samuel Su
251103293
CS4483B
This script takes the players inputs and stores them
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{

    public PlayerMovement playerMovement;
    float x, y;
    bool jumping, crouching;

    public GameObject pistol;

    public GameObject rifle;

    public bool onPistol;

    // Update is called once per frame
    void Update()
    {   
        //get if and which a or d key is pressed
        x = Input.GetAxisRaw("Horizontal");
        //get if and which w or s key is pressed
        y = Input.GetAxisRaw("Vertical");
        //get if the jumping key is pressed
        jumping = Input.GetButton("Jump");
        //get if the left control key is presssed
        crouching = Input.GetKey(KeyCode.LeftControl);
        //the left control key was pressed down then start crouching
        if (Input.GetKeyDown(KeyCode.LeftControl))
            playerMovement.StartCrouch();
        //if hte left control key was left go then stop crouching
        if (Input.GetKeyUp(KeyCode.LeftControl))
            playerMovement.StopCrouch();

        if (Input.GetKeyDown(KeyCode.Tab)){
            onPistol = !onPistol;
        }

        if (onPistol == false){
            pistol.SetActive(false);
            rifle.SetActive(true);
        }
        else{
            pistol.SetActive(true);
            rifle.SetActive(false);
        }

        
    }

    void Awake(){
        onPistol = false;
    }

    //gets for all the values

    public float GetX(){
        return x;
    }

    public float GetY(){
        return y;
    }

    public bool GetJump(){
        return jumping;
    }

    public bool GetCrouch(){
        return crouching;
    }
}
