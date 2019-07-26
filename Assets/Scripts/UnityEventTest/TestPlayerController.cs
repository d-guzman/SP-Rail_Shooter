using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TestPlayerController : MonoBehaviour
{
    public PlayerShipData shipData;
    public int WeaponLevel = 1;

    // Start is called before the first frame update
    void Start()
    {
        Instantiate(shipData.ShipModel, transform, false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            shipData.ShootFunction.Invoke(WeaponLevel);
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            WeaponLevel = 1;
            Debug.Log("Ship's weapon level set to: " + WeaponLevel.ToString());
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            WeaponLevel = 2;
            Debug.Log("Ship's weapon level set to: " + WeaponLevel.ToString());
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            WeaponLevel = 3;
            Debug.Log("Ship's weapon level set to: " + WeaponLevel.ToString());
        }
    }
}
