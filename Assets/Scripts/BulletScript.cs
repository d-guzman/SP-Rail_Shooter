using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour {
    private Rigidbody bulletRB;
    public float bulletSpeed = 100f;
    public float duration = 2f;
    public int bulletDamage;

    private bool timerStarted = false;
    private bool hitLevel = false;

    void Start()
    {
        if (bulletRB == null)
            bulletRB = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (!hitLevel)
        {
            transform.position += (transform.up * bulletSpeed) * Time.deltaTime;
            //bulletRB.MovePosition(transform.position + (transform.up * bulletSpeed) * Time.deltaTime);
        }
        if (timerStarted == false)
        {
            StartCoroutine(destroySelf());
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Level")
        {
            hitLevel = true;
            gameObject.GetComponent<CapsuleCollider>().enabled = false;
            MeshRenderer[] childMeshes = gameObject.GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer mesh in childMeshes)
            {
                mesh.enabled = false;
            }
        }
    }

    IEnumerator destroySelf()
    {
        timerStarted = true;
        yield return new WaitForSeconds(duration);
        Destroy(this.gameObject);
    }
}
