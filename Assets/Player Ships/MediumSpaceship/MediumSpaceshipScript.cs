using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MediumSpaceshipScript : PlayerShip
{
    private float rotationSpeed = 18f;

    public override void UpdateWeapons()
    {
        
        switch (ShipData.runtimeWeaponLevel)
        {
            case 1:
                shootPoints[0].SetActive(false);
                shootPoints[1].SetActive(false);
                shootPoints[2].SetActive(true);
                break;
            default:
                shootPoints[0].SetActive(true);
                shootPoints[1].SetActive(true);
                shootPoints[2].SetActive(false);
                break;
        }
        
    }
    public override void ShootWeapons()
    {
        foreach (GameObject point in shootPoints)
        {
            if (point.activeInHierarchy == true)
            {
                GameObject tempBullet;
                switch (ShipData.runtimeWeaponLevel)
                {
                    case 1:
                        tempBullet = Instantiate(ShipData.BaseBullet, point.transform.position, point.transform.rotation);
                        tempBullet.GetComponent<BulletScript>().bulletSetup(ShipData.ShootingSounds[0], 1f);
                        break;
                    case 2:
                        tempBullet = Instantiate(ShipData.BaseBullet, point.transform.position, point.transform.rotation);
                        tempBullet.GetComponent<BulletScript>().bulletSetup(ShipData.ShootingSounds[1], .6f);
                        break;
                    case 3:
                        tempBullet = Instantiate(ShipData.MaxBullet, point.transform.position, point.transform.rotation);
                        tempBullet.GetComponent<BulletScript>().bulletSetup(ShipData.ShootingSounds[2], .6f);
                        break;
                }

            }
        }
    }
    public override void ChargeAbility() { /* TBD */ }
    public override void BumperAbility()
    {
        Debug.Log("Bumper Ability Invoked! (Barrel Roll)");
        StartCoroutine(BarrelRoll());
    }

    IEnumerator BarrelRoll()
    {
        float test = transform.localRotation.z;
        for (int i = 0; i < 20; i++)
        {
            test += rotationSpeed;
            transform.localRotation = Quaternion.Euler(0f, 0f, test);
            yield return null;
        }
        transform.localRotation = Quaternion.identity;
    }
}
