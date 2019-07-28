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

    void Start()
    {
        SetupShip();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
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
            Invoke("SetupShip", (1/60));    // need to delay creating a ship by ~1 frame otherwise we get nullref errors.
        }
    }

    // Private Functions
    private void SetupShip()
    {
        currentShip = Instantiate(shipData[shipDataIndex].ShipModel, transform, false);
        shipData[shipDataIndex].UpdateFunction.Invoke(WeaponLevel);
    }
}
