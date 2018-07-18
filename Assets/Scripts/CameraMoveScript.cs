using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMoveScript : MonoBehaviour {
    [Range(50f, 100f)]
    public float moveSpeed = 75f;

    void LateUpdate()
    {
        transform.position += transform.forward * moveSpeed * Time.deltaTime;
    }
}
