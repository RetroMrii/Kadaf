using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TitleGlowPulse : MonoBehaviour
{
    [Tooltip("Pulse speed (cycles/sec)")]
    public float speed = 0.5f; // slow pulse
    [Tooltip("Alpha modulation amount (0..1)")]
    public float alphaAmount = 0.12f;

    Graphic graphic; // UnityEngine.UI.Image or TextMeshProUGUI
    Color baseColor;

    void Awake()
    {
        graphic = GetComponent<Graphic>();
        if (graphic == null) graphic = GetComponentInChildren<Graphic>();
        if (graphic != null) baseColor = graphic.color;
    }

    void Update()
    {
        if (graphic == null) return;
        float t = (Mathf.Sin(Time.time * Mathf.PI * 2f * speed) + 1f) * 0.5f; // 0..1
        float alphaOffset = (t - 0.5f) * 2f * alphaAmount; // -alphaAmount .. +alphaAmount
        Color c = baseColor;
        c.a = Mathf.Clamp01(baseColor.a + alphaOffset);
        graphic.color = c;
    }
}