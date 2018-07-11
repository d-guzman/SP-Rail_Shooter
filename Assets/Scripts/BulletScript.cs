using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour {
    public float bulletSpeed = 100f;
    public float duration = 2f;
    public int bulletDamage;

    private bool timerStarted = false;

    void Update()
    {
        transform.position += (transform.up * bulletSpeed) * Time.deltaTime;
        if (timerStarted == false) {
            StartCoroutine(destroySelf());
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        
    }

    IEnumerator destroySelf()
    {
        timerStarted = true;
        yield return new WaitForSeconds(duration);
        Destroy(this.gameObject);
    }
}
