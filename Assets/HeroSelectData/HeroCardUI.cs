using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class HeroCardUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Data")]
    [SerializeField] private HeroData heroData;
    [SerializeField] private Sprite portraitSprite;
    [SerializeField] private bool isUnlocked = true;

    [Header("UI")]
    [SerializeField] private Image portraitImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private GameObject lockedOverlay;

    [Header("Animation")]
    [SerializeField] private float normalScale = 1f;
    [SerializeField] private float hoverScale = 1.08f;
    [SerializeField] private float selectedScale = 1.12f;
    [SerializeField] private float scaleSpeed = 10f;

    private HeroSelectManager manager;
    private Vector3 targetScale;
    private bool isSelected;

    public HeroData HeroData => heroData;
    public Sprite PortraitSprite => portraitSprite;
    public bool IsUnlocked => isUnlocked;

    public void Setup(HeroSelectManager heroSelectManager)
    {
        manager = heroSelectManager;

        if (portraitImage != null)
            portraitImage.sprite = portraitSprite;

        if (nameText != null && heroData != null)
            nameText.text = heroData.heroName;

        if (lockedOverlay != null)
            lockedOverlay.SetActive(!isUnlocked);

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

    public void SetSelected(bool selected)
    {
        isSelected = selected;
        targetScale = Vector3.one * (isSelected ? selectedScale : normalScale);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (heroData == null || !isUnlocked) return;

        if (!isSelected)
            targetScale = Vector3.one * hoverScale;

        manager.PreviewHero(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (heroData == null || !isUnlocked) return;

        targetScale = Vector3.one * (isSelected ? selectedScale : normalScale);
        manager.EndPreview();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (heroData == null || !isUnlocked) return;

        manager.SelectHero(this);
    }
}