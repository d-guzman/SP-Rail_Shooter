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

    private float zPosition;
    [Header("Boost/Brake Variables")]
    public float zPositionDefault = 10f;
    public float zPositionBoost = 15f;
    public float zPositionBrake = 5f;
    public float zPositionAcceleration = 5f;

    // Variables for movement.
    private Camera mainCam;
    private float moveHori;
    private float moveVert;
    private float bumperValue;

    [Header("Rotation Values")]
    [Tooltip("How quickly the ship will rotate (in Degrees!) when moving. For Pitch and Yaw only!")]
    public float rotationSpeed = 5f;
    [Tooltip("How much the player ship will rotate around the X-Axis when moving.")]
    public float maxPitch = 20f;
    [Tooltip("How much the player ship will rotate around the Y-Axis when moving.")]
    public float maxYaw = 20f;
    [Tooltip("How much the player ship will rotate around the Z-Axis when moving. Rotates the Ship Model instead of the Player Ship object itself!")]
    public float maxRoll = 15f;
    private float bumperRotateSpeed = 0.175f;

    // Variables used when entering triggers/collisions.
    [Header("Misc. Variables for Things")]
    new public Rigidbody rigidbody;
    public CameraMovement camScript;
    public int framesToHoldForCharge = 12;
    private int framesShootHeld = 0;

    [Header("Debug-Related")]
    public bool enableMovement = true;

    void Start()
    {
        mainCam = Camera.main;
        zPosition = zPositionDefault;
        SetupShip();
    }

    void Update()
    {
        ManageEnergy();

        CheckInput_Movement();
        CheckInput_Bumpers();
        CheckInput_Boost();
        CheckInput_Brake();

        AimShip();
        RollShip();

        ShootWeapons();
        ShootBomb();
        PerformBumperAbility();

        DebugKeyInputTests();
    }

    void FixedUpdate()
    {
        MoveShip();
        AimShip();
        RollShip();
    }

    // Private Functions
    private void SetupShip()
    {
        currentShip = Instantiate(shipData[shipDataIndex].ShipModel, transform, false);
        shipData[shipDataIndex].UpdateFunction.Invoke();
    }

    private void CheckInput_Movement()
    {
        moveVert = Input.GetAxis("Vertical");
        moveHori = Input.GetAxis("Horizontal");
    }
    private void CheckInput_Bumpers()
    {
        bumperValue = Input.GetAxis("Rotate");
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

    private void MoveShip()
    {
        // Create the movement vector from the player's input.
        Vector3 movement = (mainCam.transform.right * moveHori) + (mainCam.transform.up * moveVert);

        // Change horizontal move speed when rotating ship; Move with rotation = faster, Move against rotation = slower.
        if (moveHori != 0f)
            movement += mainCam.transform.right * (bumperValue * .33f);

        // Normalize movement vector
        if (movement.magnitude > 1f)
            movement.Normalize();

        // Create the new position for the player ship and clamp its XY values
        // to keep it in the camera's view at all times
        Vector3 nextPosition = transform.position + movement * shipData[shipDataIndex].ShipSpeed * Time.deltaTime;
        nextPosition = mainCam.WorldToViewportPoint(nextPosition);
        nextPosition.Set(Mathf.Clamp01(nextPosition.x), Mathf.Clamp01(nextPosition.y), nextPosition.z);

        // Convert to world space and use rigidbody MovePosition.
        nextPosition = mainCam.ViewportToWorldPoint(nextPosition);
        rigidbody.MovePosition(nextPosition);

    }
    private void AimShip()
    {
        // Calculate what the current Yaw and Pitch of the ship should be based
        // on the current input values.
        float currentYaw = maxYaw * moveHori;
        float currentPitch = maxPitch * -moveVert;

        // Make a new rotation from the current pitch and yaw values and 
        Quaternion newRotation = Quaternion.Euler(currentPitch, currentYaw, 0f);
        transform.localRotation = Quaternion.RotateTowards(transform.localRotation, newRotation, Mathf.Deg2Rad * rotationSpeed);
    }
    private void RollShip()
    {
        // Get the current rotation of the ship model to adjust based on if the
        // bumpers are pressed.
        Vector3 currentRotation = currentShip.transform.localEulerAngles;

        // If the right bumper is pressed, go roll all the way to the right.
        if (bumperValue == 1)
        {
            currentShip.transform.localEulerAngles = new Vector3(currentRotation.x, currentRotation.y, Mathf.LerpAngle(currentRotation.z, 270, bumperRotateSpeed));
        }
        // If the left bumper is pressed, roll all the way to the left.
        else if (bumperValue == -1)
        {
            currentShip.transform.localEulerAngles = new Vector3(currentRotation.x, currentRotation.y, Mathf.LerpAngle(currentRotation.z, 90, bumperRotateSpeed));
        }
        // If no bumper is pressed, roll in the direction of movement.
        else
        {
            currentShip.transform.localEulerAngles = new Vector3(currentRotation.x, currentRotation.y, Mathf.LerpAngle(currentRotation.z, -moveHori * maxRoll, .2f));
        }
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
    private void PerformBumperAbility()
    {
        if (Input.GetButtonDown("Rotate"))
        {
            if (bumperValue == 0 && (shipData[shipDataIndex].runtimeShipEnergy >= shipData[shipDataIndex].BumperAbilityCost))
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

        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (enableMovement)
            {
                Debug.Log("Movement Disabled!");
                enableMovement = false;
            }
            else
            {
                Debug.Log("Movement Enabled!");
                enableMovement = true;
            }
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
        rigidbody.AddForce(transform.up * 30f, ForceMode.Impulse);
        yield return new WaitForSeconds(.09f);
        rigidbody.velocity = Vector3.zero;
        rigidbody.ResetInertiaTensor();
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