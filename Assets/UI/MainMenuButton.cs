using UnityEngine;
using UnityEngine.EventSystems;

public class MenuButtonAnimator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Scale")]
    [SerializeField] private float normalScale = 1f;
    [SerializeField] private float hoverScale = 1.08f;
    [SerializeField] private float pressedScale = 0.96f;

    [Header("Animation")]
    [SerializeField] private float scaleSpeed = 10f;

    private Vector3 targetScale;
    private bool isHovering;

    private void Awake()
    {
        targetScale = Vector3.one * normalScale;
        transform.localScale = targetScale;
    }

    private void Update()
    {
        transform.localScale = Vector3.Lerp(
            transform.localScale,
            targetScale,
            Time.deltaTime * scaleSpeed
        );
    }   

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        targetScale = Vector3.one * hoverScale;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        targetScale = Vector3.one * normalScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        targetScale = Vector3.one * pressedScale;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        targetScale = Vector3.one * (isHovering ? hoverScale : normalScale);
    }
}