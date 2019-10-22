using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DollyController : MonoBehaviour
{
    public Cinemachine.CinemachineDollyCart dollyCartScript;
    public bool isBoosting;
    public bool isBraking;

    public float defaultSpeed;
    public float boostSpeed;
    public float brakeSpeed;

    void Update()
    {
        if (isBoosting)
            dollyCartScript.m_Speed = boostSpeed;
        else if (isBraking)
            dollyCartScript.m_Speed = brakeSpeed;
        else
            dollyCartScript.m_Speed = defaultSpeed;
    }
}
