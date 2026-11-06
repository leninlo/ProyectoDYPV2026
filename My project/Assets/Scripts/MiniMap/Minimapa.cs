using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Minimap : MonoBehaviour
{
    [Header("Referencias")]
    public Transform player;
    public RectTransform radarArea;        // El panel circular donde viven los puntos
    public Image dotPrefab;                // Imagen pequeña (círculo), Raycast Target desactivado

    [Header("Radar")]
    public float range = 600f;             // Metros del centro al borde del radar
    public bool rotateWithPlayer = true;   // true: "arriba" del radar = hacia donde mira el avión
    public bool showOutOfRange = true;     // true: los lejanos se pegan al borde; false: se ocultan
    public float updateRate = 30f;         // Actualizaciones por segundo (0 = cada frame)

    [Header("Colores")]
    public Color enemyColor = Color.red;
    public Color allyColor = new Color(0.2f, 0.5f, 1f);

    private readonly List<Image> dots = new List<Image>();
    private float timer;

    void Update()
    {
        if (player == null) return;

        if (updateRate > 0f)
        {
            timer += Time.deltaTime;
            if (timer < 1f / updateRate) return;
            timer = 0f;
        }

        float radius = radarArea.rect.width * 0.5f;
        Quaternion inv = rotateWithPlayer
            ? Quaternion.Euler(0f, -player.eulerAngles.y, 0f)
            : Quaternion.identity;

        List<RadarTarget> all = RadarTarget.All;
        int used = 0;

        for (int i = 0; i < all.Count; i++)
        {
            RadarTarget t = all[i];
            if (t == null || t.transform == player) continue;

            // Posición relativa al jugador, vista desde arriba
            Vector3 local = inv * (t.transform.position - player.position);
            Vector2 p = new Vector2(local.x, local.z) / range * radius;

            if (p.magnitude > radius)
            {
                if (!showOutOfRange) continue;
                p = p.normalized * radius;   // Se pega al borde del círculo
            }

            Image dot = GetDot(used++);
            dot.rectTransform.anchoredPosition = p;
            dot.color = (t.team == RadarTarget.Team.Enemy) ? enemyColor : allyColor;
        }

        // Apaga los puntos que sobran (se reutilizan, no se destruyen)
        for (int i = used; i < dots.Count; i++)
            if (dots[i].gameObject.activeSelf) dots[i].gameObject.SetActive(false);
    }

    // Pool: crea puntos solo cuando hacen falta y los reutiliza
    Image GetDot(int index)
    {
        while (dots.Count <= index)
            dots.Add(Instantiate(dotPrefab, radarArea));

        Image d = dots[index];
        if (!d.gameObject.activeSelf) d.gameObject.SetActive(true);
        return d;
    }
}