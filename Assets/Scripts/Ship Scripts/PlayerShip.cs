using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerShip : MonoBehaviour
{
    public PlayerShipData ShipData;
    protected GameObject[] shootPoints;
    protected GameObject bombPoint;

    public abstract void UpdateWeapons();
    public abstract void ShootWeapons();
    public abstract void ChargeAbility();
    public abstract void BumperAbility();

    void Awake()
    {
        shootPoints = GameObject.FindGameObjectsWithTag("BulletSpawn_Player");
        bombPoint = GameObject.FindGameObjectWithTag("BombSpawn_Player");
        ShipData.ShootFunction.AddListener(ShootWeapons);
        ShipData.UpdateFunction.AddListener(UpdateWeapons);
        ShipData.ChargeFunction.AddListener(ChargeAbility);
        ShipData.BumperFunction.AddListener(BumperAbility);
        ShipData.BombFunction.AddListener(BombAbility);
        ShipData.runtimeIsActive = true;
    }

    // The reason for this function here is because firing a bomb is the same across
    // ALL ships. The only thing that could be different is the type of bomb that they fire
    // which can be made different (if desired) via the script that's attached to them.
    public void BombAbility()
    {
        Instantiate(ShipData.ShipBomb, bombPoint.transform.position, bombPoint.transform.rotation);
    }
}
