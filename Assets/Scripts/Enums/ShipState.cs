using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Flags]
public enum ShipState
{
    // Keep adding states as needed?
    None = 0,
    isBoosting = 1,              // 0000 0001
    isBraking = 2,               // 0000 0010
    isCharging = 4,              // 0000 0100
    BumperActive = 8,            // 0000 1000
    gotHit = 16,                  // 0001 0000
    gotItem = 32                  // 0010 0000
}
