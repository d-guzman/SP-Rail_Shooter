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

    /*
    [Header("Testing Track Swapping")]
    public bool SwapToTrack2 = false;
    public Vector3 swapPoint;
    public Cinemachine.CinemachinePathBase primaryTrack;
    public Cinemachine.CinemachinePathBase secondaryTrack;
    private float positionUnitsBeforeSwap;
    [SerializeField]
    private bool ifSwapFinished = false;
    */

    void Start()
    {

    }
    void Update()
    {
        if (isBoosting)
            dollyCartScript.m_Speed = boostSpeed;
        else if (isBraking)
            dollyCartScript.m_Speed = brakeSpeed;
        else
            dollyCartScript.m_Speed = defaultSpeed;

        /*
        float distanceFromSwapPoint = Vector3.Distance(transform.position, swapPoint);
        if (distanceFromSwapPoint < 20f)
            Debug.Log("Close to Swap Point!");

        
        if (SwapToTrack2)
        {
            if (dollyCartScript.m_Path != secondaryTrack)
            {
                ifSwapFinished = false;
                if (distanceFromSwapPoint < 10f)
                {
                    positionUnitsBeforeSwap = dollyCartScript.m_Position;
                    if (!ifSwapFinished)
                    {
                        dollyCartScript.m_Position = 0;
                        dollyCartScript.m_Path = secondaryTrack;
                        ifSwapFinished = true;
                    }
                }
            }
        }
        else
        {
            if (dollyCartScript.m_Path != primaryTrack)
            {
                ifSwapFinished = false;
                if (distanceFromSwapPoint < 10f)
                {
                    if (!ifSwapFinished)
                    {
                        dollyCartScript.m_Position = positionUnitsBeforeSwap;
                        dollyCartScript.m_Path = primaryTrack;
                        ifSwapFinished = true;
                    }
                }
            }
        }
        */
    }
}
