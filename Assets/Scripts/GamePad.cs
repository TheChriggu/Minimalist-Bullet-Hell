using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GamePad : MonoBehaviour
{
    float timer;

    float currentStrength = 0;

    public void SetPad(float left, float right, float time)
    {
        float newStrength = left+right;
        if (currentStrength < newStrength)
        {
            timer = time;        
            Gamepad.current.SetMotorSpeeds(left, right);
        }

    }

    public void StopPad()
    {
        Gamepad.current.SetMotorSpeeds(0, 0);
        currentStrength = 10000;
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if(timer <= 0)
        {
            Gamepad.current.SetMotorSpeeds(0.0f, 0.0f);
            currentStrength = 0;
        }
    }
}
