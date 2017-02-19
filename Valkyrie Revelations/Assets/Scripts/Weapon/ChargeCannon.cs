using UnityEngine;
using System.Collections;

public class ChargeCannon : Weapon
{    
    protected float multiplier;
    protected float chargeTime;
    protected float chargeSpeed;
    protected int charge;
    protected int maxCharge;
    protected GameObject chargeObj;
    protected ParticleSystem chargeParticle;
    protected GameObject shotObj;
    protected ParticleSystem shotParticle;

    public ChargeCannon()
    {
        name = "Charge Cannon";
        weaponTex = null;
        weaponType = WeaponType.CHARGINGCANNON;
        damage = 1;
        multiplier = 2;
        coolDown = 1f;
        maxAmmoInClip = 50;
        ammoInClip = maxAmmoInClip;
        totalAmmo = 300;
        maxCharge = 5;
        chargeSpeed = 2;
        chargeTime = 1;
        chargeObj = GameObject.Find("PlayerCharge");
        chargeParticle = chargeObj.GetComponent<ParticleSystem>();
        shotObj = GameObject.Find("PlayerShot");
        shotParticle = shotObj.GetComponent<ParticleSystem>();
    }

    public float GetMultiplier()
    {
        return multiplier;
    }
    public float GetChargeTime()
    {
        return chargeTime;
    }
    public void AddChargeTime(float ct, Transform transform)
    {
        if (ammoInClip > 1 && chargeTime < maxCharge)
        {
            if ((int)chargeTime < (int)(chargeTime + ct * chargeSpeed))
            {
                ammoInClip--;
            }
            chargeTime += ct * chargeSpeed;
            if (chargeTime > maxCharge)
            {
                chargeTime = maxCharge;
            }
        }
        
    }
    public void ResetChargeTime()
    {
        chargeTime = 1;
    }
    public GameObject GetChargeObj()
    {
        return chargeObj;
    }
    public ParticleSystem GetChargeParticle()
    {
        return chargeParticle;
    }
    public GameObject GetShotObj()
    {
        return shotObj;
    }
    public ParticleSystem GetShotParticle()
    {
        return shotParticle;
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
        if (chargeParticle.isPlaying) { 
            chargeParticle.Stop();
            chargeParticle.enableEmission = false;
            shotParticle.Play();
            shotParticle.enableEmission = true;
        }
        ResetChargeTime();
        return coolDown;
    }

    public override void ResetWeapon()
    {
        if (chargeParticle.isPlaying)
        {
            chargeParticle.Stop();
            chargeParticle.enableEmission = false;
        }
        ResetChargeTime();
    }
}
