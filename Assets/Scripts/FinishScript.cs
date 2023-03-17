/*
Samuel Su
251103293
CS4483B
This script tells us when the game is finished
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishScript : MonoBehaviour
{
    public ControllerScript controllerScript;
    //when a collider enters the trigger hitbox
    void OnTriggerEnter(Collider other){
        //make sure it was the player
        if (other.tag == "Player"){
            //finish the game
            controllerScript.Finish();
        }
    }
}
