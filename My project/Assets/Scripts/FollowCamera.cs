using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [Header("Objetivo")]
    public Transform target;                 // Arrastra aquí el avión

    [Header("Posición")]
    public Vector3 offset = new Vector3(0f, 3f, -10f);  // detrás y arriba
    public float positionSmooth = 5f;

    [Header("Rotación")]
    public float lookSmooth = 5f;
    public Vector3 lookOffset = new Vector3(0f, 1f, 0f); // mira un poco arriba del avión

    private Vector3 velocity = Vector3.zero;

    void LateUpdate()
    {
        if (target == null) return;

        // Posición deseada: detrás del avión, en su espacio local
        Vector3 desiredPos = target.position + target.TransformDirection(offset);

        // Suavizado de posición
        transform.position = Vector3.SmoothDamp(
            transform.position,
            desiredPos,
            ref velocity,
            1f / positionSmooth
        );

        // Rotación: mirar al avión (con un pequeño offset vertical)
        Vector3 lookPoint = target.position + target.TransformDirection(lookOffset);
        Quaternion desiredRot = Quaternion.LookRotation(lookPoint - transform.position);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            desiredRot,
            lookSmooth * Time.deltaTime
        );
    }
}