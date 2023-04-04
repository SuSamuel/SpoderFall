/*
Samuel Su
251103293
CS4483B
This script adds a rotation to the gun to simulate recoil
Taken from https://forum.unity.com/threads/simple-weapon-recoil-script.70271/
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recoil : MonoBehaviour
{
    //public GameObject Weapon;
    public float maxRecoil_x;
    public float maxRecoil_y;
 
    public float maxTrans_x;
    public float maxTrans_z;
 
    public float recoilSpeed;
    public float recoil = 0.0f;
 
    void Update()
    {
        if (recoil > 0)
        {
            var maxRecoil = Quaternion.Euler(
                Random.Range(transform.localRotation.x, maxRecoil_x),
                Random.Range(transform.localRotation.y, maxRecoil_y),
                transform.localRotation.z);
       
            // Dampen towards the target rotation
            transform.localRotation = Quaternion.Slerp(transform.localRotation, maxRecoil, Time.deltaTime * recoilSpeed);
 
            var maxTranslation = new Vector3(
                Random.Range(transform.localPosition.x, maxTrans_x),
                transform.localPosition.y,
                Random.Range(transform.localPosition.z, maxTrans_z));
 
            transform.localPosition = Vector3.Slerp(transform.localPosition, maxTranslation, Time.deltaTime * recoilSpeed);
 
            recoil -= Time.deltaTime;
        }
        else
        {
            recoil = 0;
            var minRecoil = Quaternion.Euler(
                Random.Range(0, transform.localRotation.x),
                Random.Range(0, transform.localRotation.y),
                transform.localRotation.z);
 
            // Dampen towards the target rotation
            transform.localRotation = Quaternion.Slerp(transform.localRotation, minRecoil, Time.deltaTime * recoilSpeed / 2);
 
            var minTranslation = new Vector3(
                Random.Range(0, transform.localPosition.x),
                transform.localPosition.y,
                Random.Range(0, transform.localPosition.z));
 
            transform.localPosition = Vector3.Slerp(transform.localPosition, minTranslation, Time.deltaTime * recoilSpeed);
        }
    }
}
