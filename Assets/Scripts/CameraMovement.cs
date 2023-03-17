/*
Samuel Su
251103293
CS4483B
This script moves the camera to follow the player character.
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform player;
    
    // Update is called once per frame
    void Update()
    {
        transform.position = player.transform.position;
    }
}
