using UnityEngine;
using System.Collections;

public class MachineGun : Weapon {
    public MachineGun()
    {
        name = "Machine Gun";
        weaponTex = null;
        weaponType = WeaponType.RAPIDFIRE;
        damage = 8;
        coolDown = 0.1f;
        maxAmmoInClip = 50;
        ammoInClip = maxAmmoInClip;
        totalAmmo = 300;
    }
}
