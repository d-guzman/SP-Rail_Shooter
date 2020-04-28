using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReticleScript : MonoBehaviour
{
    public Transform reticleCamera;

    void LateUpdate()
    {
        transform.LookAt(transform.position + reticleCamera.forward);
    }
}
