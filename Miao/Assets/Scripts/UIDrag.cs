using UnityEngine;
using UnityEngine.EventSystems;

public class UIDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rect;
    private Canvas canvas;
    private Vector2 originalPos;
    private Transform originalParent;

    [HideInInspector] public RectTransform[] panels;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPos = rect.anchoredPosition;
        originalParent = transform.parent;
        
        transform.SetParent(canvas.transform);
    }

    public void OnDrag(PointerEventData eventData)
    {
        rect.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        foreach (var p in panels)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(p, Input.mousePosition))
            {
                transform.SetParent(p);
                rect.anchoredPosition = Vector2.zero;
                return;
            }
        }
        
        transform.SetParent(originalParent);
        rect.anchoredPosition = originalPos;
    }
}