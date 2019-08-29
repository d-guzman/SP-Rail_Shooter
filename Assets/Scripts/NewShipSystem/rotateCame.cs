using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotateCame : MonoBehaviour
{
    private float moveHori;
    private float moveVert;
    private Vector3 movement;

    public float moveSpeed = 10f;
    public float lerpRate = .2f;

    [Header("Movement Limits")]
    public float horiOffsetMax = 10f;
    public float vertOffsetMax = 10f;
    private Vector3 centralPoint;

    void Start()
    {
        centralPoint = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // Get input from the joystick.
        moveHori = Input.GetAxis("Horizontal");
        moveVert = Input.GetAxis("Vertical");
        movement = new Vector3(moveHori, moveVert, 0f);

        // Normalize the movement vector to prevent excessive speed.
        if (movement.magnitude > 1f)
        {
            movement.Normalize();
        }

        // Lerp between camera's current position and its next position.
        if (movement.magnitude != 0f)
        {
            Vector3 nextPosition = Vector3.Lerp(transform.position, (movement * moveSpeed), lerpRate * Time.deltaTime);
            float nextX = Mathf.Clamp(nextPosition.x, -horiOffsetMax, horiOffsetMax);
            float nextY = Mathf.Clamp(nextPosition.y, -vertOffsetMax, vertOffsetMax);
            nextPosition.Set(nextX, nextY, transform.position.z);
            transform.position = nextPosition;
        }
    }
}
