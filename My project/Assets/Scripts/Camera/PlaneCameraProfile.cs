using UnityEngine;

public class PlaneCameraProfile : MonoBehaviour
{
    [Header("1. Tercera persona (detrás)")]
    public Vector3 chaseOffset = new Vector3(0f, 3f, -10f);
    public float lookAhead = 30f;
    public Vector3 lookOffset = new Vector3(0f, 1f, 0f);

    [Header("2. Frontal (delante, mirando hacia atrás)")]
    public Vector3 frontOffset = new Vector3(0f, 2f, 12f);

    [Header("3. Cabina")]
    public Transform cockpitPoint;                       // Objeto vacío hijo, colocado en la cabina
    public Vector3 cockpitOffset = new Vector3(0f, 0.8f, 1f); // Solo se usa si no asignas cockpitPoint

    public Vector3 CockpitPosition()
    {
        return cockpitPoint != null ? cockpitPoint.position : transform.TransformPoint(cockpitOffset);
    }

    // Al seleccionar el avión, muestra dónde quedaría cada cámara (aprox., con el avión nivelado)
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.TransformPoint(chaseOffset), 0.5f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.TransformPoint(frontOffset), 0.5f);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(CockpitPosition(), 0.3f);
    }
}