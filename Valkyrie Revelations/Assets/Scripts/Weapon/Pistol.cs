using UnityEngine;
using System.Collections;

public class Pistol : Weapon {
    public Pistol()
    {
        name = "Pistol";
        weaponTex = null;
        rapidFire = false;
        damage = 10;
        coolDown = 0.2f;
        maxAmmoInClip = 10;
        ammoInClip = maxAmmoInClip;
        totalAmmo = -1;
    }
}
