/*
Samuel Su
251103293
CS4483B
This script controls the guns and shoots bullets
Inspired by https://www.youtube.com/watch?v=bqNW08Tac0Y
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerShoot : MonoBehaviour
{
    //variables for the gun
    public int damage;
    public float timeBetweenShooting;
    public float timeBetweenShots;
    public float range;
    public float spread;
    public float reloadTime;
    public int magazine;
    public int bulletsTap;
    public bool allowHold;
    int bulletsLeft;
    int bulletsShot;
    AudioSource gunAudio;
    bool shooting;
    bool readyToShoot;
    bool reloading;

    //necessary objects
    public Transform mainCamera;
    public Transform AttackPoint;
    public RaycastHit rayHit;
    public LayerMask enemy;

    //muzzle flash effect
    public GameObject muzzleFlash;

    //text for magazine count
    public TextMeshProUGUI text;

    //object used to add recoil visually
    public Recoil RecoilObject;

    private void Update(){
        GetInput();
        text.SetText(bulletsLeft + " / " + magazine);
    }

    private void Awake(){
        bulletsLeft = magazine;
        readyToShoot = true;
        gunAudio = GetComponent<AudioSource>();
    }

    private void OnEnable(){
        readyToShoot = true;
    }

    private void OnDisable(){
        readyToShoot = false;
    }

    private void GetInput(){
        //if the game isnt paused
        if (ControllerScript.paused == false){
            //get the left mouse button
            if (allowHold){
                shooting = Input.GetMouseButton(0);
            }
            else{
                shooting = Input.GetMouseButtonDown(0);
            }  
            //reload on pressing r
            if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazine && !reloading){
                Reload();
            }
            //shoot if the left mouse button is down and there are bullets
            if (readyToShoot && shooting && !reloading && bulletsLeft > 0){
                bulletsShot = bulletsTap;
                Shoot();
            }
            //reload if there are no bullets left QOL
            else if (bulletsLeft <= 0){
                Reload();
            }
        }

        
    }

    //reload the gun
    private void Reload(){
        reloading = true;
        Invoke("ResetReload", reloadTime);
    }

    //shot a bullet in the target direction
    private void Shoot(){
        readyToShoot = false;
        bulletsLeft--;
        bulletsShot--;
        RecoilObject.recoil += 0.1f;

        //shoot a raycast in the forward direction
        if(Physics.Raycast(mainCamera.position, mainCamera.forward, out rayHit, range, enemy)){
            //if it hits an enemy
            if (rayHit.collider.CompareTag("Enemy")){
                rayHit.collider.GetComponent<AIEnemy>().TakeDamage(damage);
            }
        }
        //create a muzzle flash
        Instantiate(muzzleFlash, AttackPoint.position, Quaternion.identity, AttackPoint);
        // play sound
        gunAudio.Play();

        Invoke("ResetShots", timeBetweenShooting);

        if (bulletsShot > 0 && bulletsLeft > 0){
            Invoke("Shoot", timeBetweenShots);
        }
    }
    
    private void ResetShots(){
        readyToShoot = true;
    }

    private void ResetReload(){
        bulletsLeft = magazine;
        reloading = false;
    }
}
