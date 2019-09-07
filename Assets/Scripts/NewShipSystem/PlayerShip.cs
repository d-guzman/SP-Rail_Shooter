using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerShip : MonoBehaviour
{
    public PlayerShipData ShipData;
    protected GameObject[] shootPoints;

    public abstract void UpdateWeapons(int weaponLevel);
    public abstract void ShootWeapons(int weaponLevel);
    public abstract void ChargeAbility();
    public abstract void BumperAbility();

    void Awake()
    {
        shootPoints = GameObject.FindGameObjectsWithTag("BulletSpawn_Player");
        ShipData.ShootFunction.AddListener(ShootWeapons);
        ShipData.UpdateFunction.AddListener(UpdateWeapons);
        ShipData.runtimeIsActive = true;
    }
}
