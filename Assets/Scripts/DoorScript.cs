using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour {
    public bool hitByPlayer;
    private float rotateOffset;
    private float originalRotation;

    // A Door that closes after a bit.
    public bool isTimedDoor = false;
    public float openForSeconds = 1.5f;
    private bool isOpen = false;

    void Start()
    {
        originalRotation = transform.rotation.eulerAngles.y;
        if (transform.rotation.eulerAngles.y >= 180f)
            rotateOffset = transform.rotation.eulerAngles.y - 90;
        else
            rotateOffset = transform.rotation.eulerAngles.y + 90;
    }

    void Update()
    {
        if (hitByPlayer)
        {
            if (!isTimedDoor)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0f, rotateOffset, 0f), .07f);
            }
            else if (isTimedDoor)
            {
                if (!isOpen)
                    StartCoroutine(cycle_isOpen());
                else
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0f, rotateOffset, 0f), .07f);
            }
        }
        else
        {
            if (isTimedDoor)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0f, originalRotation, 0f), .07f);
            }
        }
    }

    //Coroutines
    IEnumerator cycle_isOpen()
    {
        isOpen = true;
        yield return new WaitForSeconds(openForSeconds);
        isOpen = false;
        hitByPlayer = false;
    }
}
