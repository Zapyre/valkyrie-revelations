using UnityEngine;
using System.Collections;

public class Weapon {
    protected string name;
    protected Texture weaponTex;
    protected bool rapidFire;
    protected int damage;
    protected float coolDown;
    protected int maxAmmoInClip;
    protected int ammoInClip;

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
    public int GetDamage()
    {
        return damage;
    }
    public float GetCoolDown()
    {
        return coolDown;
    }
    public int GetMaxAmmoInClip()
    {
        return maxAmmoInClip;
    }
    public int GetAmmoInClip()
    {
        return ammoInClip;
    }
    public void Reload()
    {
        ammoInClip = maxAmmoInClip;
    }

    public virtual float ShootWeapon(RaycastHit hit, Transform transform)
    {
        Projectile projectile = new Projectile(transform.FindChild("Gunpoint").position);
        GameObject bullet = projectile.projectileObj;
        ProjectileMovement pm = (ProjectileMovement)bullet.GetComponent(typeof(ProjectileMovement));
        if (hit.point != null)
        {
            pm.SetDestination(hit.point);
            pm.CalculateSpeed(2000.0f);
            pm.setDamage(damage);
        }
        ammoInClip--;
        return coolDown;
    }
}
