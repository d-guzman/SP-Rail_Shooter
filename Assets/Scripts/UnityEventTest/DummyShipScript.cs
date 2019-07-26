using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyShipScript : MonoBehaviour
{
    private List<Transform> shootPoints;
    public GameObject BaseBullet;
    public GameObject MaxBullet;

    [SerializeField]
    public void exampleFunction()
    {
        Debug.Log("A Unity Event in the PlayerShip ScriptableObject was invoked via the Player Controller, calling this method.");
    }
    public void exampleFunction2(int x)
    {
        Debug.Log("Look a number: " + x.ToString() + ". This was passed from UnityEventTest, through PlayerShip to testUnityEvent.");
        foreach (Transform point in shootPoints)
        {
            Debug.Log(point.root + " Position: " + point.position.ToString());
            GameObject temp = Instantiate(BaseBullet, point.position, Quaternion.Euler(90f, 0f, 0f));
        }
    }
}
