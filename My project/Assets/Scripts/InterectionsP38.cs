using UnityEngine;

public class InterectionsP38 : MonoBehaviour
{
    public Transform propellerL;
    public Transform propellerR;

    void Movment2Motor(float speed)
    {
        propellerL.Rotate(Vector3.up * speed * Time.deltaTime);
        propellerR.Rotate(Vector3.up * speed * Time.deltaTime);
    }
}