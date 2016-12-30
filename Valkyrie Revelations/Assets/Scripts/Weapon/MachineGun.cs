using UnityEngine;
using System.Collections;

public class MachineGun : Weapon {
    public MachineGun()
    {
        name = "Machine Gun";
        weaponTex = null;
        rapidFire = true;
        damage = 5;
        coolDown = 0.1f;
        maxAmmoInClip = 50;
        ammoInClip = maxAmmoInClip;
    }
}
