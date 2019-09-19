using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionScript : MonoBehaviour
{
    public float scaleOfExplosion = 5f;
    public float scaleRate = 10f;

    private Vector3 scaleVector;

    void Start()
    {
        scaleVector = new Vector3(scaleOfExplosion, scaleOfExplosion, scaleOfExplosion);
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, scaleVector, scaleRate * Time.deltaTime);
        if (transform.localScale.magnitude >= (scaleVector.magnitude - .5f))
            Destroy(this.gameObject);
    }
}
