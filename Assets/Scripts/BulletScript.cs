using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class BulletScript : MonoBehaviour {
    private Rigidbody bulletRB;
    public float bulletSpeed = 100f;
    public float duration = 2f;
    public int bulletDamage;

    private bool timerStarted = false;
    private bool hitLevel = false;

    private AudioSource soundSource;

    // Unity Functions
    void Awake()
    {
        if (bulletRB == null)
        {
            bulletRB = GetComponent<Rigidbody>();
        }
        if (soundSource == null)
        {
            soundSource = GetComponent<AudioSource>();
        }
    }

    void Update()
    {
        if (!hitLevel)
        {
            transform.position += (transform.up * bulletSpeed) * Time.deltaTime;
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
            onCollide();
        }

        else if (other.gameObject.tag == "Interactable")
        {
            other.gameObject.GetComponent<DoorScript>().hitByPlayer = true;
            onCollide();
        }
    }

    // Public Functions
    public void bulletSetup(int damage, AudioClip shootSound, float soundVolume)
    {
        bulletDamage = damage;
        soundSource.clip = shootSound;
        soundSource.volume = soundVolume;
        soundSource.Play();
    }

    //Private Funtions
    private void onCollide()
    {
        hitLevel = true;
        gameObject.GetComponent<CapsuleCollider>().enabled = false;
        MeshRenderer[] childMeshes = gameObject.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer mesh in childMeshes)
        {
            mesh.enabled = false;
        }
    }
    // Coroutines
    IEnumerator destroySelf()
    {
        timerStarted = true;
        yield return new WaitForSeconds(duration);
        Destroy(this.gameObject);
    }
}
