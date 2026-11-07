using UnityEngine;
using UnityEngine.Pool;

public class GunWeapon : Weapon
{
    ObjectPool<Projectile> pool;

    public override void Init(Rigidbody owner)
    {
        base.Init(owner);
        pool = new ObjectPool<Projectile>(
            () => Instantiate(data.projectilePrefab),
            p => p.gameObject.SetActive(true),
            p => p.gameObject.SetActive(false),
            p => Destroy(p.gameObject),
            defaultCapacity: 60, maxSize: 200);
    }

    protected override void Fire()
    {
        // Punto al frente del avión donde convergen los cañones (como el P-38)
        Vector3 aimPoint = transform.position + transform.forward * data.convergenceDistance;

        foreach (Transform m in muzzles)
        {
            Vector3 dir = (aimPoint - m.position).normalized;
            dir = Quaternion.Euler(Random.insideUnitSphere * data.spreadDegrees) * dir;

            Projectile p = pool.Get();
            p.transform.SetPositionAndRotation(m.position, Quaternion.LookRotation(dir));
            // Hereda la velocidad del avión para que no "se queden atrás"
            Vector3 velocity = dir * data.projectileSpeed + ownerRb.velocity;
            p.Launch(velocity, data.damage, data.projectileLifetime, ownerRb.gameObject, pool);
        }
    }
}