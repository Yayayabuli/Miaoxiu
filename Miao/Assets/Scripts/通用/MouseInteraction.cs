using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MouseInteraction : MonoBehaviour
{
    [Header("缩放速度")]
    public float zoomSpeed = 0.1f;

    [Header("操作的拼图图片数组")]
    public CheckCorrect[] allPieces;

    [Header("Canvas 引用（用于坐标转换）")]
    public Canvas canvas;

    [Header("滚轮缩放范围")]
    public float minScale = 0.3f;
    public float maxScale = 6f;

    // 拖拽状态
    private bool isDragging = false;
    private CheckCorrect draggingPiece;
    private Vector3 originalPosition;
    private int draggingIndex = -1;

    void Update()
    {
        HandleMouse();
    }

    void HandleMouse()
    {
        Vector2 mousePos = Input.mousePosition;

        // 分离滚轮缩放和拖拽检测
        CheckCorrect currentPieceForScroll = null; // 鼠标在图片上即可缩放
        CheckCorrect currentPieceForDrag = null;   // 鼠标在 Slot 内才可拖拽

        foreach (var piece in allPieces)
        {
            if (!piece.isInteractable) continue;

            RectTransform rt = piece.GetComponent<RectTransform>();
            RectTransform slotRect = piece.transform.parent as RectTransform;

            // Overlay Canvas -> camera 参数传 null
            bool mouseOnPiece = RectTransformUtility.RectangleContainsScreenPoint(rt, mousePos, null);
            bool mouseOnSlot = slotRect != null && RectTransformUtility.RectangleContainsScreenPoint(slotRect, mousePos, null);

            if (mouseOnPiece)
                currentPieceForScroll = piece;

            if (mouseOnSlot)
                currentPieceForDrag = piece;
        }

        // 滚轮缩放
        if (currentPieceForScroll != null && !isDragging)
        {
            float scroll = Input.mouseScrollDelta.y;
            if (Mathf.Abs(scroll) > 0.01f)
            {
                RectTransform rt = currentPieceForScroll.GetComponent<RectTransform>();
                float newScale = rt.localScale.x + scroll * zoomSpeed;
                newScale = Mathf.Clamp(newScale, minScale, maxScale);
                rt.localScale = Vector3.one * newScale;
                MapManager.Instance.IsPieceCorrect(currentPieceForScroll);

            }
        }

        // 拖拽逻辑
        if (Input.GetMouseButtonDown(0) && currentPieceForDrag != null)
        {
            isDragging = true;
            draggingPiece = currentPieceForDrag;
            draggingIndex = System.Array.IndexOf(allPieces, draggingPiece);
            originalPosition = draggingPiece.GetComponent<RectTransform>().anchoredPosition;
        }

        if (isDragging)
        {
            RectTransform rt = draggingPiece.GetComponent<RectTransform>();
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                mousePos, null, out localPoint);
            rt.anchoredPosition = localPoint;
        }

        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            isDragging = false;
            bool swapped = false;
            for (int i = 0; i < allPieces.Length; i++)
            {
                if (i == draggingIndex) continue;
                if (!allPieces[i].isInteractable) continue;

                RectTransform rt = allPieces[i].GetComponent<RectTransform>();
                if (RectTransformUtility.RectangleContainsScreenPoint(rt, mousePos, null))
                {
                    StartCoroutine(SwapPieces(draggingPiece, allPieces[i]));
                    swapped = true;
                    break;
                }
            }

            if (!swapped)
            {
                StartCoroutine(MoveTo(draggingPiece, originalPosition, 0.15f));
            }
        }
    }

    // 动画交换两张图片
    IEnumerator SwapPieces(CheckCorrect a, CheckCorrect b)
    {
        RectTransform rtA = a.GetComponent<RectTransform>();
        RectTransform rtB = b.GetComponent<RectTransform>();

        Vector2 posA = rtA.anchoredPosition;
        Vector2 posB = rtB.anchoredPosition;

        float t = 0;
        float duration = 0.15f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float k = t / duration;
            rtA.anchoredPosition = Vector2.Lerp(posA, posB, k);
            rtB.anchoredPosition = Vector2.Lerp(posB, posA, k);
            yield return null;
        }

        rtA.anchoredPosition = posB;
        rtB.anchoredPosition = posA;

        MapManager.Instance.CheckSingle(a);
        MapManager.Instance.CheckSingle(b);
    }

    // 回弹到指定位置
    IEnumerator MoveTo(CheckCorrect piece, Vector2 target, float duration)
    {
        RectTransform rt = piece.GetComponent<RectTransform>();
        Vector2 start = rt.anchoredPosition;
        float t = 0;

        while (t < duration)
        {
            t += Time.deltaTime;
            rt.anchoredPosition = Vector2.Lerp(start, target, t / duration);
            yield return null;
        }

        rt.anchoredPosition = target;
        MapManager.Instance.CheckSingle(piece);
    }

    // 重置拖拽状态接口
    public void ResetDraggingState()
    {
        isDragging = false;
        draggingPiece = null;
        draggingIndex = -1;
    }
}
