using UnityEngine;
using System.Collections;

public class ChargeCannon : Weapon
{
    protected float multiplier;
    protected float chargeTime;
    protected int charge;
    protected int maxCharge;

    public ChargeCannon()
    {
        name = "Charge Cannon";
        weaponTex = null;
        weaponType = WeaponType.CHARGINGCANNON;
        damage = 1;
        multiplier = 2;
        coolDown = 0.1f;
        maxAmmoInClip = 50;
        ammoInClip = maxAmmoInClip;
        totalAmmo = 300;
        maxCharge = 5;
        chargeTime = 1;
    }

    public float GetMultiplier()
    {
        return multiplier;
    }
    public float GetChargeTime()
    {
        return chargeTime;
    }
    public void AddChargeTime(float ct)
    {
        chargeTime += ct;
    }
    public void ResetChargeTime()
    {
        chargeTime = 0;
    }

    public override float ShootWeapon(RaycastHit hit, Transform transform)
    {
        Projectile projectile = new Projectile(transform.FindChild("Gunpoint").position, false);
        GameObject bullet = projectile.projectileObj;
        ProjectileMovement pm = (ProjectileMovement)bullet.GetComponent(typeof(ProjectileMovement));
        if (hit.point != null)
        {
            charge = 1;
            if (chargeTime > maxCharge)
            {
                charge = maxCharge;
            }
            else
            {
                charge = (int)chargeTime;
            }
            pm.SetDestination(hit.point);
            pm.CalculateSpeed(2000.0f);
            pm.setDamage((int)(damage * multiplier * charge));
            Debug.Log("Damage: " + damage + " | Multiplier: " + multiplier * charge);
        }
        ammoInClip--;
        return coolDown;
    }
}
