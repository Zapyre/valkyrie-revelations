using UnityEngine;
using System.Collections;

public class Weapon {
    private string name;
    private Texture weaponTex;
    private bool rapidFire;
    private float damage;
    private float coolDown;

    public string GetName()
    {
        return name;
    }
    public Texture GetWeaponTex()
    {
        return weaponTex;
    }
    public bool IsRapidFire()
    {
        return rapidFire;
    }
    public float GetDamage()
    {
        return damage;
    }
    public float GetCoolDown()
    {
        return coolDown;
    }
}
