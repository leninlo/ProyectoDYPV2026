using UnityEngine;

public class InterectionsP38 : MonoBehaviour
{
    public Transform propellerL;
    public Transform propellerR;
    public SimplePlaneController plane;

    void Start()
    {
        if (plane == null)
            plane = GetComponentInParent<SimplePlaneController>();
    }

    void Movment2Motor(float speed)
    {
        propellerL.Rotate(Vector3.up * speed * 20 * Time.deltaTime);
        propellerR.Rotate(Vector3.up * speed * 20 * Time.deltaTime);
    }
    void Update()
    {
        Movment2Motor(plane.currentSpeed);
    }
}