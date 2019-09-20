using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Ship", menuName = "PlayerShip", order = 1)]
public class PlayerShipData : ScriptableObject, ISerializationCallbackReceiver
{
    // Each player ship needs one PlayerShipData object, a "Ship" script that
    // implements the PlayerShipInterface interface, and a prefab with said
    // "Ship" script attached to it.
    private bool isActive = false;
    [Header("Ship Data")]
    public string ShipName;
    public GameObject ShipModel;
    public int ShipHealth;
    public int ShipEnergy;
    public float ShipSpeed;
    public GameObject ShipBomb;
    public int ShipBombCount = 3;
    private ShipStatus ShipStatus = ShipStatus.NoDamage;
    private int weaponLevel = 1;
    public int BumperAbilityCost;

    [System.NonSerialized]
    public int runtimeShipHealth;
    [System.NonSerialized]
    public int runtimeShipEnergy;
    [System.NonSerialized]
    public ShipStatus runtimeShipStatus;
    [System.NonSerialized]
    public int runtimeWeaponLevel = 1;
    [System.NonSerialized]
    public bool runtimeIsActive;
    [System.NonSerialized]
    public int runtimeShipBombCount;

    public void OnBeforeSerialize() { }
    public void OnAfterDeserialize()
    {
        runtimeShipHealth = ShipHealth;
        runtimeShipEnergy = ShipEnergy;
        runtimeIsActive = isActive;
        runtimeShipStatus = ShipStatus;
        runtimeWeaponLevel = weaponLevel;
        runtimeShipBombCount = ShipBombCount;
    }

    [Header("Bullet Data")]
    // The Base and Max Bullets are holdovers from the previous Ship System.
    // Replace with a list if so desired.
    public GameObject BaseBullet;
    public GameObject MaxBullet;
    public List<AudioClip> ShootingSounds;

    [HideInInspector()]
    public UnityEvent UpdateFunction;
    [HideInInspector()]
    public UnityEvent ShootFunction;
    [HideInInspector()]
    public UnityEvent ChargeFunction;
    [HideInInspector()]
    public UnityEvent BumperFunction;
    [HideInInspector()]
    public UnityEvent BombFunction;

    public void EventCleanup()
    {
        runtimeIsActive = false;
        ShootFunction.RemoveAllListeners();
        UpdateFunction.RemoveAllListeners();
        ChargeFunction.RemoveAllListeners();
        BumperFunction.RemoveAllListeners();
        BombFunction.RemoveAllListeners();
    }
}
