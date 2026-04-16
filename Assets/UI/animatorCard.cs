using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AnimatedRoomCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Target")]
    [SerializeField] private RectTransform targetTransform;
    [SerializeField] private Image glowImage;

    [Header("Scale")]
    [SerializeField] private float normalScale = 1f;
    [SerializeField] private float hoverScale = 1.06f;
    [SerializeField] private float pressedScale = 0.96f;
    [SerializeField] private float scaleSpeed = 10f;

    [Header("Glow")]
    [SerializeField] private float glowSpeed = 2f;
    [SerializeField] private float glowMinAlpha = 0.35f;
    [SerializeField] private float glowMaxAlpha = 0.75f;

    private float targetScale;
    private bool isHovered;
    private bool isPressed;

    private void Awake()
    {
        if (targetTransform == null)
            targetTransform = transform as RectTransform;

        targetScale = normalScale;
    }

    private void Update()
    {
        UpdateScale();
        UpdateGlow();
    }

    private void UpdateScale()
    {
        Vector3 desiredScale = Vector3.one * targetScale;
        targetTransform.localScale = Vector3.Lerp(
            targetTransform.localScale,
            desiredScale,
            Time.deltaTime * scaleSpeed
        );
    }

    private void UpdateGlow()
    {
        if (glowImage == null)
            return;

        Color c = glowImage.color;
        float pulse = Mathf.Lerp(glowMinAlpha, glowMaxAlpha, (Mathf.Sin(Time.time * glowSpeed) + 1f) * 0.5f);
        c.a = pulse;
        glowImage.color = c;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        isPressed = false;
        targetScale = hoverScale;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        isPressed = false;
        targetScale = normalScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
        targetScale = pressedScale;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
        targetScale = isHovered ? hoverScale : normalScale;
    }
}