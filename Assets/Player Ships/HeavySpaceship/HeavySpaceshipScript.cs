using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeavySpaceshipScript : PlayerShip
{
    public float timeBetweenShots = 1f;
    public float beamUptime = 1f;
    public float timeBetweenBeams = 1.5f;
    public float shieldDuration = 3f;
    public GameObject Shield;
    public GameObject LaserBeam;
    public LineRenderer LaserRenderer;
    public Material[] BeamMaterials;
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
                    StartCoroutine(BeamRoutine(beamUptime));
                    StartCoroutine(DelayAfterShooting(timeBetweenBeams));
                    LaserRenderer.material = BeamMaterials[0];
                    break;
                case 3:
                    StartCoroutine(BeamRoutine(beamUptime));
                    StartCoroutine(DelayAfterShooting(timeBetweenBeams));
                    LaserRenderer.material = BeamMaterials[1];
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

    IEnumerator BeamRoutine(float uptime)
    {
        LaserBeam.SetActive(true);
        yield return new WaitForSeconds(uptime);
        LaserBeam.SetActive(false);
    }
}
