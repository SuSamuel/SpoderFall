/*
Samuel Su
251103293
CS4483B
This scriptable object take stores the best time achieve by the user
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class TimeScriptableObject : ScriptableObject
{
    private float BestTime = 0;

    //load the BestTime from the player preferences with a default value of 0
    public void Awake(){
        BestTime = PlayerPrefs.GetFloat("BestTime", 0);
    }

    //getter method for hte best time
    public float GetTime(){
        return BestTime;
    }

    //Setter method for hte best time
    public void SetBestTime(float time){
        //if the time achieved is less than the current best time, or hte current best time is the default 0
        if (time < BestTime || BestTime == 0){
            //store a new best time
            PlayerPrefs.SetFloat("BestTime", time);
            //save
            PlayerPrefs.Save();
            //change the value stored in the variable to match the new best time
            BestTime = time;
        }
    }
}
