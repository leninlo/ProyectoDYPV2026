    using UnityEngine;
using UnityEngine.Pool;

public class Projectile : MonoBehaviour
{
    public LayerMask hitMask = ~0;

    Vector3 velocity;
    float damage, lifeLeft;
    GameObject owner;
    IObjectPool<Projectile> pool;

    public void Launch(Vector3 vel, float dmg, float life, GameObject ownerObj, IObjectPool<Projectile> p)
    {
        velocity = vel; damage = dmg; lifeLeft = life; owner = ownerObj; pool = p;
        GetComponent<TrailRenderer>()?.Clear();
    }

    void Update()
    {
        float dt = Time.deltaTime;
        Vector3 step = velocity * dt;

        // Raycast en vez de collider: las balas rápidas atraviesan objetos con colliders
        if (Physics.Raycast(transform.position, step.normalized, out RaycastHit hit,
                            step.magnitude, hitMask, QueryTriggerInteraction.Ignore))
        {
            if (hit.collider.attachedRigidbody == null ||
                hit.collider.attachedRigidbody.gameObject != owner)
            {
                hit.collider.GetComponentInParent<IDamageable>()?.TakeDamage(damage);
                Release();
                return;
            }
        }

        transform.position += step;

        lifeLeft -= dt;
        if (lifeLeft <= 0f) Release();
    }

    void Release()
    {
        if (pool != null)
        {
            pool.Release(this);
        }
        else
        {
            Debug.LogWarning($"Bala sin pool: {name}", this);
            Destroy(gameObject);
        }
    }
}