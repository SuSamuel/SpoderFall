using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class healthsystem : MonoBehaviour
{
    public int maxHP = 100;
    public int currentHP = 100;
    public Slider hpSlider;

    void Start()
    {
        hpSlider.interactable = false;
        UpdateHPSlider();
    }

    public void DecreaseHP(int amount)
    {
        currentHP -= amount;
        if (currentHP < 0)
        {
            currentHP = 0;
        }
        UpdateHPSlider();
    }

    public void IncreaseHP(int amount)
    {
        currentHP += amount;
        if (currentHP > maxHP)
        {
            currentHP = maxHP;
        }
        UpdateHPSlider();
    }

    void UpdateHPSlider()
    {
        hpSlider.value = (float)currentHP / (float)maxHP;
    }

}
