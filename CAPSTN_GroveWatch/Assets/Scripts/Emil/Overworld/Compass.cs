using UnityEngine;

public class Compass : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private RectTransform northIcon;
    [SerializeField] private RectTransform eastIcon;
    [SerializeField] private RectTransform southIcon;
    [SerializeField] private RectTransform westIcon;

    [Header("Settings")]
    [SerializeField] private float pixelsPerDegree = 2f;

    private RectTransform compassRect;

    private void Awake()
    {
        compassRect = transform as RectTransform;
    }

    void Update()
    {
        UpdateCardinal(northIcon, 0f);
        UpdateCardinal(eastIcon, 90f);
        UpdateCardinal(southIcon, 180f);
        UpdateCardinal(westIcon, 270f);
    }

    private void UpdateCardinal(RectTransform icon, float cardinalAngle)
    {
        if (icon == null)
            return;

        if (compassRect == null)
            return;

        // Negative values place the cardinal to the left; positive values place it to the right.
        float angle = Mathf.DeltaAngle(player.eulerAngles.y, cardinalAngle);
        float offset = angle * pixelsPerDegree;

        // Keep the complete icon within the compass's left and right edges.
        float maxOffset = Mathf.Max(0f, (compassRect.rect.width - icon.rect.width) * 0.5f);
        bool visible = Mathf.Abs(offset) <= maxOffset;
        float x = Mathf.Clamp(offset, -maxOffset, maxOffset);

        icon.gameObject.SetActive(visible);
        icon.anchoredPosition = new Vector2(x, icon.anchoredPosition.y);
    }
}
