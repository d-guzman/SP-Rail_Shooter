using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TestPlayerController : MonoBehaviour
{
    public PlayerShipData[] shipData;
    private int shipDataIndex = 0;
    public int WeaponLevel = 1;
    private GameObject currentShip;

    private Vector3 maxLRotation = new Vector3(0f, 0f, 90f);
    private Vector3 maxRRotation = new Vector3(0f, 0f, 270f);
    private float bumperRotateSpeed = .2f;
    private Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;
        SetupShip();
    }

    void Update()
    {
        float moveVert = Input.GetAxis("Vertical");
        float moveHori = Input.GetAxis("Horizontal");
        float rotateValue = Input.GetAxis("Rotate");

        Vector3 movement = new Vector3(moveHori, moveVert, 0f);
        if (movement.magnitude > 1)
            movement.Normalize();

        if (moveHori != 0f)
            movement += new Vector3(-rotateValue / 3.5f, 0f, 0f);

        Vector3 nextPos = transform.localPosition + movement;
        Vector3 nextViewportPos = mainCam.WorldToViewportPoint(transform.TransformPoint(nextPos));
        float viewX = Mathf.Clamp(nextViewportPos.x, 0f, 1f);
        float viewY = Mathf.Clamp(nextViewportPos.y, 0f, 1f);

        // Angle the ship in the direction of movement.
        //Vector3 angleDirection = transform.TransformPoint(nextPos + new Vector3(0f, 0f, 1.5f));
        //transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(angleDirection), Mathf.Deg2Rad*120f);

        Vector3 lookAtPoint = mainCam.ViewportToWorldPoint(new Vector3(viewX, viewY, 10f)) + (mainCam.transform.right * moveHori + mainCam.transform.up * moveVert) + mainCam.transform.forward * 11.5f;
        transform.LookAt(lookAtPoint);

        // Rotate the ship model if the bumpers are pressed.
        if (rotateValue == -1)
        {
            if (currentShip.transform.localRotation.eulerAngles.z > 270.5f)
            {
                Vector3 meshRotation = Vector3.Lerp(currentShip.transform.localRotation.eulerAngles, maxRRotation, bumperRotateSpeed);
                currentShip.transform.localRotation = Quaternion.Euler(meshRotation);
            }
            else if (currentShip.transform.localRotation.eulerAngles.z < 90.5f && currentShip.transform.localRotation.eulerAngles.z > 20f)
            {
                Vector3 meshRotation = Vector3.Lerp(currentShip.transform.localRotation.eulerAngles, Vector3.zero, bumperRotateSpeed);
                currentShip.transform.localRotation = Quaternion.Euler(meshRotation);
            }
            else if (currentShip.transform.localRotation.eulerAngles.z <= 20f)
                currentShip.transform.localRotation = Quaternion.Euler(0f, 0f, 358.5f);
        }
        else if (rotateValue == 1)
        {
            if (currentShip.transform.localEulerAngles.z < 90.5f)
            {
                Vector3 meshRotation = Vector3.Lerp(currentShip.transform.localRotation.eulerAngles, maxLRotation, bumperRotateSpeed);
                currentShip.transform.localRotation = Quaternion.Euler(meshRotation);
            }
            else if (currentShip.transform.localEulerAngles.z > 269.5f && currentShip.transform.localEulerAngles.z < 340f)
            {
                Vector3 meshRotation = Vector3.Lerp(currentShip.transform.localRotation.eulerAngles, new Vector3(0f, 0f, 360f), bumperRotateSpeed);
                currentShip.transform.localRotation = Quaternion.Euler(meshRotation);
            }
            else if (currentShip.transform.localEulerAngles.z >= 340f)
                currentShip.transform.localRotation = Quaternion.Euler(0f, 0f, 1.5f);
        }
        else
        {
            if (currentShip.transform.localEulerAngles.z > 269.5f)
            {
                Vector3 meshRotation = Vector3.Lerp(currentShip.transform.localRotation.eulerAngles, new Vector3(0f, 0f, 360f), bumperRotateSpeed);
                currentShip.transform.localRotation = Quaternion.Euler(meshRotation);
                if (currentShip.transform.localEulerAngles.z > 359.5f)
                    currentShip.transform.localRotation = Quaternion.Euler(Vector3.zero);
            }
            else if (currentShip.transform.localEulerAngles.z < 90.5f)
            {
                Vector3 meshRotation = Vector3.Lerp(currentShip.transform.localRotation.eulerAngles, Vector3.zero, bumperRotateSpeed);
                currentShip.transform.localRotation = Quaternion.Euler(meshRotation);
            }
        }

        // Move the ship
        nextPos = transform.InverseTransformPoint(mainCam.ViewportToWorldPoint(new Vector3(viewX, viewY, nextViewportPos.z)));
        Vector3 lerpedPos = Vector3.Lerp(transform.localPosition, nextPos, shipData[shipDataIndex].ShipSpeed * Time.deltaTime);
        lerpedPos.Set(lerpedPos.x, lerpedPos.y, 10f);
        transform.localPosition = lerpedPos;

        
        DebugKeyInputTests();
    }

    // Private Functions
    private void SetupShip()
    {
        currentShip = Instantiate(shipData[shipDataIndex].ShipModel, transform, false);
        shipData[shipDataIndex].UpdateFunction.Invoke(WeaponLevel);
    }

    private void DebugKeyInputTests()
    { 
        if (Input.GetButtonDown("Shoot"))
        {
            shipData[shipDataIndex].ShootFunction.Invoke(WeaponLevel);
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            WeaponLevel = 1;
            shipData[shipDataIndex].UpdateFunction.Invoke(WeaponLevel);
            Debug.Log("Ship's weapon level set to: " + WeaponLevel.ToString());
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            WeaponLevel = 2;
            shipData[shipDataIndex].UpdateFunction.Invoke(WeaponLevel);
            Debug.Log("Ship's weapon level set to: " + WeaponLevel.ToString());
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            WeaponLevel = 3;
            shipData[shipDataIndex].UpdateFunction.Invoke(WeaponLevel);
            Debug.Log("Ship's weapon level set to: " + WeaponLevel.ToString());
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Debug.Log("Destroying and replacing ship!");
            shipData[shipDataIndex].EventCleanup();
            shipDataIndex = (shipDataIndex + 1) % 2;
            Destroy(currentShip);
            Invoke("SetupShip", (1 / 60));    // need to delay creating a ship by ~1 frame otherwise we get nullref errors.
        }
    }
}