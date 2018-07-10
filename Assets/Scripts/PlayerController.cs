using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    public int health = 100;
    [Range(0f, .5f)]
    public float moveSpeed = .2f;
    private float moveHori;
    private float moveVert;

    private Camera mainCam;

    void Start() {
        mainCam = Camera.main;
    }

    void Update()
    {
        //movePlayer_NotChild();
        movePlayer_AsChild();
    }

    //Private Functions

    // This function works for moving the player around without making them a child of the main camera, but that comes with some visual quirks.
    private void movePlayer_NotChild() {
        moveHori = Input.GetAxis("Horizontal");
        moveVert = Input.GetAxis("Vertical");

        // Code to keep ship moving correctly within the camera viewport.
        Vector3 camUp = mainCam.transform.up;
        Vector3 camRight = mainCam.transform.right;
        camUp.x = 0f;
        camRight.y = 0f;
        Vector3 nextPos = transform.position + (camRight * moveHori + camUp * moveVert).normalized;

        // Viewport Clamping code - Keeps the ship in the camera at all times.
        Vector3 viewportPos = mainCam.WorldToViewportPoint(nextPos);
        float viewX = Mathf.Clamp(viewportPos.x, .1f, .9f);
        float viewY = Mathf.Clamp(viewportPos.y, .1f, .9f);

        nextPos = mainCam.ViewportToWorldPoint(new Vector3(viewX, viewY, 10f));
        Vector3 lerpedPos = Vector3.Lerp(transform.position, nextPos, moveSpeed);

        transform.rotation = mainCam.transform.rotation;
        transform.position = lerpedPos;
    }

    // haven't figured out how to rotate the ship with this, which makes the design of the game a bit more difficult.
    private void movePlayer_AsChild()
    {
        moveHori = Input.GetAxis("Horizontal");
        moveVert = Input.GetAxis("Vertical");

        Vector3 nextPos = transform.localPosition + new Vector3(moveHori, moveVert).normalized;

        // Viewport Clamping code - Keeps the ship in the camera at all times.
        Vector3 viewportPos = mainCam.WorldToViewportPoint(transform.TransformPoint(nextPos));
        float viewX = Mathf.Clamp(viewportPos.x, -0.1f, 1.1f);
        float viewY = Mathf.Clamp(viewportPos.y, -0.1f, 1.1f);

        // This apparently works even though earlier testing showed that it didn't.
        Vector3 lookAtPoint = mainCam.ViewportToWorldPoint(new Vector3(viewX, viewY, 10f)) + (mainCam.transform.right * moveHori + mainCam.transform.up * moveVert) + mainCam.transform.forward * 10f;
        transform.LookAt(lookAtPoint);

        //I don't know what problem I was trying to solve here, but i wasted a lot of time on it. Maybe could be used in movement as not a child object.
        /*
        Vector3 lookAtPointHori = mainCam.ViewportToWorldPoint(new Vector3(viewX, viewY, 10f)) + (mainCam.transform.right * moveHori) + mainCam.transform.forward * 10f;
        Vector3 lookAtPointVert = mainCam.ViewportToWorldPoint(new Vector3(viewX, viewY, 10f)) + (mainCam.transform.up * moveVert) + mainCam.transform.forward * 10f;
        float xRot = (Mathf.Atan2((lookAtPointVert - transform.position).magnitude, (lookAtPointVert - transform.position).y) * Mathf.Rad2Deg - 90f);
        float yRot = -(Mathf.Atan2((lookAtPointHori - transform.position).z, (lookAtPointHori - transform.position).x) * Mathf.Rad2Deg - 90f);
        //xRot = Mathf.Clamp(xRot, -15f, 15f);
        //yRot = Mathf.Clamp(yRot, -15f, 15f);
        Vector3 rot = new Vector3(xRot, yRot, 0f);
        transform.localRotation = Quaternion.Euler(rot);
        */


        nextPos = transform.InverseTransformPoint(mainCam.ViewportToWorldPoint(new Vector3(viewX, viewY, viewportPos.z)));
        Vector3 lerpedPos = Vector3.Lerp(transform.localPosition, nextPos, moveSpeed);
        lerpedPos.Set(lerpedPos.x, lerpedPos.y, 10f);
        transform.localPosition = lerpedPos;


    }
}
