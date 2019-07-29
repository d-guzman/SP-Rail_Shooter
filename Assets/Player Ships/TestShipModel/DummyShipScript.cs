using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyShipScript : PlayerShip
{
    public override void UpdateWeapons(int weaponLevel)
    {
        switch (weaponLevel)
        {
            case 1:
                shootPoints[0].SetActive(false);
                shootPoints[1].SetActive(false);
                shootPoints[2].SetActive(true);

                shootPoints[0].transform.parent.gameObject.SetActive(false);
                shootPoints[1].transform.parent.gameObject.SetActive(false);
                shootPoints[2].transform.parent.gameObject.SetActive(true);
                break;
            default:
                shootPoints[0].SetActive(true);
                shootPoints[1].SetActive(true);
                shootPoints[2].SetActive(false);

                shootPoints[0].transform.parent.gameObject.SetActive(true);
                shootPoints[1].transform.parent.gameObject.SetActive(true);
                shootPoints[2].transform.parent.gameObject.SetActive(false);
                break;
        }
    }

    public override void ShootWeapons(int weaponLevel)
    {
        foreach (GameObject point in shootPoints)
        {
            if (point.activeInHierarchy == true)
            {
                GameObject tempBullet;
                switch (weaponLevel)
                {
                    case 1:
                        tempBullet = Instantiate(ShipData.BaseBullet, point.transform.position, point.transform.parent.rotation);
                        tempBullet.GetComponent<BulletScript>().bulletSetup(ShipData.ShootingSounds[0], 1f);
                        break;
                    case 2:
                        tempBullet = Instantiate(ShipData.BaseBullet, point.transform.position, point.transform.parent.rotation);
                        tempBullet.GetComponent<BulletScript>().bulletSetup(ShipData.ShootingSounds[1], .6f);
                        break;
                    case 3:
                        tempBullet = Instantiate(ShipData.MaxBullet, point.transform.position, point.transform.parent.rotation);
                        tempBullet.GetComponent<BulletScript>().bulletSetup(ShipData.ShootingSounds[2], .6f);
                        break;
                }

            }
        }
    }

    public override void ChargeAbility() { /* TBD */ }
    public override void BumperAbility() { /* TBD */ }
}
