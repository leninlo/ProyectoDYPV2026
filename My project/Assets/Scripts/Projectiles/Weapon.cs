using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public WeaponData data;
    public Transform[] muzzles;

    protected Rigidbody ownerRb;
    protected float nextFireTime;
    protected int ammo;

    public virtual void Init(Rigidbody owner)
    {
        ownerRb = owner;
        ammo = data.maxAmmo;
    }

    public bool TryFire()
    {
        if (Time.time < nextFireTime) return false;
        if (data.maxAmmo >= 0 && ammo <= 0)
        {
            return false;
        }

        nextFireTime = Time.time + 1f / data.fireRate;
        Fire();
        if (data.maxAmmo >= 0) ammo--;
        return true;
    }
    protected abstract void Fire();
}