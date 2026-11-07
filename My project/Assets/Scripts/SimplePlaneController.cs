using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SimplePlaneController : MonoBehaviour
{
    [Header("1. Velocidad")]
    public float baseSpeed = 50f;         // Velocidad normal de crucero (m/s)
    public float maxSpeed = 100f;         // Velocidad máxima con Shift
    public float acceleration = 30f;      // Qué tan rápido acelera (m/s²)
    public float deceleration = 20f;      // Qué tan rápido vuelve a la base (m/s²)
    public KeyCode boostKey = KeyCode.LeftShift;

    [Header("2. Giro (grados/seg)")]
    public float pitchSpeed = 60f;
    public float yawSpeed = 50f;
    public float maxPitch = 80f;
    public bool invertPitch = false;

    [Header("3. Suavizado de controles")]
    public float inputSmoothing = 5f;

    [Header("4. Inclinación visual al girar")]
    public float maxBank = 30f;           // Alabeo (eje Z): alas inclinadas
    public float maxVisualYaw = 8f;       // Giro extra del morro (eje Y), leve
    public float bankSmoothing = 4f;

    private Rigidbody rb;
    private float pitch, yaw;
    private float pitchInput, turnInput;
    private float bank, visualYaw;
    public float currentSpeed; //publico
    private bool boosting;


    void Start()
    {
        rb = GetComponent<Rigidbody>();

        rb.useGravity = false;
        rb.drag = 0f;
        rb.angularDrag = 0f;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        Vector3 e = transform.eulerAngles;
        yaw = e.y;
        pitch = NormalizeAngle(e.x);

        currentSpeed = baseSpeed;
    }

    void Update()
    {
        float targetPitch = Input.GetAxisRaw("Vertical") * (invertPitch ? -1f : 1f);
        float targetTurn = Input.GetAxisRaw("Horizontal");

        float step = inputSmoothing * Time.deltaTime;
        pitchInput = Mathf.MoveTowards(pitchInput, targetPitch, step);
        turnInput = Mathf.MoveTowards(turnInput, targetTurn, step);

        // Velocidad: Shift acelera hasta maxSpeed, al soltar vuelve a baseSpeed
        boosting = Input.GetKey(boostKey);
        float targetSpeed = boosting ? maxSpeed : baseSpeed;
        float rate = boosting ? acceleration : deceleration;
        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, rate * Time.deltaTime);

    }

    void FixedUpdate()
    {
        float dt = Time.fixedDeltaTime;

        pitch = Mathf.Clamp(pitch + pitchInput * pitchSpeed * dt, -maxPitch, maxPitch);
        yaw += turnInput * yawSpeed * dt;

        // Alabeo: se inclina al girar y se nivela solo al soltar A/D
        float targetBank = -turnInput * maxBank;
        bank = Mathf.Lerp(bank, targetBank, bankSmoothing * dt);

        Quaternion rot = Quaternion.Euler(pitch, yaw, bank);
        rb.MoveRotation(rot);

        // El alabeo gira sobre el eje de avance, así que no altera la dirección del vuelo
        rb.velocity = rot * Vector3.forward * currentSpeed;
    }
 
    float NormalizeAngle(float angle)
    {
        angle = angle % 360f;
        if (angle > 180f) angle -= 360f;
        return angle;
    }
}