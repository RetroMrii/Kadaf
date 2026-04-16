using UnityEngine;
using UnityEngine.UI;

public class BeamPulseUI : MonoBehaviour
{
    public Graphic target;
    public float minAlpha = 0.15f;
    public float maxAlpha = 0.35f;
    public float speed = 0.6f;

    void Update()
    {
        if (target == null) return;

        Color c = target.color;
        float t = (Mathf.Sin(Time.time * speed) + 1f) * 0.5f;
        c.a = Mathf.Lerp(minAlpha, maxAlpha, t);
        target.color = c;
    }
}