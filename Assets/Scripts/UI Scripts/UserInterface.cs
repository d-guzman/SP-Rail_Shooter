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
    private string htInit = "Health: ";
    private string etInit = "Energy: ";

    void Update()
    {
        for (int i = 0; i < selectedShips.Length; i++)
        {
            if (selectedShips[i].runtimeIsActive)
            {
                healthText.text = htInit + selectedShips[i].runtimeShipHealth;
                energyText.text = etInit + selectedShips[i].runtimeShipEnergy;
            }
        }
    }
}
