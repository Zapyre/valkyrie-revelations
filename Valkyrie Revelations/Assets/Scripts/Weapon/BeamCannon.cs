using UnityEngine;
using System.Collections;

public class BeamCannon : ChargeCannon
{
    public BeamCannon()
    {
        name = "Beam Cannon";
        weaponTex = null;
        weaponType = WeaponType.CHARGINGCANNON;
        damage = 2;
        multiplier = 5f;
        coolDown = 1f;
        maxAmmoInClip = 50;
        ammoInClip = maxAmmoInClip;
        totalAmmo = 300;
        maxCharge = 5;
        chargeTime = 1;
        chargeSpeed = 2;
        chargeObj = GameObject.Find("PlayerCharge");
        chargeParticle = chargeObj.GetComponent<ParticleSystem>();
    }
}
