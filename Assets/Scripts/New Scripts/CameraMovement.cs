using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [Header("Player Ship Transform")]
    public GameObject playerShip;

    [Header("Camera Movement Limits")]
    [Tooltip("How far the camera move along the X-axis. For both negative and positive directions!")]
    public float limitX = 10;
    [Tooltip("How far the camera move along the Y-axis. For both negative and positive directions!")]
    public float limitY = 7;
    [Tooltip("How far back the camera will be positioned on its local Z-axis.")]
    public float cameraOffset = -10f;
    [Range(0f, 1f)]
    [Tooltip("How fast the camera will keep up with the Player ship.")]
    public float smoothDampSpeed = .2f;


    [Header("Legacy Code Support")]
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
            playerShip = GameObject.FindGameObjectWithTag("Player");
        }
    }

    void Update()
    {
        UpdateGizmoVectors();
    }

    private void FixedUpdate()
    {
        Vector3 playerShipPosition = playerShip.transform.localPosition;
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, new Vector3(playerShipPosition.x, playerShipPosition.y, cameraOffset), ref velocity, smoothDampSpeed, Mathf.Infinity, Time.deltaTime);
    }

    void LateUpdate()
    {
        Vector3 camPosition = transform.localPosition;
        float camX = Mathf.Clamp(camPosition.x, -limitX, limitX);
        float camY = Mathf.Clamp(camPosition.y, -limitY, limitY);
        transform.localPosition = new Vector3(camX, camY, cameraOffset);
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(bottomLeft, topLeft);
        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
    }

    private void UpdateGizmoVectors()
    {
        bottomLeft = (transform.right * -limitX + transform.up * -limitY) + (transform.forward * cameraOffset) + transform.parent.position;
        topLeft = (transform.right * -limitX + transform.up * limitY) + (transform.forward * cameraOffset) + transform.parent.position;
        bottomRight = (transform.right * limitX + transform.up * -limitY) + (transform.forward * cameraOffset) + transform.parent.position;
        topRight = (transform.right * limitX + transform.up * limitY) + (transform.forward * cameraOffset) + transform.parent.position;
    }
}
