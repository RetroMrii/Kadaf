using UnityEngine;
using TMPro;

public class TitlePulse : MonoBehaviour
{
    private TextMeshProUGUI text;
    private Color baseColor;

    [SerializeField] private float pulseSpeed = 2f;
    [SerializeField] private float pulseStrength = 0.1f;

    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        baseColor = text.color;
    }

    void Update()
    {
        float t = Mathf.Sin(Time.time * pulseSpeed) * pulseStrength;
        text.color = baseColor + new Color(t, t, t, 0);
    }
}