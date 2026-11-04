using UnityEngine;

// Ocupamos un Rigibody paar manejar la fisica.
[RequireComponent(typeof(Rigidbody))]
public class SimplePlaneController : MonoBehaviour
{
    // ============================================================
    // 1) PARÁMETROS
    // ============================================================
    [Header("1. Movimiento")]
    public float forwardSpeed = 50f;      // Velocidad constante (m/s)

    [Header("2. Giro (grados/seg)")]
    public float pitchSpeed = 60f;        // W/S: picar / subir nariz
    public float yawSpeed = 50f;          // A/D: girar izquierda / derecha
    public float maxPitch = 80f;          // Límite para no dar la vuelta completa
    public bool invertPitch = false;      // false: W baja el morro, S lo sube

    [Header("3. Suavizado de controles")]
    public float inputSmoothing = 5f;     // Más alto = respuesta más rápida

    [Header("4. Alabeo visual (inclinación al girar)")]
    public Transform visualModel;         // Hijo con el modelo 3D del avión
    public float maxBank = 45f;           // Inclinación máxima de las alas
    public float bankSmoothing = 4f;      // Qué tan rápido se inclina / se nivela

    // ============================================================
    // Referencias internas
    // ============================================================
    private Rigidbody rb;
    private float pitch, yaw;             // Orientación actual (grados)
    private float pitchInput, turnInput;  // Entradas ya suavizadas (-1..1)
    private float bank;                   // Alabeo visual actual

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        rb.useGravity = false;
        rb.drag = 0f;
        rb.angularDrag = 0f;
        rb.constraints = RigidbodyConstraints.FreezeRotation; // rotamos por código

        // Evita tirones entre física (FixedUpdate) y pantalla (Update)
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        // Parte de la orientación inicial del avión
        Vector3 e = transform.eulerAngles;
        yaw = e.y;
        pitch = NormalizeAngle(e.x);
    }

    // ============================================================
    // BLOQUE 1: LEER TECLADO Y SUAVIZAR
    // ============================================================
    void Update()
    {
        // Unity: pitch positivo = morro hacia abajo.
        // W (+1) = picar, S (-1) = subir (se invierte con invertPitch)
        float targetPitch = Input.GetAxisRaw("Vertical") * (invertPitch ? -1f : 1f);

        // A (-1) = izquierda, D (+1) = derecha
        float targetTurn = Input.GetAxisRaw("Horizontal");

        // La entrada sube y baja de forma gradual, sin saltos bruscos
        float step = inputSmoothing * Time.deltaTime;
        pitchInput = Mathf.MoveTowards(pitchInput, targetPitch, step);
        turnInput = Mathf.MoveTowards(turnInput, targetTurn, step);

        ApplyVisualBank();
    }

    // ============================================================
    // BLOQUE 2: MOVIMIENTO Y ROTACIÓN (física)
    // ============================================================
    void FixedUpdate()
    {
        float dt = Time.fixedDeltaTime;

        pitch = Mathf.Clamp(pitch + pitchInput * pitchSpeed * dt, -maxPitch, maxPitch);
        yaw += turnInput * yawSpeed * dt;

        Quaternion rot = Quaternion.Euler(pitch, yaw, 0f);
        rb.MoveRotation(rot);

        // Avance constante hacia donde apunta el avión
        rb.velocity = rot * Vector3.forward * forwardSpeed;
    }

    // ------------------------------------------------------------
    // Alabeo visual: el modelo se inclina al girar y se nivela solo
    // al soltar A/D (sin trucos con eulerAngles)
    // ------------------------------------------------------------
    void ApplyVisualBank()
    {
        if (visualModel == null) return;

        // D (+1) -> ala derecha abajo (z negativo en Unity)
        float targetBank = -turnInput * maxBank;
        bank = Mathf.Lerp(bank, targetBank, bankSmoothing * Time.deltaTime);
        visualModel.localRotation = Quaternion.Euler(0f, 0f, bank);
    }

    // Utilidad: convierte un ángulo de 0..360 a -180..180
    float NormalizeAngle(float angle)
    {
        angle = angle % 360f;
        if (angle > 180f) angle -= 360f;
        return angle;
    }
}