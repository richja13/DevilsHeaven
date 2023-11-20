using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaManagerScript : MonoBehaviour
{
    public static StaminaManagerScript Instance;
    [SerializeField] private Slider StaminaBar;
    public float MaxStamina;
    public float currentStamina;  
    bool CanRestore;  
    public float RestoreStaminaSpeed = 0.1f;

    void Awake()
    {
        Instance = this;
        StaminaBar.maxValue = MaxStamina;
    }
    void Start()
    {
        currentStamina = MaxStamina;
    }

    void Update()
    {
        if(CanRestore && currentStamina < MaxStamina) RestoreStamina();
        StaminaTimer();
        StaminaBar.value = currentStamina;
    }

    void RestoreStamina()
    {
        currentStamina += RestoreStaminaSpeed;
    }

    float Timer = 0;

    public void SubtractStamina(float value)
    {
        currentStamina -= value;
        CanRestore = false;
        Timer = 1f;
    }

    public void StaminaTimer()
    {
        if(!CanRestore)
        {
            if(Timer > 0) Timer -= Time.deltaTime;
            else CanRestore = true;
        }
    }
}
