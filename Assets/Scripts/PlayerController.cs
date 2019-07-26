using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    private Camera mainCam;
    public Transform childMesh;
    private Vector3 maxLRotation = new Vector3(0f, 0f, 90f);
    private Vector3 maxRRotation = new Vector3(0f, 0f, 270f);
    private float bumperRotateSpeed = .2f;

    [Header("Player Stats")]
    [Tooltip("How much damage the player's ship can take before they die.")]
    public int health = 100;
    [Range(10f, 15f)]
    [Tooltip("How fast the player's ship can move")]
    public float moveSpeed = 12.5f;
    private float moveHori;
    private float moveVert;
    [Range(10f, 20f)]
    public float lookAtDist = 10f;
    [Tooltip("How strong the player's bullets are.")]
    public int bulletDamage = 10;

    [Header("Weapon Related")]
    public Transform reticle;
    [Range(.1f, 1f)]
    [Tooltip("How fast the can player's ship can fire its weapons.")]
    public float fireRate = .3f;
    private bool canFire = true;
    [Range(1,3)]
    [Tooltip("The current level of the player's weapons. Higher level means more guns.")]    
    public int weaponLevel = 1;
    private bool reachedMax;
    public GameObject basicLaserPrefab;
    public GameObject maxLaserPrefab;
    // UNITY 2018: Index 2: Level 1 Gun. Index 0 & 1: Level 2+ Guns
    // UNITY 2019: Index 0: Level 1 Gun. Index 1 & 2: Level 2+ Guns.
    public GameObject[] bulletSpawnLocations;
    // index 0: basic | index 1: twin | index 2: max
    public AudioClip[] shootingSounds;

    // Unity Functions
    void Start() {
        mainCam = Camera.main;
        //childMesh = transform.GetComponentInChildren<Transform>();
        if (bulletSpawnLocations.Length == 0) {
            bulletSpawnLocations = GameObject.FindGameObjectsWithTag("BulletSpawn_Player");
            if (weaponLevel == 1)
            {
                bulletSpawnLocations[2].SetActive(false);
                bulletSpawnLocations[1].SetActive(false);
                bulletSpawnLocations[2].transform.parent.gameObject.SetActive(false);
                bulletSpawnLocations[1].transform.parent.gameObject.SetActive(false);
            }
            else if (weaponLevel == 2 || weaponLevel == 3)
            {
                bulletSpawnLocations[0].SetActive(false);
                bulletSpawnLocations[0].transform.parent.gameObject.SetActive(false);
            }
        }
    }

    void Update()
    {
        //movePlayer_NotChild();
        checkWeapons();
        movePlayer_AsChild();
        shoot();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "WeaponUpgrade")
        {
            if (weaponLevel != 3)
                weaponLevel++;
            Destroy(other.gameObject);
        }

        if (other.tag == "EnemyLoad")
        {
            other.GetComponent<EnemyLoadingScript>().LoadEnemies();
        }
    }

    ////Private Functions
    //// This function works for moving the player around without making them a child of the main camera, but that comes with some visual quirks.
    //private void movePlayer_NotChild() {
    //    moveHori = Input.GetAxis("Horizontal");
    //    moveVert = Input.GetAxis("Vertical");

    //    // Code to keep ship moving correctly within the camera viewport.
    //    Vector3 camUp = mainCam.transform.up;
    //    Vector3 camRight = mainCam.transform.right;
    //    camUp.x = 0f;
    //    camRight.y = 0f;
    //    Vector3 nextPos = transform.position + (camRight * moveHori + camUp * moveVert).normalized;

    //    // Viewport Clamping code - Keeps the ship in the camera at all times.
    //    Vector3 viewportPos = mainCam.WorldToViewportPoint(nextPos);
    //    float viewX = Mathf.Clamp(viewportPos.x, .1f, .9f);
    //    float viewY = Mathf.Clamp(viewportPos.y, .1f, .9f);

    //    nextPos = mainCam.ViewportToWorldPoint(new Vector3(viewX, viewY, 10f));
    //    Vector3 lerpedPos = Vector3.Lerp(transform.position, nextPos, moveSpeed);

    //    transform.rotation = mainCam.transform.rotation;
    //    transform.position = lerpedPos;
    //}

    // haven't figured out how to rotate the ship with this, which makes the design of the game a bit more difficult.
    private void movePlayer_AsChild()
    {
        moveHori = Input.GetAxis("Horizontal");
        moveVert = Input.GetAxis("Vertical");
        float rotateValue = Input.GetAxis("Rotate");

        Vector3 movement = new Vector3(moveHori, moveVert);
        reticle.localPosition = new Vector3(0f, 0f, 35f) + movement;

        if (movement.magnitude > 1)
            movement.Normalize();

        if (moveHori != 0f)
            movement += new Vector3(-rotateValue/3.5f, 0f, 0f);          // If the player is rotating, they move faster in the direction they are rotating towards.

        Vector3 nextPos = transform.localPosition + movement;

        // Viewport Clamping code - Keeps the ship in the camera at all times.
        Vector3 viewportPos = mainCam.WorldToViewportPoint(transform.TransformPoint(nextPos));
        float viewX = Mathf.Clamp(viewportPos.x, 0.0f, 1f);
        float viewY = Mathf.Clamp(viewportPos.y, 0.0f, 1f);

        // ROTATION CODE
        // First, we check if the player is pressing any of the rotate buttons.
        // If they are, rotate in that direction. Otherwise, the ship levels out.
        // Second, we rotate the player ship to look towards the direction the
        // player is moving in.
        if (rotateValue == -1)
        {
            if (childMesh.localRotation.eulerAngles.z > 270.5f)
            {
                Vector3 meshRotation = Vector3.Lerp(childMesh.localRotation.eulerAngles, maxRRotation, bumperRotateSpeed);
                childMesh.localRotation = Quaternion.Euler(meshRotation);
            }
            else if (childMesh.localRotation.eulerAngles.z < 90.5f && childMesh.localRotation.eulerAngles.z > 20f)
            {
                Vector3 meshRotation = Vector3.Lerp(childMesh.localRotation.eulerAngles, Vector3.zero, bumperRotateSpeed);
                childMesh.localRotation = Quaternion.Euler(meshRotation);
            }
            else if (childMesh.localRotation.eulerAngles.z <= 20f)
                childMesh.localRotation = Quaternion.Euler(0f, 0f, 358.5f);
        }
        else if (rotateValue == 1)
        {
            if (childMesh.localEulerAngles.z < 90.5f)
            {
                Vector3 meshRotation = Vector3.Lerp(childMesh.localRotation.eulerAngles, maxLRotation, bumperRotateSpeed);
                childMesh.localRotation = Quaternion.Euler(meshRotation);
            }
            else if (childMesh.localEulerAngles.z > 269.5f && childMesh.localEulerAngles.z < 340f)
            {
                Vector3 meshRotation = Vector3.Lerp(childMesh.localRotation.eulerAngles, new Vector3(0f, 0f, 360f), bumperRotateSpeed);
                childMesh.localRotation = Quaternion.Euler(meshRotation);
            }
            else if (childMesh.localEulerAngles.z >= 340f)
                childMesh.localRotation = Quaternion.Euler(0f, 0f, 1.5f);
        }
        else
        {
            if (childMesh.localEulerAngles.z > 269.5f)
            {
                Vector3 meshRotation = Vector3.Lerp(childMesh.localRotation.eulerAngles, new Vector3(0f, 0f, 360f), bumperRotateSpeed);
                childMesh.localRotation = Quaternion.Euler(meshRotation);
                if (childMesh.localEulerAngles.z > 359.5f)
                    childMesh.localRotation = Quaternion.Euler(Vector3.zero);
            }
            else if (childMesh.localEulerAngles.z < 90.5f)
            {
                Vector3 meshRotation = Vector3.Lerp(childMesh.localRotation.eulerAngles, Vector3.zero, bumperRotateSpeed);
                childMesh.localRotation = Quaternion.Euler(meshRotation);
            }
        }

        Vector3 lookAtPoint = mainCam.ViewportToWorldPoint(new Vector3(viewX, viewY, 10f)) + (mainCam.transform.right * moveHori + mainCam.transform.up * moveVert) + mainCam.transform.forward * lookAtDist;
        //Vector3 reticlePoint = mainCam.ViewportToWorldPoint(new Vector3(viewX, viewY, 10f)) + (mainCam.transform.right * moveHori * 9 + mainCam.transform.up * moveVert * 9) + mainCam.transform.forward * (lookAtDist * 2f);
        //reticle.position = reticlePoint;
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
        Vector3 lerpedPos = Vector3.Lerp(transform.localPosition, nextPos, moveSpeed * Time.deltaTime);
        lerpedPos.Set(lerpedPos.x, lerpedPos.y, 10f);
        transform.localPosition = lerpedPos;
    }

    private void shoot() {
        if (Input.GetButtonDown("Shoot") && canFire)
        {
            GameObject[] bulletRefs = new GameObject[2];
            switch (weaponLevel)
            {
                case 1:
                    StartCoroutine(shootWait());
                    bulletRefs[0] = Instantiate(basicLaserPrefab, bulletSpawnLocations[0].transform.position, bulletSpawnLocations[0].transform.parent.rotation);
                    bulletRefs[0].GetComponent<BulletScript>().bulletSetup(bulletDamage, shootingSounds[0], 1f);
                    break;
                case 2:
                    StartCoroutine(shootWait());
                    bulletRefs[0] = Instantiate(basicLaserPrefab, bulletSpawnLocations[2].transform.position, bulletSpawnLocations[2].transform.parent.rotation);
                    bulletRefs[1] = Instantiate(basicLaserPrefab, bulletSpawnLocations[1].transform.position, bulletSpawnLocations[1].transform.parent.rotation);

                    bulletRefs[0].GetComponent<BulletScript>().bulletSetup(bulletDamage, shootingSounds[1], .6f);
                    bulletRefs[1].GetComponent<BulletScript>().bulletSetup(bulletDamage, shootingSounds[1], .6f);
                    break;
                case 3:
                    StartCoroutine(shootWait());
                    bulletRefs[0] = Instantiate(maxLaserPrefab, bulletSpawnLocations[2].transform.position, bulletSpawnLocations[2].transform.parent.rotation);
                    bulletRefs[1] = Instantiate(maxLaserPrefab, bulletSpawnLocations[1].transform.position, bulletSpawnLocations[1].transform.parent.rotation);

                    bulletRefs[0].GetComponent<BulletScript>().bulletSetup(bulletDamage, shootingSounds[2], .6f);
                    bulletRefs[1].GetComponent<BulletScript>().bulletSetup(bulletDamage, shootingSounds[2], .6f);
                    break;
            }
        }
    }

    private void checkWeapons() {
        if (weaponLevel == 1)
        {
            if (bulletSpawnLocations[0].activeSelf == false && bulletSpawnLocations[0].transform.parent.gameObject.activeSelf == false)
            {
                if (bulletDamage != 10)
                    bulletDamage = 10;
                bulletSpawnLocations[2].SetActive(false);
                bulletSpawnLocations[2].transform.parent.gameObject.SetActive(false);

                bulletSpawnLocations[1].SetActive(false);
                bulletSpawnLocations[1].transform.parent.gameObject.SetActive(false);

                bulletSpawnLocations[0].SetActive(true);
                bulletSpawnLocations[0].transform.parent.gameObject.SetActive(true);
            }
            // Safety code. ERASE WHEN WEAPON UPGRADES ARE PROPERLY IMPLEMENTED.
            if (reachedMax)
            {
                bulletDamage = 10;
                reachedMax = false;
            }
        }
        else if (weaponLevel == 2)
        {
            if (bulletSpawnLocations[0].activeSelf == true && bulletSpawnLocations[0].transform.parent.gameObject.activeSelf == true)
            {
                bulletSpawnLocations[2].SetActive(true);
                bulletSpawnLocations[2].transform.parent.gameObject.SetActive(true);

                bulletSpawnLocations[1].SetActive(true);
                bulletSpawnLocations[1].transform.parent.gameObject.SetActive(true);

                bulletSpawnLocations[0].SetActive(false);
                bulletSpawnLocations[0].transform.parent.gameObject.SetActive(false);
            }

            // Safety code. ERASE WHEN WEAPON UPGRADES ARE PROPERLY IMPLEMENTED.
            if (reachedMax)
            {
                bulletDamage = 10;
                reachedMax = false;
            }
        }

        // Safety code. ERASE WHEN WEAPON UPGRADES ARE PROPERLY IMPLEMENTED.
        else if (weaponLevel == 3)
        {
            if (!reachedMax)
            {
                reachedMax = true;
                bulletDamage = 15;
            }
        }
    }

    // Coroutines!
    IEnumerator shootWait() {
        canFire = false;
        yield return new WaitForSeconds(fireRate);
        canFire = true;
    }
    
}
