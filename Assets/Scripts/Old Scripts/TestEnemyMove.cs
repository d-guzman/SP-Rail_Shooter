using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemyMove : MonoBehaviour {
    public Transform player;
    public Transform[] points;
    [Range(0f, .1f)]
    public float moveSpeed = .0075f;

    private float currentStep = 0f;

    void Update()
    {
        // this is an inefficient method of doing a bezier curve for movement, but whatever. currently too lazy to make a real bezier curve.
        currentStep += moveSpeed;
        Vector3 a = Vector3.Lerp(points[0].localPosition, points[1].localPosition, currentStep);
        //Debug.Log(a);
        Vector3 b = Vector3.Lerp(points[1].localPosition, points[2].localPosition, currentStep);
        //Debug.Log(b);
        Vector3 final = Vector3.Lerp(a, b, currentStep);
        //Debug.Log("Final Vector in World Space: " + transform.TransformPoint(final));

        transform.localPosition = final;
        transform.LookAt(player);

        if (currentStep >= 1.1f)
        {
            Destroy(this.gameObject);
        }
    }
}
