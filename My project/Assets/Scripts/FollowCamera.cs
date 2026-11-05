using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [Header("Objetivo")]
    public Transform target;                 // Arrastra aquí el avión (el objeto con el Rigidbody)

    [Header("Posición")]
    public Vector3 offset = new Vector3(0f, 3f, -10f);  // detrás y arriba

    [Header("Seguimiento")]
    public float rotationFollow = 4f;        // Más bajo = la cámara "se queda atrás" más en los giros

    [Header("Mirada")]
    public float lookAhead = 30f;            // Mira un punto delante del avión
    public Vector3 lookOffset = new Vector3(0f, 1f, 0f);
    public float lookSmooth = 8f;

    private Quaternion followRotation;

    void Start()
    {
        if (target != null)
            followRotation = NoRoll(target.rotation);
    }

    void LateUpdate()
    {
        if (target == null) return;

        float dt = Time.deltaTime;
        Quaternion targetRot = NoRoll(target.rotation);

        // 1) La orientación de seguimiento persigue a la del avión (sin alabeo)
        //    con retraso suave. (1 - exp) hace el suavizado igual a cualquier FPS.
        followRotation = Quaternion.Slerp(
            followRotation,
            targetRot,
            1f - Mathf.Exp(-rotationFollow * dt)
        );

        // 2) Posición: siempre a la misma distancia del avión.
        transform.position = target.position + followRotation * offset;

        // 3) Mirada: un punto delante del avión, así ves hacia dónde vas
        Vector3 lookPoint = target.position
                          + targetRot * Vector3.forward * lookAhead
                          + targetRot * lookOffset;

        Quaternion desiredRot = Quaternion.LookRotation(lookPoint - transform.position, Vector3.up);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            desiredRot,
            1f - Mathf.Exp(-lookSmooth * dt)
        );
    }

    // Devuelve la rotación del objetivo sin alabeo (solo cabeceo y guiñada)
    Quaternion NoRoll(Quaternion q)
    {
        return Quaternion.LookRotation(q * Vector3.forward, Vector3.up);
    }
}