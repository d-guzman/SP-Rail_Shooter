using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserInterface : MonoBehaviour
{
    [Header("Player Ship Data")]
    [Tooltip("These are the ships the player has chosen to play with.")]
    public PlayerShipData[] selectedShips;                                                          // need to get a reference to the current ship, which the Player Con. has.

    [Header("UI Element References")]                                                               // more convienent to set up in inspector, then make a prefab.
    public Text healthText;
    public Text energyText;
    public Text bombText;
    public Text ship1StatusText;
    public Text ship2StatusText;
    private string htInit = "Health: ";
    private string etInit = "Energy: ";
    private string btInit = "Bombs: ";
    private string s1Init = "Ship 1: ";
    private string s2Init = "Ship 2: ";




    void Update()
    {
        for (int i = 0; i < selectedShips.Length; i++)
        {
            // Update the Ship Status texts with the current status of each ship.
            if (i == 0)
            {
                if (selectedShips[i].runtimeShipStatus == ShipStatus.NoDamage)
                    ship1StatusText.text = s1Init + "Good";
                else if (selectedShips[i].runtimeShipStatus == ShipStatus.LightDamage)
                    ship1StatusText.text = s1Init + "Light";
                else if (selectedShips[i].runtimeShipStatus == ShipStatus.HeavyDamage)
                    ship1StatusText.text = s1Init + "Heavy";
                else if (selectedShips[i].runtimeShipStatus == ShipStatus.CriticalDamage)
                    ship1StatusText.text = s1Init + "Critical";
                else if (selectedShips[i].runtimeShipStatus == ShipStatus.Down)
                    ship1StatusText.text = s1Init + "Down";
            }
            else
            {
                if (selectedShips[i].runtimeShipStatus == ShipStatus.NoDamage)
                    ship2StatusText.text = s2Init + "Good";
                else if (selectedShips[i].runtimeShipStatus == ShipStatus.LightDamage)
                    ship2StatusText.text = s2Init + "Light";
                else if (selectedShips[i].runtimeShipStatus == ShipStatus.HeavyDamage)
                    ship2StatusText.text = s2Init + "Heavy";
                else if (selectedShips[i].runtimeShipStatus == ShipStatus.CriticalDamage)
                    ship2StatusText.text = s2Init + "Critical";
                else if (selectedShips[i].runtimeShipStatus == ShipStatus.Down)
                    ship2StatusText.text = s2Init + "Down";
            }
            
            // Update the Ship Health, Energy, and Bombs to be accurate to the ship that is currently active.
            if (selectedShips[i].runtimeIsActive)
            {
                healthText.text = htInit + selectedShips[i].runtimeShipHealth;
                energyText.text = etInit + selectedShips[i].runtimeShipEnergy;
                bombText.text = btInit + selectedShips[i].runtimeShipBombCount;
            }
        }
    }
}
