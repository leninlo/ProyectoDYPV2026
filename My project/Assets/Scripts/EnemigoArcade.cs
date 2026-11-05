using UnityEngine;

public class EnemigoArcade : MonoBehaviour
{
    enum Estado { Acercarse, Alejarse }

    [SerializeField] Transform jugador;
    [SerializeField] float velocidad = 50f;
    [SerializeField] float giroPorSegundo = 70f;
    [SerializeField] float distanciaDisparo = 150f;
    [SerializeField] float distanciaMinima = 40f;
    [SerializeField] float tiempoAlejarse = 4f;

    Estado estado = Estado.Acercarse;
    float temporizador;
    Vector3 desvio;

    void Start() => NuevoDesvio();

    void Update()
    {
        float dist = Vector3.Distance(transform.position, jugador.position);

        switch (estado)
        {
            case Estado.Acercarse:
                Girar(jugador.position + desvio);

                if (dist < distanciaDisparo && EstaApuntando())
                    Disparar();

                if (dist < distanciaMinima)
                {
                    estado = Estado.Alejarse;
                    temporizador = tiempoAlejarse;
                }
                break;

            case Estado.Alejarse:
                // Se aleja en la dirección contraria al jugador
                Vector3 huida = transform.position + (transform.position - jugador.position);
                Girar(huida);

                temporizador -= Time.deltaTime;
                if (temporizador <= 0f)
                {
                    NuevoDesvio();
                    estado = Estado.Acercarse;
                }
                break;
        }

        transform.position += transform.forward * velocidad * Time.deltaTime;
    }

    void Girar(Vector3 objetivo)
    {
        Quaternion deseada = Quaternion.LookRotation(objetivo - transform.position);
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation, deseada, giroPorSegundo * Time.deltaTime);
    }

    bool EstaApuntando()
    {
        Vector3 haciaJugador = (jugador.position - transform.position).normalized;
        return Vector3.Angle(transform.forward, haciaJugador) < 10f;
    }

    void NuevoDesvio() => desvio = Random.insideUnitSphere * 30f;

    void Disparar()
    {
        // Aquí instancias tu bala o misil
    }
}