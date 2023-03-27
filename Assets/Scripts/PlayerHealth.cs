using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public int maxHP = 100;
    public int currentHP = 100;
    public Slider hpSlider;

    public Image blackScreen;
    public float fadeDuration = 2.0f;
    public float restartDelay = 1.0f;
    void Start()
    {
        hpSlider.interactable = false;
        UpdateHPSlider();
    }
    //for testing decrease HP
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C)) // Replace with your desired input condition
        {
            DecreaseHP(20);
        }
    }
    
    //decrease HP
    public void DecreaseHP(int amount)
    {
        currentHP -= amount;
        if (currentHP < 0)
        {
            currentHP = 0;
            
            StartCoroutine(FadeOutAndRestart());
        }
        UpdateHPSlider();
    }
    
    //increase HP
    public void IncreaseHP(int amount)
    {
        currentHP += amount;
        if (currentHP > maxHP)
        {
            currentHP = maxHP;
        }
        UpdateHPSlider();
    }

    
    //update HP bar
    void UpdateHPSlider()
    {
        hpSlider.value = (float)currentHP / (float)maxHP;
    }
    
    //when HP go to 0
    IEnumerator FadeOutAndRestart()
    {
        float elapsedTime = 0.0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0, 1, elapsedTime / fadeDuration);
            blackScreen.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        yield return new WaitForSeconds(restartDelay);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}
