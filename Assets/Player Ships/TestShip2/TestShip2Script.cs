using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestShip2Script : MonoBehaviour, PlayerShipInterface
{
    public PlayerShipData shipData;
    private GameObject[] shootPoints;

    void Awake()
    {
        shootPoints = GameObject.FindGameObjectsWithTag("BulletSpawn_Player");
    }

    public void UpdateWeapons(int weaponLevel)
    {
        // THIS IS INENTIONALLY LEFT BLANK.
        // This ship always has 2 guns. The only thing that changes with weapon level is
        // the type of shot and the pattern.
    }

    public void ShootWeapons(int weaponLevel)
    {
        foreach (GameObject point in shootPoints)
        {
            GameObject tempBullet = null;
            IEnumerator shootSubroutine;
            switch (weaponLevel)
            {
                case 1:
                    tempBullet = Instantiate(shipData.BaseBullet, point.transform.position, point.transform.parent.rotation);
                    tempBullet.GetComponent<BulletScript>().bulletSetup(shipData.ShootingSounds[0], .5f);
                    break;
                case 2:
                    shootSubroutine = ThreeShotPattern(tempBullet, shipData.BaseBullet, point, shipData.ShootingSounds[0]);
                    StartCoroutine(shootSubroutine);
                    break;
                case 3:
                    shootSubroutine = ThreeShotPattern(tempBullet, shipData.MaxBullet, point, shipData.ShootingSounds[1]);
                    StartCoroutine(shootSubroutine);
                    break;
            }
        }
    }

    public void ChargeAbility() { }
    public void BumperAbility() { }

    // Coroutines!
    IEnumerator ThreeShotPattern(GameObject tempBullet, GameObject bulletType, GameObject point, AudioClip sound)
    {
        for (int i = 0; i < 3; i++)
        {
            tempBullet = Instantiate(bulletType, point.transform.position, point.transform.parent.rotation);
            tempBullet.GetComponent<BulletScript>().bulletSetup(sound, .5f);
            yield return new WaitForSeconds(.1f);
        }
    }
}
