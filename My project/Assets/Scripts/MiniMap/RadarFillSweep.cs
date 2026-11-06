using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class RadarFillSweep : MonoBehaviour
{
    public float cycleDuration = 2f; // segundos por vuelta
    private Image img;

    void Awake() => img = GetComponent<Image>();

    void Update()
    {
        img.fillAmount = Mathf.Repeat(Time.time / cycleDuration, 1f);
    }
}