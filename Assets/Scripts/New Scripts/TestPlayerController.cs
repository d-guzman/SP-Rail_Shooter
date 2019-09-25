using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TestPlayerController : MonoBehaviour
{
    // Variables for Ship Management.
    public PlayerShipData[] shipData;
    private int shipDataIndex = 0;
    private GameObject currentShip;

    [Header("Boost/Brake Variables")]
    private float zPosition;
    public float zPositionDefault = 10f;
    public float zPositionBoost = 15f;
    public float zPositionBrake = 5f;
    public float zPositionAcceleration = 5f;

    // Variables for rotating ship when pressing bumpers.
    private Vector3 maxLRotation = new Vector3(0f, 0f, 90f);
    private Vector3 maxRRotation = new Vector3(0f, 0f, 270f);
    private float bumperRotateSpeed = .2f;
    
    // Main Camera is used in movement code.
    private Camera mainCam;

    // Variables used when entering triggers/collisions.
    private bool gotItemPickup = false;
    private bool isHit = false;
    [Header("Misc. Variables for Things")]
    public Rigidbody rb;
    public CameraMovement camScript;
    public int framesToHoldForCharge = 12;
    private int framesShootHeld = 0;

    void Start()
    {
        mainCam = Camera.main;
        zPosition = zPositionDefault;
        SetupShip();
    }

    void Update()
    {
        CheckInput_Boost();
        CheckInput_Brake();
        ManageEnergy();
        PerformBumperAbility();
        ShootBomb();
        MoveShip();
        DebugKeyInputTests();
    }

    void FixedUpdate()
    {
        ShootWeapons();                             // This is in FixedUpdate because I want a frame-rate independent(?) counting of frames.
    }

    // Private Functions
    private void SetupShip()
    {
        currentShip = Instantiate(shipData[shipDataIndex].ShipModel, transform, false);
        shipData[shipDataIndex].UpdateFunction.Invoke();
    }
    private void MoveShip()
    {
        // Get input data.
        float moveVert = Input.GetAxis("Vertical");
        float moveHori = Input.GetAxis("Horizontal");
        float rotateValue = Input.GetAxis("Rotate");

        // Make the movement vector and normalize input.
        Vector3 movement = new Vector3(moveHori, moveVert, 0f);
        if (movement.magnitude > 1)
            movement.Normalize();

        // Change horizontal move speed when rotating ship; Move with rotation = faster, Move against rotation = slower.
        if (moveHori != 0f)
            movement += new Vector3(-rotateValue / 3.5f, 0f, 0f);

        // Make a vector for the next position for the ship, and clamp it within the bounds of the camera.
        Vector3 nextPos = transform.localPosition + movement;
        Vector3 nextViewportPos = mainCam.WorldToViewportPoint(transform.TransformPoint(nextPos));
        float viewX = Mathf.Clamp(nextViewportPos.x, 0f, 1f);
        float viewY = Mathf.Clamp(nextViewportPos.y, 0f, 1f);

        // Angle the ship in the direction of movement.
        Vector3 lookAtPoint = mainCam.ViewportToWorldPoint(new Vector3(viewX, viewY, zPosition)) + (mainCam.transform.right * moveHori + mainCam.transform.up * moveVert) + mainCam.transform.forward * zPosition;
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

        // IF BOOSTING - Set the zPosition to move farther away from camera.
        // IF BRAKING - Set the zPosition to move closer to camera.
        bool isBoosting = shipData[shipDataIndex].CheckStateFlag<ShipState>(shipData[shipDataIndex].runtimeShipState, ShipState.isBoosting);
        bool isBraking = shipData[shipDataIndex].CheckStateFlag<ShipState>(shipData[shipDataIndex].runtimeShipState, ShipState.isBraking);

        if (isBoosting)
        {
            if (zPosition < (zPositionBoost - .05f))
                zPosition = Mathf.Lerp(zPosition, zPositionBoost, zPositionAcceleration * Time.deltaTime);
            else
                zPosition = zPositionBoost;
        }
        else if (isBraking)
        {
            if (zPosition > (zPositionBrake + .05f))
                zPosition = Mathf.Lerp(zPosition, zPositionBrake, zPositionAcceleration * Time.deltaTime);
            else
                zPosition = zPositionBrake;
        }
        else if (!isBoosting && !isBraking && zPosition != zPositionDefault)
        {
            if (zPosition > (zPositionDefault + .05f) || zPosition < (zPositionDefault - .05f))
                zPosition = Mathf.Lerp(zPosition, zPositionDefault, zPositionAcceleration * Time.deltaTime);
            else
                zPosition = zPositionDefault;
        }

        // Move the ship
        nextPos = transform.InverseTransformPoint(mainCam.ViewportToWorldPoint(new Vector3(viewX, viewY, nextViewportPos.z)));
        Vector3 lerpedPos = Vector3.Lerp(transform.localPosition, nextPos, shipData[shipDataIndex].ShipSpeed * Time.deltaTime);
        lerpedPos.Set(lerpedPos.x, lerpedPos.y, zPosition);
        transform.localPosition = lerpedPos;
    }
    private void ManageEnergy()
    {
        // --- ENERGY MANAGEMENT CODE ---
        // If player is not boosting, braking, or charging(?), build up energy.
        bool isBoosting = shipData[shipDataIndex].CheckStateFlag<ShipState>(shipData[shipDataIndex].runtimeShipState, ShipState.isBoosting);
        bool isBraking = shipData[shipDataIndex].CheckStateFlag<ShipState>(shipData[shipDataIndex].runtimeShipState, ShipState.isBraking);

        if (!isBoosting && !isBraking)
        {
            if (shipData[shipDataIndex].runtimeShipEnergy != shipData[shipDataIndex].ShipEnergy)
            {
                shipData[shipDataIndex].runtimeShipEnergy++;
            }
        }

        // If player is boosting or braking, decrease energy.
        if (isBoosting || isBraking)
        {
            if (shipData[shipDataIndex].runtimeShipEnergy > 0)
            {
                if ((shipData[shipDataIndex].runtimeShipEnergy - 2) <= 0)
                {
                    shipData[shipDataIndex].runtimeShipEnergy = 0;
                }
                else
                {
                    shipData[shipDataIndex].runtimeShipEnergy -= 4;
                }
            }
            else
            {
                shipData[shipDataIndex].UnsetStateFlag<ShipState>(ref shipData[shipDataIndex].runtimeShipState, ShipState.isBoosting);
                shipData[shipDataIndex].UnsetStateFlag<ShipState>(ref shipData[shipDataIndex].runtimeShipState, ShipState.isBraking);
                camScript.isBoosting = false;
                camScript.isBraking = false;
            }
        }
    }
    private void CheckInput_Boost()
    {
        // --- CHECK BOOST INPUT CODE ---
        // Can't boost if already braking.
        if (Input.GetButtonDown("Boost") && !shipData[shipDataIndex].CheckStateFlag<ShipState>(shipData[shipDataIndex].runtimeShipState, ShipState.isBraking))
        {
            camScript.isBoosting = true;        // still need this cause the camera needs references to the shipdata (fuck).
            shipData[shipDataIndex].SetStateFlag<ShipState>(ref shipData[shipDataIndex].runtimeShipState, ShipState.isBoosting);
        }
        else if (Input.GetButtonUp("Boost"))
        {
            camScript.isBoosting = false;
            shipData[shipDataIndex].UnsetStateFlag<ShipState>(ref shipData[shipDataIndex].runtimeShipState, ShipState.isBoosting);
        }
    }
    private void CheckInput_Brake()
    {
        // --- CHECK BRAKE INPUT CODE ---
        // Can't brake if already boosting.
        if (Input.GetButtonDown("Brake") && !shipData[shipDataIndex].CheckStateFlag<ShipState>(shipData[shipDataIndex].runtimeShipState, ShipState.isBoosting))
        {
            camScript.isBraking = true;
            shipData[shipDataIndex].SetStateFlag<ShipState>(ref shipData[shipDataIndex].runtimeShipState, ShipState.isBraking);
        }
        else if (Input.GetButtonUp("Brake"))
        {
            camScript.isBraking = false;
            shipData[shipDataIndex].UnsetStateFlag<ShipState>(ref shipData[shipDataIndex].runtimeShipState, ShipState.isBraking);
        }
    }
    private void PerformBumperAbility()
    {
        if (Input.GetButtonDown("Rotate"))
        {
            if (Input.GetAxis("Rotate") == 0 && (shipData[shipDataIndex].runtimeShipEnergy >= shipData[shipDataIndex].BumperAbilityCost))
            {
                shipData[shipDataIndex].BumperFunction.Invoke();
                shipData[shipDataIndex].runtimeShipEnergy -= shipData[shipDataIndex].BumperAbilityCost;
            }
        }
    }
    private void ShootWeapons()
    {
        if (Input.GetButtonDown("Shoot"))
        {
            shipData[shipDataIndex].ShootFunction.Invoke();
        }
        if (Input.GetButton("Shoot"))
        {
            framesShootHeld++;
            if (framesShootHeld == framesToHoldForCharge)
            {
                Debug.Log("Shoot Button held for " + framesToHoldForCharge.ToString() + " frames. Start Charging.");
            }
        }
        if (Input.GetButtonUp("Shoot"))
        {
            framesShootHeld = 0;
        }
    }
    private void ShootBomb()
    {
        if (Input.GetButtonDown("Bomb"))
        {
            if (shipData[shipDataIndex].runtimeShipBombCount > 0)
            {
                Debug.Log("Bomb Button pressed. Bombs Away!");
                shipData[shipDataIndex].runtimeShipBombCount--;
                shipData[shipDataIndex].BombFunction.Invoke();
            }
            else
                Debug.Log("Bomb Button pressed. No more bombs...");
        }
    }
    private void DebugKeyInputTests()
    { 
        
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            shipData[shipDataIndex].runtimeWeaponLevel = 1;
            shipData[shipDataIndex].UpdateFunction.Invoke();
            Debug.Log("Ship's weapon level set to: " + shipData[shipDataIndex].runtimeWeaponLevel.ToString());
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            shipData[shipDataIndex].runtimeWeaponLevel = 2;
            shipData[shipDataIndex].UpdateFunction.Invoke();
            Debug.Log("Ship's weapon level set to: " + shipData[shipDataIndex].runtimeWeaponLevel.ToString());
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            shipData[shipDataIndex].runtimeWeaponLevel = 3;
            shipData[shipDataIndex].UpdateFunction.Invoke();
            Debug.Log("Ship's weapon level set to: " + shipData[shipDataIndex].runtimeWeaponLevel.ToString());
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


    // Coroutines
    IEnumerator takeDamage(int damage, int waitTime)
    {
        shipData[shipDataIndex].runtimeShipHealth -= damage;

        float healthRatio = (float)shipData[shipDataIndex].runtimeShipHealth / shipData[shipDataIndex].ShipHealth;
        if (healthRatio < 1f && healthRatio >= .6f)
            shipData[shipDataIndex].runtimeShipStatus = ShipStatus.LightDamage;
        else if (healthRatio < .6f && healthRatio >= .2f)
            shipData[shipDataIndex].runtimeShipStatus = ShipStatus.HeavyDamage;
        else if (healthRatio < .2f && healthRatio > 0f)
            shipData[shipDataIndex].runtimeShipStatus = ShipStatus.CriticalDamage;
        else
            shipData[shipDataIndex].runtimeShipStatus = ShipStatus.Down;

        yield return new WaitForSeconds((waitTime / 60));
        shipData[shipDataIndex].UnsetStateFlag<ShipState>(ref shipData[shipDataIndex].runtimeShipState, ShipState.gotHit);
    }
    IEnumerator shoveShip()
    {
        rb.AddForce(transform.up * 30f, ForceMode.Impulse);
        yield return new WaitForSeconds(.09f);
        rb.velocity = Vector3.zero;
        rb.ResetInertiaTensor();
    }
    IEnumerator getPowerup(ItemType itemType)
    {
        yield return new WaitForEndOfFrame();                                           // Need to delay incrementing the weapon level until EOF, to avoid multiple calls to coroutine.
        if (itemType == ItemType.WeaponUpgrade)
        {
            if (shipData[shipDataIndex].runtimeWeaponLevel != 3)
                shipData[shipDataIndex].runtimeWeaponLevel++;
            shipData[shipDataIndex].UpdateFunction.Invoke();
        }
        else if (itemType == ItemType.BombPickup)
        {
            shipData[shipDataIndex].runtimeShipBombCount++;
        }
        shipData[shipDataIndex].UnsetStateFlag<ShipState>(ref shipData[shipDataIndex].runtimeShipState, ShipState.gotItem);
    }


    // OnTrigger and OnCollision methods
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Level")
        {
            if (!shipData[shipDataIndex].CheckStateFlag<ShipState>(shipData[shipDataIndex].runtimeShipState, ShipState.gotHit))
            {
                shipData[shipDataIndex].SetStateFlag<ShipState>(ref shipData[shipDataIndex].runtimeShipState, ShipState.gotHit);
                StartCoroutine(takeDamage(10, 60));
            }
        }
        else if (other.tag == "WeaponUpgrade")
        {
            Destroy(other.gameObject);
            if (!shipData[shipDataIndex].CheckStateFlag<ShipState>(shipData[shipDataIndex].runtimeShipState, ShipState.gotItem))
            {
                shipData[shipDataIndex].SetStateFlag<ShipState>(ref shipData[shipDataIndex].runtimeShipState, ShipState.gotItem);
                StartCoroutine(getPowerup(ItemType.WeaponUpgrade));
            }
        }
        else if (other.tag == "BombPickup")
        {
            Destroy(other.gameObject);
            if (!shipData[shipDataIndex].CheckStateFlag<ShipState>(shipData[shipDataIndex].runtimeShipState, ShipState.gotItem))
            {
                shipData[shipDataIndex].SetStateFlag<ShipState>(ref shipData[shipDataIndex].runtimeShipState, ShipState.gotItem);
                StartCoroutine(getPowerup(ItemType.BombPickup));
            }
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            if (!shipData[shipDataIndex].CheckStateFlag<ShipState>(shipData[shipDataIndex].runtimeShipState, ShipState.gotHit))
            {
                shipData[shipDataIndex].SetStateFlag<ShipState>(ref shipData[shipDataIndex].runtimeShipState, ShipState.gotHit);
                StartCoroutine(takeDamage(10, 15));
                StartCoroutine(shoveShip());
            }
        }
    }
}