using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Flags]
public enum ShipState
{
    // Keep adding states as needed?
    isBoosting = 0x01,              // 0000 0001
    isBraking = 0x02,               // 0000 0010
    isCharging = 0x04,              // 0000 0100
    BumperActive = 0x08             // 0000 1000
}
