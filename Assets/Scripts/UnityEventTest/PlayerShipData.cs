using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class ShootEvent : UnityEvent<int> { };
[System.Serializable]
public class ChargeEvent : UnityEvent<int> { };

[CreateAssetMenu(fileName = "Ship", menuName = "PlayerShip", order = 1)]
public class PlayerShipData : ScriptableObject
{
    [Header("Ship Attributes")]
    public GameObject ShipModel;
    public GameObject BaseBullet;
    public GameObject MaxBullet;
    public int ShipHealth;
    public int ShipEnergy;
    public float ShipSpeed;

    [Header("Ship-Specific Functions")]    
    public ShootEvent ShootFunction;
    //public UnityEvent ChargeFunction;
}
