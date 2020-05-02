using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeavySpaceshipScript : PlayerShip
{
    public float timeBetweenShots = 1f;
    public float shieldDuration = 3f;
    public GameObject Shield;
    private bool canShoot = true;
    private bool canShield = true;
    public override void UpdateWeapons()
    {
        // Leave Empty. Unneeded.
    }

    public override void ShootWeapons()
    {
        if (canShoot)
        {
            GameObject point = shootPoints[0];
            GameObject tempBullet = null;
            switch (ShipData.runtimeWeaponLevel)
            {
                case 1:
                    tempBullet = Instantiate(ShipData.BaseBullet, point.transform.position, point.transform.rotation);
                    tempBullet.GetComponent<BulletScript>().bulletSetup(ShipData.ShootingSounds[0], 1f);
                    StartCoroutine(DelayAfterShooting(timeBetweenShots));
                    break;
                case 2:
                    Debug.Log("Imagine a cool laser!");
                    break;
                case 3:
                    Debug.Log("Imagine an even cooler laser! That pierces ships too!");
                    break;
            }
        }
    }

    public override void ChargeAbility()
    {
        // TBD
    }

    public override void BumperAbility()
    {
        if (canShield)
        {
            StartCoroutine(ShieldAbility(shieldDuration));
        }
    }

    IEnumerator DelayAfterShooting(float timeDelay)
    {
        canShoot = false;
        yield return new WaitForSeconds(timeDelay);
        canShoot = true;
    }

    IEnumerator ShieldAbility(float shieldTime)
    {
        Shield.SetActive(true);
        canShield = false;
        yield return new WaitForSeconds(shieldTime);
        canShield = true;
        Shield.SetActive(false);
    }
}
