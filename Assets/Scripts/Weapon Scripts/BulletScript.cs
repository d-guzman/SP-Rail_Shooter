using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class BulletScript : MonoBehaviour {
    [Header("Important Components")]
    public Rigidbody bulletRB;
    public AudioSource soundSource;

    [Header("Bullet Data")]
    public float bulletSpeed = 100f;
    public float duration = 2f;
    public int bulletDamage;

    private bool timerStarted = false;
    private bool hitLevel = false;

    // Unity Functions
    void Awake()
    {
        if (bulletRB == null)
            bulletRB = GetComponent<Rigidbody>();

        if (soundSource == null)
            soundSource = GetComponent<AudioSource>();
    }

    void FixedUpdate()
    {
        if (!hitLevel)
            bulletRB.MovePosition(transform.position + (transform.up * bulletSpeed) * Time.deltaTime);

        if (timerStarted == false)
            StartCoroutine(destroySelf());
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Level")
            onCollide();

        else if (other.gameObject.tag == "EnemyShip")
        {
            onCollide();
            Destroy(other.transform.parent.gameObject);
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
        // This version is kept for compatibility with the old ship system!
        // DO NOT DELETE UNTIL THE NEW SYSTEM IS IMPLEMENTED!
        bulletDamage = damage;
        soundSource.clip = shootSound;
        soundSource.volume = soundVolume;
        soundSource.Play();
    }

    public void bulletSetup(AudioClip shootSound, float soundVolume)
    {
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
            mesh.enabled = false;
    }

    // Coroutines
    IEnumerator destroySelf()
    {
        timerStarted = true;
        yield return new WaitForSeconds(duration);
        Destroy(this.gameObject);
    }
}
