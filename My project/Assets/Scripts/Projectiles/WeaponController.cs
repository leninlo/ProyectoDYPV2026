using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public Weapon[] primaryWeapons;    // ametralladoras
    public Weapon[] secondaryWeapons;  // misiles, bombas... (vacío por ahora)
    public KeyCode primaryKey = KeyCode.Space;
    public KeyCode secondaryKey = KeyCode.F;

    void Start()
    {
        var rb = GetComponent<Rigidbody>();
        foreach (var w in primaryWeapons) w.Init(rb);
        foreach (var w in secondaryWeapons) w.Init(rb);
    }

    void Update()
    {
        if (Input.GetKey(primaryKey))
            foreach (var w in primaryWeapons) w.TryFire();

        if (Input.GetKeyDown(secondaryKey))
            foreach (var w in secondaryWeapons) w.TryFire();
    }
}