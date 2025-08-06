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
    [Tooltip("Handle�in background i�inde ne kadar hareket edebilece�i (0..1)")]
    [Range(0f, 1f)]
    public float handleRange = 0.5f;

    private Vector2 input = Vector2.zero;
    private Vector2 backgroundCenter;

    void Start()
    {
        // Background pivot ortadaysa center de (0,0) demektir.
        // Ama emin olmak i�in world-space merkezini alal�m:
        //backgroundCenter = background.position;

        // Handle pozisyonunu s�f�ra �ekelim:
        handle.anchoredPosition = Vector2.zero;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Bas�ld���nda de OnDrag�i tetikle ki an�nda tepki versin
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 localPoint;
        // Ekran noktas�n� background'un local koordinat�na �evir:
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                background, eventData.position, eventData.pressEventCamera, out localPoint))
            return;

        // localPoint'un pivot merkezli olmas�n� sa�la
        Vector2 pivotOffset = background.pivot * background.sizeDelta;
        Vector2 normalized = (localPoint - (background.sizeDelta * (background.pivot)))
                             / background.sizeDelta;

        // input de�erini -1..+1 aral���na �l�ekle:
        input = Vector2.ClampMagnitude(normalized * 2f, 1f);

        // handle pozisyonunu g�ncelle
        handle.anchoredPosition = new Vector2(
            input.x * background.sizeDelta.x * handleRange,
            input.y * background.sizeDelta.y * handleRange
        );
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // B�rak�ld���nda input s�f�rlans�n + handle ortaya d�ns�n
        input = Vector2.zero;
        handle.anchoredPosition = Vector2.zero;
    }

    /// <summary>
    /// D��ar�dan sadece yatay� okumak i�in:
    /// </summary>
    public float Horizontal => input.x;

    /// <summary>
    /// D��ar�dan sadece d��ey (z�plama/inklinasyon) okumak istersen:
    /// </summary>
    public float Vertical => input.y;
}
