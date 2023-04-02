using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerShoot : MonoBehaviour
{
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

    bool shooting;
    bool readyToShoot;
    bool reloading;

    public Transform mainCamera;
    public Transform AttackPoint;
    public RaycastHit rayHit;
    public LayerMask enemy;

    public GameObject muzzleFlash;
    //public GameObject bulletHole;

    public TextMeshProUGUI text;

    public Recoil RecoilObject;

    private void Update(){
        GetInput();
        text.SetText(bulletsLeft + " / " + magazine);
    }

    private void Awake(){
        bulletsLeft = magazine;
        readyToShoot = true;
    }

    private void OnEnable(){
        readyToShoot = true;
    }

    private void OnDisable(){
        readyToShoot = false;
    }
    private void GetInput(){
        if (allowHold){
            shooting = Input.GetMouseButton(0);
        }
        else{
            shooting = Input.GetMouseButtonDown(0);
        }

        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazine && !reloading){
            Reload();
        }

        if (readyToShoot && shooting && !reloading && bulletsLeft > 0){
            bulletsShot = bulletsTap;
            Shoot();
        }
    }

    private void Reload(){
        reloading = true;
        Invoke("ResetReload", reloadTime);
    }

    private void Shoot(){
        readyToShoot = false;
        bulletsLeft--;
        bulletsShot--;
        RecoilObject.recoil += 0.1f;
        //spread for AI and other guns
        //replace maincamera.forward with direction to implement
        //float x = Random.Range(-spread, spread);
        //float y = Random.Range(-spread, spread);

        //Vector3 direction = mainCamera.forward + new Vector3(x, y, 0);

        if(Physics.Raycast(mainCamera.position, mainCamera.forward, out rayHit, range, enemy)){
            if (rayHit.collider.CompareTag("Enemy")){
                //rayHit.collider.GetComponent<AIhealth>().TakeDamage(damage);
            }
        }

        //Instantiate(bulletHole, rayHit.point, Quaternion.Euler(0,180,0));

        Instantiate(muzzleFlash, AttackPoint.position, Quaternion.identity, AttackPoint);

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
