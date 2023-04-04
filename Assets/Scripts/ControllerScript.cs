/*
Samuel Su
251103293
CS4483B
This script controls the UI and save data
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ControllerScript : MonoBehaviour
{
    public static bool paused = false;

    public GameObject endPillar;
    public GameObject startPillar;
    public GameObject menu;
    public TextMeshProUGUI bestText;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI currentLabel;
    public TextMeshProUGUI bestLabel;
    public TextMeshProUGUI description;
    public Transform mainCamera;
    public LayerMask startEvent;

    public float maxDistance;
    public bool start = false;
    public TimeScriptableObject time;

    public bool stopTimer = false;

    public float currentTime;
    
    //set all the basic UI and variables to default values
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        startPillar.SetActive(true);
        endPillar.SetActive(false);
        currentTime = 0;
        start = false;
        string bestTime = time.GetTime().ToString();
        bestText.SetText(bestTime);
        bestText.enabled = false;
        currentLabel.enabled = false;
        bestLabel.enabled = false;
        timeText.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        //if the player presses the escape key
        if (Input.GetKeyDown(KeyCode.Escape)){
            //either resume the game or pause the game
            if (paused){
                Resume();
            }
            else{
                Pause();
            }
        }
        //if we stop the timer check to see if we need to change the best time
        if (stopTimer){
            ChangeBest();
            endPillar.SetActive(false);
        }
        //if the player started the game
        else if (start){
            //count up the timer
            currentTime = currentTime + Time.deltaTime;
            timeText.SetText(currentTime.ToString());
        }

        //check if the player is looking at the start pillar
        RaycastHit target;
        if (Physics.Raycast(mainCamera.position, mainCamera.forward, out target, maxDistance, startEvent)){
            bestLabel.enabled = true;
            bestText.enabled = true;
            description.enabled = true;
            //when they press E they start the race
            if (Input.GetKeyDown(KeyCode.E)){
                StartEvent();
            }
        }
        //remove the UI when not looking at the start pillar
        else{
            bestLabel.enabled = false;
            bestText.enabled = false;
            description.enabled = false;
        }

    }

    //resume the game on pause
    public void Resume(){
        //remove and restrict hte mouse cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        //remove the pause menu
        menu.SetActive(false);
        //resume time
        Time.timeScale = 1f;
        paused = false;
    }
    
    //pause the game
    public void Pause(){
        //show and unrestrict the mouse cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        //show the pause menu
        menu.SetActive(true);
        //pause the game
        Time.timeScale = 0f;
        //set pause variable
        paused = true;
    }

    //restart the game
    public void Restart(){
        Resume();
        //load the scene to reset
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    //quit the game
    public void QuitGame(){
        //quit the game
        Application.Quit();
    }

    //check to see if the best time needs to be changed
    private void ChangeBest(){
        //use the scriptable object function to see if the best time got beaten
        time.SetBestTime(currentTime);
        //change the best time text
        bestText.SetText(time.GetTime().ToString());
    }

    //if the game is finished we need to stop the timer
    public void Finish(){
        stopTimer = true;
    }

    private void StartEvent(){
        currentLabel.enabled = true;
        timeText.enabled = true;
        start = true;
        startPillar.SetActive(false);
        endPillar.SetActive(true);
        bestLabel.enabled = false;
        bestText.enabled = false;
        description.enabled = false;
    }

}
