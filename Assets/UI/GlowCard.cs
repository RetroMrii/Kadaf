using UnityEngine;

public class CardGlowSweep : MonoBehaviour
{
    [SerializeField] private RectTransform glowTransform;
    [SerializeField] private float leftX = -220f;
    [SerializeField] private float rightX = 220f;
    [SerializeField] private float speed = 80f;

    private float currentX;
    private bool movingRight = true;

    private void Awake()
    {
        if (glowTransform == null)
            glowTransform = transform as RectTransform;

        currentX = leftX;
        SetPosition();
    }

    private void Update()
    {
        float dir = movingRight ? 1f : -1f;
        currentX += dir * speed * Time.deltaTime;

        if (currentX >= rightX)
        {
            currentX = rightX;
            movingRight = false;
        }
        else if (currentX <= leftX)
        {
            currentX = leftX;
            movingRight = true;
        }

        SetPosition();
    }

    private void SetPosition()
    {
        Vector2 pos = glowTransform.anchoredPosition;
        pos.x = currentX;
        glowTransform.anchoredPosition = pos;
    }
}