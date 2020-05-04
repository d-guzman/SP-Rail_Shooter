using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamScript : MonoBehaviour
{
    public LineRenderer lr;
    public LayerMask mask;
    public float length = 50f;

    void LateUpdate()
    {
        //lr.SetPosition(0, transform.position);
        RaycastHit hitInfo;
        if (Physics.SphereCast(transform.position, lr.endWidth / 2, transform.forward, out hitInfo, length, mask))
        {
            lr.SetPosition(1, new Vector3(0f, 0f, hitInfo.distance));
        }
        else
        {
            lr.SetPosition(1, new Vector3(0f, 0f, length));
        }
    }
}
