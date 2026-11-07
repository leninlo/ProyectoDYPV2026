using UnityEngine;

[CreateAssetMenu(menuName = "Aviones/Weapon Data")]
public class WeaponData : ScriptableObject
{
    public string weaponName = "P-38 Guns";
    public Projectile projectilePrefab;
    public float fireRate = 12f;          // disparos por segundo
    public float damage = 10f;
    public float projectileSpeed = 400f;  // debe ser mayor que la velocidad del avión
    public float projectileLifetime = 2f;
    public float spreadDegrees = 0.4f;    // dispersión leve
    public float convergenceDistance = 300f; // dónde se cruzan las balas de los cañones
    public int maxAmmo = -1;              // -1 = infinita
}