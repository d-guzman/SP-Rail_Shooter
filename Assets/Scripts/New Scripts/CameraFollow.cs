using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Player Ship Transform")]
    public Rigidbody playerShip;

    [Header("Camera Limits")]
    [Tooltip("How far the camera move along the X-axis. For both negative and positive directions!")]
    public float limitX = 10;
    [Tooltip("How far the camera move along the Y-axis. For both negative and positive directions!")]
    public float limitY = 7;
    [Tooltip("How far back the camera will be positioned on its local Z-axis when not boosting or braking.")]
    public float cameraOffset = -10f;
    [Tooltip("How far back the camera will be position on its local Z-axis when boosting.")]
    public float cameraOffsetBoost = -15f;
    [Tooltip("How far back the camera will be position on its local Z-axis when braking.")]
    public float cameraOffsetBrake = -5f;
    [Tooltip("How quickly the camera will adjust its offset.")]
    public float offsetStepSpeed = .2f;
    private float currentCameraOffset;
    

    [Header("Follow Camera Variables")]
    [Tooltip("If enabled, the camera will behave as a follow camera for the player ship, instead of moving on its own.")]
    public bool enableFollowCam = false;
    [Range(0f, 1f)]
    [Tooltip("How fast the camera will keep up with the Player ship.")]
    public float smoothDampTime = .2f;

    [Header("Movement Variables")]
    public float maxSpeed = 10f;
    private float currentSpeed = 0f;
    [Range(0f, 1f)]
    public float lerpRate = .1f;

    [Header("Bools for Boost/Brake")]
    public bool isBoosting;
    public bool isBraking;

    // Vectors for drawing the box that represents the camera's
    // total range of movement at any given time.
    private Vector3 bottomLeft;
    private Vector3 topLeft;
    private Vector3 bottomRight;
    private Vector3 topRight;

    // Velocity vector needed for Vector3.SmoothDamp.
    private Vector3 velocity = Vector3.zero;

    void Start()
    {
        if (cameraOffset > 0f)
        {
            Debug.LogWarning("Camera Offset was set to be in front of player! Setting to default value (-10f)!");
            cameraOffset = -10f;
        }
        if (playerShip == null)
        {
            Debug.LogWarning("Player Ship not set! Finding the first object with \"Player\" tag!");
            playerShip = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>();
        }
        currentCameraOffset = cameraOffset;
    }

    void Update()
    {
        UpdateGizmoVectors();
        if (isBoosting)
            currentCameraOffset = Mathf.SmoothStep(currentCameraOffset, cameraOffsetBoost, offsetStepSpeed);
        else if (isBraking)
            currentCameraOffset = Mathf.SmoothStep(currentCameraOffset, cameraOffsetBrake, offsetStepSpeed);
        else
            currentCameraOffset = Mathf.SmoothStep(currentCameraOffset, cameraOffset, offsetStepSpeed);
    }

    void FixedUpdate()
    {
        if (enableFollowCam)
        {
            FollowPlayer();
            ClampPosition();
        }
        if (!enableFollowCam)
            MoveCamera();
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(bottomLeft, topLeft);
        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
    }

    private void FollowPlayer()
    {
        Vector3 shipPosition = transform.localPosition;
        Vector3 playerShipPosition = playerShip.transform.localPosition;
        transform.localPosition = Vector3.SmoothDamp(shipPosition, new Vector3(playerShipPosition.x, playerShipPosition.y, shipPosition.z), ref velocity, smoothDampTime, Mathf.Infinity, Time.smoothDeltaTime);
    }
    private void ClampPosition()
    {
        Vector3 camPosition = transform.localPosition;
        float camX = Mathf.Clamp(camPosition.x, -limitX, limitX);
        float camY = Mathf.Clamp(camPosition.y, -limitY, limitY);
        transform.localPosition = new Vector3(camX, camY, currentCameraOffset);
    }

    private void MoveCamera()
    {
        // Get player input.
        float moveHori = Input.GetAxis("Horizontal");
        float moveVert = Input.GetAxis("Vertical");

        // Create movement vector and normalize it.
        Vector3 movement = new Vector3(moveHori, moveVert, 0f);
        if (movement.magnitude > 1f)
            movement.Normalize();

        // Adjust camera's currentSpeed based on if the player is moving.
        if (movement.magnitude != 0f)
        {
            currentSpeed = Mathf.Lerp(currentSpeed, maxSpeed, lerpRate * movement.magnitude);
        }
        else
        {
            currentSpeed = Mathf.Lerp(currentSpeed, 0f, lerpRate);
        }

        // Create a new vector representing the next position of the camera.
        Vector3 nextPosition = transform.localPosition + movement * currentSpeed * Time.fixedDeltaTime;

        // Clamp it to stay within boundaries.
        nextPosition.Set(Mathf.Clamp(nextPosition.x, -limitX, limitX), Mathf.Clamp(nextPosition.y, -limitY, limitY), currentCameraOffset);

        // Set camera to new position.
        transform.localPosition = nextPosition;
    }

    private void UpdateGizmoVectors()
    {
        bottomLeft = (transform.right * -limitX + transform.up * -limitY) + (transform.forward * currentCameraOffset) + transform.parent.position;
        topLeft = (transform.right * -limitX + transform.up * limitY) + (transform.forward * currentCameraOffset) + transform.parent.position;
        bottomRight = (transform.right * limitX + transform.up * -limitY) + (transform.forward * currentCameraOffset) + transform.parent.position;
        topRight = (transform.right * limitX + transform.up * limitY) + (transform.forward * currentCameraOffset) + transform.parent.position;
    }
}
