using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotateCame : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y + .2f, 0f);
    }
}
