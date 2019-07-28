using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class ShootEvent : UnityEvent<int> { };
[System.Serializable]
public class UpdateWeaponEvent : UnityEvent<int> { };

[CreateAssetMenu(fileName = "Ship", menuName = "PlayerShip", order = 1)]
public class PlayerShipData : ScriptableObject
{
    // Each player ship needs one PlayerShipData object, a "Ship" script that
    // implements the PlayerShipInterface interface, and a prefab with said
    // "Ship" script attached to it.
    [Header("Ship Data")]
    public GameObject ShipModel;
    public int ShipHealth;
    public int ShipEnergy;
    public float ShipSpeed;

    [Header("Bullet Data")]
    // The Base and Max Bullets are holdovers from the previous Ship System.
    // Replace with a list if so desired.
    public GameObject BaseBullet;
    public GameObject MaxBullet;
    public List<AudioClip> ShootingSounds;

    //[Header("Ship-Specific Functions")]
    [HideInInspector()]
    public UpdateWeaponEvent UpdateFunction;
    [HideInInspector()]
    public ShootEvent ShootFunction;
    [HideInInspector()]
    public UnityEvent ChargeFunction;
    [HideInInspector()]
    public UnityEvent BumperFunction;

    public void EventCleanup()
    {
        ShootFunction.RemoveAllListeners();
        UpdateFunction.RemoveAllListeners();
        ChargeFunction.RemoveAllListeners();
        BumperFunction.RemoveAllListeners();
    }
}
