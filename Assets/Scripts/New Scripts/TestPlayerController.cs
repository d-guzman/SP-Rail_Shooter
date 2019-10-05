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

    // Variables for rotating ship when pressing bumpers.
    private Vector3 maxLRotation = new Vector3(0f, 0f, 90f);
    private Vector3 maxRRotation = new Vector3(0f, 0f, 270f);
    private float bumperRotateSpeed = .2f;

    // Variables for movement.
    [Tooltip("How large the buffers on the edge of the screen are (so the player ship doesn't go offscreen), in terms of percent.")]
    public float screenEdgeBufferSize = .05f;
    private Camera mainCam;
    private float moveHori;
    private float moveVert;
    private float bumperValue;
    private float screenWidthBuffer;
    private float screenHeightBuffer;

    [Header("Rotation Values")]
    public float rotationSpeed = 5f;
    [Tooltip("How much the player ship will rotate around the Y-Axis")]
    public float maxYaw = 20f;
    [Tooltip("How much the player ship will rotate around the Z-Axis")]
    public float maxPitch = 20f;

    // Variables used when entering triggers/collisions.
    [Header("Misc. Variables for Things")]
    public Rigidbody rb;
    public CameraMovement camScript;
    public int framesToHoldForCharge = 12;
    private int framesShootHeld = 0;

    [Header("Debug-Related")]
    public bool useOldMovement = false;
    public bool useRigidbodyForceMovement = false;
    public bool useRigidbodyTranslationMovement = false;

    void Start()
    {
        mainCam = Camera.main;
        zPosition = zPositionDefault;
        screenWidthBuffer = (float)mainCam.pixelWidth * screenEdgeBufferSize;
        screenHeightBuffer = (float)mainCam.pixelHeight * screenEdgeBufferSize;
        SetupShip();
    }

    void Update()
    {
        CheckInput_Movement();
        CheckInput_Bumpers();
        CheckInput_Boost();
        CheckInput_Brake();

        ManageEnergy();

        PerformBumperAbility();
        ShootBomb();

        if(useOldMovement)
            MoveShip();

        DebugKeyInputTests();
    }

    void FixedUpdate()
    {
        if(useRigidbodyForceMovement)
            TranslateShipForces();
        if (useRigidbodyTranslationMovement)
            TranslateShipMovePosition();
        AimShip();
        RollShip();
        ShootWeapons();                             // This is in FixedUpdate because I want a frame-rate independent(?) counting of frames.
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

    private void TranslateShipForces()
    {
        // Make movement vector by taking the right and up vectors of the main
        // camera, multiplying them by input values, and adding them together.
        Vector3 movement = (mainCam.transform.right * moveHori) + (mainCam.transform.up * moveVert);

        // If the player ship gets too close to the camera's edge, don't apply force
        // in the direction of that edge.
        Vector3 screenPosition = mainCam.WorldToScreenPoint(transform.position);
        if ((screenPosition.x < screenWidthBuffer && moveHori < 0f) || (screenPosition.x > (mainCam.pixelWidth - screenWidthBuffer) && moveHori > 0f))
            movement -= (mainCam.transform.right * moveHori);
        if ((screenPosition.y < screenHeightBuffer && moveVert < 0f) || (screenPosition.y > (mainCam.pixelHeight - screenHeightBuffer) && moveVert > 0f))
            movement -= (mainCam.transform.up * moveVert);

        // Normalize the movement vector.
        if (movement.magnitude > 1)
            movement.Normalize();

        // If boosting, add a new vector (representing boosting) to movement.
        // If braking, subtract a new vector (representing braking) to movement.
        bool isBoosting = shipData[shipDataIndex].CheckStateFlag<ShipState>(shipData[shipDataIndex].runtimeShipState, ShipState.isBoosting);
        bool isBraking = shipData[shipDataIndex].CheckStateFlag<ShipState>(shipData[shipDataIndex].runtimeShipState, ShipState.isBraking);
        if (isBoosting && (transform.localPosition.z < zPositionBoost))
        {
            movement += (mainCam.transform.forward * zPositionAcceleration);
        }
        else if (isBraking && (transform.localPosition.z > zPositionBrake))
        {
            movement -= (mainCam.transform.forward * zPositionAcceleration);
        }
        else if (!isBoosting && !isBraking && (transform.localPosition.z > (zPositionDefault + .05f) || transform.localPosition.z < (zPositionDefault - .05f)))
        {
            float zPositionDifference = (transform.localPosition.z - zPositionDefault);
            if (zPositionDifference > .05f || zPositionDifference < -.05f)
            {
                movement -= (mainCam.transform.forward * (zPositionAcceleration * Mathf.Clamp(zPositionDifference, -1f, 1f)));
            }
        }

        // If we are at the edges of the screen, immediately subtract the
        // respective velocity. This hasn't accounted for boosting/braking.
        if ((screenPosition.x < screenWidthBuffer) || (screenPosition.x > (mainCam.pixelWidth - screenWidthBuffer)))
        {
            //Debug.Log("Horizontal Screen-edge");
            rb.velocity -= new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        }
        if ((screenPosition.y < screenHeightBuffer) || (screenPosition.y > (mainCam.pixelHeight - screenHeightBuffer)))
        {
            //Debug.Log("Vertical Screen-edge");
            rb.velocity -= new Vector3(0f, rb.velocity.y, 0f);
        }

        // Add force to the player ship in the form of velocity change. The
        // reasoning behind this is because (for now) i want the ships to move in
        // a mostly similar fashion.
        rb.AddForce(movement, ForceMode.VelocityChange);

        // Clamp the velocity to the top speed of the ship. If the player is rotating the ship,
        // the magnitude of velocity will be clamped more or less depending on the direction
        // the player is moving.
        if ((bumperValue == -1f && moveHori < 0f) || (bumperValue == 1f && moveHori > 0f))
        {
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, shipData[shipDataIndex].ShipSpeed * 1.33f);
        }
        else if ((bumperValue == -1f && moveHori > 0f) || (bumperValue == 1f && moveHori < 0f))
        {
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, shipData[shipDataIndex].ShipSpeed * .66f);
        }
        else if (bumperValue == 0f)
        {
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, shipData[shipDataIndex].ShipSpeed);
        }

        // If the magnitude of velocity is close to zero, set it to zero.
        if (rb.velocity.magnitude < .05f)
            rb.velocity = Vector3.zero;
    }
    private void TranslateShipMovePosition()
    {
        // Create the movement vector from the player's input and normalize it.
        Vector3 movement = (mainCam.transform.right * moveHori + mainCam.transform.up * moveVert);


        // Change horizontal move speed when rotating ship; Move with rotation = faster, Move against rotation = slower.
        if (moveHori != 0f)
            movement += mainCam.transform.right * (bumperValue * .33f);

        // IF BOOSTING - Move the player ship farther away from the camera.
        // IF BRAKING - Move the player ship closer to the camera.
        bool isBoosting = shipData[shipDataIndex].CheckStateFlag<ShipState>(shipData[shipDataIndex].runtimeShipState, ShipState.isBoosting);
        bool isBraking = shipData[shipDataIndex].CheckStateFlag<ShipState>(shipData[shipDataIndex].runtimeShipState, ShipState.isBraking);
        if (isBoosting && transform.localPosition.z < zPositionBoost)
        {
            movement += mainCam.transform.forward * zPositionAcceleration;
        }
        else if (isBraking && transform.localPosition.z > zPositionBrake)
        {
            movement -= mainCam.transform.forward * zPositionAcceleration;
        }
        else if (!isBoosting && !isBraking)
        {
            if (transform.localPosition.z < zPositionDefault - .05f || transform.localPosition.z > zPositionDefault + .05f)
            {
                Debug.Log("Not at default position! adjust player ship until it returns to default z position!");
                movement -= mainCam.transform.forward * (zPositionAcceleration * Mathf.Clamp((transform.localPosition.z - zPositionDefault), -1f, 1f));
            }
        }
        if (movement.magnitude > 1f)
            movement.Normalize();

        // Take the final movement vector and multiply it by ShipSpeed and FixedDeltaTime,
        // add it to the dolly's position in world space, and clamp the X and Y values
        // so the ship stays in the camera's view at all times.
        if (movement.magnitude != 0f)
        {
            Debug.Log("Moving!");
            Vector3 nextpostion = transform.position + movement * shipData[shipDataIndex].ShipSpeed * Time.deltaTime;
            nextpostion = mainCam.WorldToViewportPoint(nextpostion);
            nextpostion.x = Mathf.Clamp(nextpostion.x, .1f, .9f);
            nextpostion.y = Mathf.Clamp(nextpostion.y, .1f, .9f);

            // Convert nextposition from Viewport Space Back into World Space.
            nextpostion = mainCam.ViewportToWorldPoint(nextpostion);

            // Move the player ship to its new position.
            rb.MovePosition(nextpostion);
        }
    } 

    private void AimShip()
    {
        float currentYaw = maxYaw * moveHori;
        float currentPitch = maxPitch * -moveVert;

        Quaternion newRotation = Quaternion.Euler(currentPitch, currentYaw, 0f);
        transform.localRotation = Quaternion.RotateTowards(transform.localRotation, newRotation, rotationSpeed);
    }
    private void RollShip()
    {

        // Rotate the ship model if the bumpers are pressed.
        if (bumperValue == 1)
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
        else if (bumperValue == -1)
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
            // Enable Old Movement code.
            Debug.Log("Using Transform Movement!");
            useOldMovement = true;
            useRigidbodyForceMovement = false;
            useRigidbodyTranslationMovement = false;
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            // Enable Rigidbody.AddForce Movement code.
            Debug.Log("Using AddForce Movement!");
            useOldMovement = false;
            useRigidbodyForceMovement = true;
            useRigidbodyTranslationMovement = false;
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("Using MovePosition Movement!");
            useOldMovement = false;
            useRigidbodyForceMovement = false;
            useRigidbodyTranslationMovement = true;
        }
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