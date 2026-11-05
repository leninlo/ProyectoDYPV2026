using UnityEngine;

public class CameraSystem : MonoBehaviour
{
    public enum CamMode { Chase, Front, Cockpit }

    [Header("Objetivo")]
    public Transform target;                 // El avión activo
    public KeyCode switchKey = KeyCode.Tab;
    public CamMode mode = CamMode.Chase;

    [Header("Opciones")]
    public bool hidePlaneInCockpit = true;

    [Header("Suavizado")]
    public float rotationFollow = 4f;
    public float lookSmooth = 8f;

    private PlaneCameraProfile profile;
    private Renderer[] planeRenderers;
    private Quaternion followRotation;
    private bool snap;

    void Start()
    {
        if (target != null) SetTarget(target);
    }

    // Llámalo cuando cambies de avión (al spawnear o elegir otro modelo)
    public void SetTarget(Transform newTarget)
    {
        // Deja visible el avión anterior
        SetPlaneVisible(true);

        target = newTarget;
        profile = target.GetComponent<PlaneCameraProfile>();
        if (profile == null)
            profile = target.gameObject.AddComponent<PlaneCameraProfile>(); // valores por defecto

        planeRenderers = target.GetComponentsInChildren<Renderer>();
        SetMode(mode);
    }

    void Update()
    {
        if (target != null && Input.GetKeyDown(switchKey))
            SetMode((CamMode)(((int)mode + 1) % 3));
    }

    void SetMode(CamMode newMode)
    {
        mode = newMode;
        snap = true;
        followRotation = NoRoll(target.rotation);
        SetPlaneVisible(!(hidePlaneInCockpit && mode == CamMode.Cockpit));
    }

    void SetPlaneVisible(bool visible)
    {
        if (planeRenderers == null) return;
        foreach (Renderer r in planeRenderers)
            if (r != null) r.enabled = visible;
    }

    void LateUpdate()
    {
        if (target == null || profile == null) return;

        // CABINA: pegada al avión, gira con él (incluido el alabeo)
        if (mode == CamMode.Cockpit)
        {
            transform.position = profile.CockpitPosition();
            transform.rotation = target.rotation;
            return;
        }

        // CHASE y FRONT
        float dt = Time.deltaTime;
        Quaternion targetRot = NoRoll(target.rotation);

        float followK = snap ? 1f : 1f - Mathf.Exp(-rotationFollow * dt);
        followRotation = Quaternion.Slerp(followRotation, targetRot, followK);

        Vector3 offset = (mode == CamMode.Chase) ? profile.chaseOffset : profile.frontOffset;
        transform.position = target.position + followRotation * offset;

        Vector3 lookPoint = target.position + targetRot * profile.lookOffset;
        if (mode == CamMode.Chase)
            lookPoint += targetRot * Vector3.forward * profile.lookAhead;

        Quaternion desiredRot = Quaternion.LookRotation(lookPoint - transform.position, Vector3.up);

        float lookK = snap ? 1f : 1f - Mathf.Exp(-lookSmooth * dt);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRot, lookK);

        snap = false;
    }

    Quaternion NoRoll(Quaternion q)
    {
        return Quaternion.LookRotation(q * Vector3.forward, Vector3.up);
    }
}