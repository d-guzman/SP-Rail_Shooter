using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private float moveHori;
    private float moveVert;
    private Vector3 movement;

    [Header("Offset Movement")]
    public float offsetMoveSpeed = 10f;
    private float currentSpeed;
    public float offsetAcceleration = .2f;

    [Header("Offset Limits")]
    public float horiOffsetMax = 10f;
    public float vertOffsetMax = 10f;
    private float currentHoriOffset = 0f;
    private float currentVertOffset = 0f;

    private float forwardMoveSpeed;
    [Header("Forward Movement")]
    public float forwardMoveSpeedDefault = 50f;
    public float forwardMoveSpeedBraking = 25f;
    public float forwardMoveSpeedBoosting = 75f;
    public float forwardAcceleration = 2.5f;
    private Vector3 centralPoint;

    [Header("Control Booleans")]
    public bool isBoosting;
    public bool isBraking;

    void Start()
    {
        forwardMoveSpeed = forwardMoveSpeedDefault;
        isBoosting = false;
        isBraking = false;
        centralPoint = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        /*
        // --- CHECK BOOST INPUT CODE ---
        // Can't boost if already braking.
        if (Input.GetButtonDown("Boost") && !isBraking)
        {
            isBoosting = true;
        }
        else if (Input.GetButtonUp("Boost"))
        {
            isBoosting = false;
        }

        // --- CHECK BRAKE INPUT CODE ---
        // Can't brake if already boosting.
        if (Input.GetButtonDown("Brake") && !isBoosting)
        {
            isBraking = true;
        }
        else if (Input.GetButtonUp("Brake"))
        {
            isBraking = false;
        }
        */

        // Change the forward move speed based the current action.
        if (isBoosting)
        {
            if (forwardMoveSpeed < (forwardMoveSpeedBoosting - .05f))
                forwardMoveSpeed = Mathf.Lerp(forwardMoveSpeed, forwardMoveSpeedBoosting, forwardAcceleration * Time.deltaTime);
            else
                forwardMoveSpeed = forwardMoveSpeedBoosting;
        }
        else if (isBraking)
        {
            if (forwardMoveSpeed > (forwardMoveSpeedBraking + .05f))
                forwardMoveSpeed = Mathf.Lerp(forwardMoveSpeed, forwardMoveSpeedBraking, forwardAcceleration * Time.deltaTime);
            else
                forwardMoveSpeed = forwardMoveSpeedBraking;
        }
        else if (!isBoosting && !isBraking && forwardMoveSpeed != forwardMoveSpeedDefault)
        {
            if (forwardMoveSpeed > (forwardMoveSpeedBraking + .05f) || forwardMoveSpeed < (forwardMoveSpeedBoosting - .05f))
                forwardMoveSpeed = Mathf.Lerp(forwardMoveSpeed, forwardMoveSpeedDefault, forwardAcceleration * Time.deltaTime);
            else
                forwardMoveSpeed = forwardMoveSpeedDefault;
        }

        // --- MOVEMENT CODE ---
        // Get input from the joystick.
        moveHori = Input.GetAxis("Horizontal");
        moveVert = Input.GetAxis("Vertical");
        movement = new Vector3(moveHori, moveVert, 0f);


        // Modify the Central Point's Z-value to move the camera forward.
        centralPoint.Set(centralPoint.x, centralPoint.y, (centralPoint.z + forwardMoveSpeed * Time.deltaTime));

        // Normalize the movement vector to prevent excessive speed.
        if (movement.magnitude > 1f)
        {
            movement.Normalize();
        }


        // Increase the camera's speed when input is detected.
        // Decrease the camera's speed when input is not detected        
        if (movement.magnitude == 0f)
        {
            currentSpeed = Mathf.Lerp(currentSpeed, 0f, offsetAcceleration * Time.deltaTime);                     // Why not a rigidbody to do this with less code? I was concerned that
            if (currentSpeed < .1f )                                                                             // a rigidbody might have some adverse affect to the camera, even if
            {                                                                                                   // was just a kinematic rigidbody... while that is probably is the faster
                currentSpeed = 0f;                                                                              // (probably better) way to implement this system, I figured that this
            }                                                                                                   // would provide a good amount of granular control that would be useful.
        }
        else
        {                                                                                                       // NOTE: The reason why we slow down instead of instantly going to zero
            currentSpeed = Mathf.Lerp(currentSpeed, offsetMoveSpeed, offsetAcceleration * Time.deltaTime);      // is because that would make movement both slow (cause of the time to
            if (currentSpeed > offsetMoveSpeed - .1f)                                                           // build up speed) and jerky (imagine building up speed and then stopping
            {                                                                                                   // and having to accelerate again).
                currentSpeed = offsetMoveSpeed;
            }
        }
        //Debug.Log("Current Camera Speed: " + currentSpeed);


        // Modify the offset using the input data from this frame
        // and clamp it to make sure we stay within a reasonable boundary.
        currentHoriOffset += movement.x * currentSpeed * Time.deltaTime;
        currentHoriOffset = Mathf.Clamp(currentHoriOffset, -horiOffsetMax, horiOffsetMax);

        currentVertOffset += movement.y * currentSpeed * Time.deltaTime;
        currentVertOffset = Mathf.Clamp(currentVertOffset, -vertOffsetMax, vertOffsetMax);
        //Debug.Log(" - Current Offsets - \nHorizontal: " + currentHoriOffset + "\nVertical: " + currentVertOffset);

        // Add the relevant vectors to the central point to get the camera's final position.
        Vector3 offsetVector = new Vector3(currentHoriOffset, currentVertOffset, 0f);
        transform.position = centralPoint + offsetVector;
    }
}
