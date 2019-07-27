using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TestPlayerController : MonoBehaviour
{
    public PlayerShipData shipData;
    public int WeaponLevel = 1;

    void Start()
    {
        // MUST ADD THE FUNCTIONS OF NEWLY INSTANTIATED OBJECT AS A LISTENER TO THE
        // RESPECTIVE EVENTS IN THE SCRIPTABLE OBJECT!
        // Is this the best way to do this? Probably not. Instead the script should
        // probably do this instead. If doing that would work, delete this code!
        GameObject temp = Instantiate(shipData.ShipModel, transform, false);
        PlayerShipInterface tempInterface = temp.GetComponent<PlayerShipInterface>();
        shipData.ShootFunction.AddListener(tempInterface.ShootWeapons);
        shipData.UpdateFunction.AddListener(tempInterface.UpdateWeapons);
        shipData.UpdateFunction.Invoke(WeaponLevel);
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
            shipData.UpdateFunction.Invoke(WeaponLevel);
            Debug.Log("Ship's weapon level set to: " + WeaponLevel.ToString());
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            WeaponLevel = 2;
            shipData.UpdateFunction.Invoke(WeaponLevel);
            Debug.Log("Ship's weapon level set to: " + WeaponLevel.ToString());
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            WeaponLevel = 3;
            shipData.UpdateFunction.Invoke(WeaponLevel);
            Debug.Log("Ship's weapon level set to: " + WeaponLevel.ToString());
        }
    }
}
