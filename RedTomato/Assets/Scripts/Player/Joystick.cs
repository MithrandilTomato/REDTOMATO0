using UnityEngine;
using UnityEngine.EventSystems;

public class Joystick : MonoBehaviour,
                        IDragHandler,
                        IPointerUpHandler,
                        IPointerDownHandler
{
    [Header("UI References")]
    [SerializeField] private RectTransform background;
    [SerializeField] private RectTransform handle;

    [Header("Configuration")]
    [Tooltip("Handle’in background içinde ne kadar hareket edebileceði (0..1)")]
    [Range(0f, 1f)]
    public float handleRange = 0.5f;

    private Vector2 input = Vector2.zero;
    private Vector2 backgroundCenter;

    void Start()
    {
        // Background pivot ortadaysa center de (0,0) demektir.
        // Ama emin olmak için world-space merkezini alalým:
        //backgroundCenter = background.position;

        // Handle pozisyonunu sýfýra çekelim:
        handle.anchoredPosition = Vector2.zero;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Basýldýðýnda de OnDrag’i tetikle ki anýnda tepki versin
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 localPoint;
        // Ekran noktasýný background'un local koordinatýna çevir:
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                background, eventData.position, eventData.pressEventCamera, out localPoint))
            return;

        // localPoint'un pivot merkezli olmasýný saðla
        Vector2 pivotOffset = background.pivot * background.sizeDelta;
        Vector2 normalized = (localPoint - (background.sizeDelta * (background.pivot)))
                             / background.sizeDelta;

        // input deðerini -1..+1 aralýðýna ölçekle:
        input = Vector2.ClampMagnitude(normalized * 2f, 1f);

        // handle pozisyonunu güncelle
        handle.anchoredPosition = new Vector2(
            input.x * background.sizeDelta.x * handleRange,
            input.y * background.sizeDelta.y * handleRange
        );
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Býrakýldýðýnda input sýfýrlansýn + handle ortaya dönsün
        input = Vector2.zero;
        handle.anchoredPosition = Vector2.zero;
    }

    /// <summary>
    /// Dýþarýdan sadece yatayý okumak için:
    /// </summary>
    public float Horizontal => input.x;

    /// <summary>
    /// Dýþarýdan sadece düþey (zýplama/inklinasyon) okumak istersen:
    /// </summary>
    public float Vertical => input.y;
}
