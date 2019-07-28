using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface PlayerShipInterface
{
    // This interface holds all of the methods that each player ship must define.
    // This means that each playable ship will need it's own script, prefab, and
    // PlayerShipData in order to be playable in game!
    void UpdateWeapons(int weaponLevel);
    void ShootWeapons(int weaponLevel);
    void ChargeAbility();
    void BumperAbility();
}
