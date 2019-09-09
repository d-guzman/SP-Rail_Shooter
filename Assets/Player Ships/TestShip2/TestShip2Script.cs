using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestShip2Script : PlayerShip
{
    public override void UpdateWeapons() { }
    public override void ShootWeapons()
    {
        foreach (GameObject point in shootPoints)
        {
            GameObject tempBullet = null;
            IEnumerator shootSubroutine;
            switch (ShipData.runtimeWeaponLevel)
            {
                case 1:
                    tempBullet = Instantiate(ShipData.BaseBullet, point.transform.position, point.transform.parent.rotation);
                    tempBullet.GetComponent<BulletScript>().bulletSetup(ShipData.ShootingSounds[0], .5f);
                    break;
                case 2:
                    shootSubroutine = ThreeShotPattern(tempBullet, ShipData.BaseBullet, point, ShipData.ShootingSounds[0]);
                    StartCoroutine(shootSubroutine);
                    break;
                case 3:
                    shootSubroutine = ThreeShotPattern(tempBullet, ShipData.MaxBullet, point, ShipData.ShootingSounds[1]);
                    StartCoroutine(shootSubroutine);
                    break;
            }
        }
    }

    public override void ChargeAbility() { }
    public override void BumperAbility() { }

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
