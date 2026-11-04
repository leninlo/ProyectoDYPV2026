using UnityEngine;

public class MotorP38 : MonoBehaviour
{
    public Transform propellerL;
    public Transform propellerR;

    public float velocidad = 1500f;

    void Update()
    {
        propellerL.Rotate(Vector3.up * velocidad * Time.deltaTime);
        propellerR.Rotate(Vector3.up * velocidad * Time.deltaTime);
    }
}