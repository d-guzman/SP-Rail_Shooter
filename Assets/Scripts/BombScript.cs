using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombScript : MonoBehaviour
{
    public float moveSpeed = 100f;
    public float fuseTime = 2f;
    public GameObject explosion;

    void Awake()
    {
        StartCoroutine(startFuse());
    }

    void Update()
    {
        transform.position += (transform.forward * moveSpeed * Time.deltaTime);
    }

    // Coroutines
    IEnumerator startFuse()
    {
        yield return new WaitForSeconds(fuseTime);
        Instantiate(explosion, transform.position, transform.rotation);
        Destroy(this.gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Ground" || other.tag == "Level")
        {
            StopCoroutine(startFuse());
            Instantiate(explosion, transform.position, transform.rotation);
            Destroy(this.gameObject);
        }
    }
}
