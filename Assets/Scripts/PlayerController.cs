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
        transform.LookAt(nextPos);      // ROTATION WAS THIS SIMPLE AGHGHGHAGHAGH

        // Viewport Clamping code - Keeps the ship in the camera at all times.
        Vector3 viewportPos = mainCam.WorldToViewportPoint(transform.TransformPoint(nextPos));
        float viewX = Mathf.Clamp(viewportPos.x, .1f, .9f);
        float viewY = Mathf.Clamp(viewportPos.y, .1f, .9f);

        nextPos = transform.InverseTransformPoint(mainCam.ViewportToWorldPoint(new Vector3(viewX, viewY, viewportPos.z)));
        Vector3 lerpedPos = Vector3.Lerp(transform.localPosition, nextPos, moveSpeed);
        lerpedPos.Set(lerpedPos.x, lerpedPos.y, 10f);
        transform.localPosition = lerpedPos;
    }
}
