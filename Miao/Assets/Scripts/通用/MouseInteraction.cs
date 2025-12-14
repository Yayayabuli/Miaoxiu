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
    private Vector2 dragOffset; // 鼠标到图片中心的偏移
    private Transform originalParent;
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
        if (!MapManager.Instance.canDrag)
            return;

        if (Input.GetMouseButtonDown(0) && currentPieceForDrag != null)
        {
            isDragging = true;
            draggingPiece = currentPieceForDrag;
            draggingIndex = System.Array.IndexOf(allPieces, draggingPiece);
            originalPosition = draggingPiece.GetComponent<RectTransform>().anchoredPosition;
            originalParent = draggingPiece.transform.parent;

            // 计算鼠标与图片中心的偏移
            RectTransform rt = draggingPiece.GetComponent<RectTransform>();
            Vector2 localMousePos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                Input.mousePosition, null, out localMousePos);
            dragOffset = rt.anchoredPosition - localMousePos;

            // 拖拽时提升到 Canvas 根节点，避免被 Mask 裁剪
            draggingPiece.transform.SetParent(canvas.transform, true);
        }

        if (isDragging)
        {
            RectTransform rt = draggingPiece.GetComponent<RectTransform>();
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                Input.mousePosition, null, out localPoint);
            rt.anchoredPosition = localPoint+dragOffset;
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
                    if (CanSwap(draggingPiece, allPieces[i]))
                    {
                        StartCoroutine(SwapBySlotAnimated(draggingPiece, allPieces[i]));
                        swapped = true;
                    }

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

    IEnumerator SwapBySlotAnimated(CheckCorrect a, CheckCorrect b)
    {
        RectTransform rtA = a.GetComponent<RectTransform>();
        RectTransform rtB = b.GetComponent<RectTransform>();

        Transform slotA = a.transform.parent;
        Transform slotB = b.transform.parent;

        Vector2 startA = rtA.anchoredPosition;
        Vector2 startB = rtB.anchoredPosition;

        // 交换父物体
        a.transform.SetParent(slotB, false);
        b.transform.SetParent(slotA, false);

        // 目标位置
        Vector2 endA = Vector2.zero;
        Vector2 endB = Vector2.zero;

        float t = 0f;
        float duration = 0.15f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float k = t / duration;
            rtA.anchoredPosition = Vector2.Lerp(startA, endA, k);
            rtB.anchoredPosition = Vector2.Lerp(startB, endB, k);
            yield return null;
        }

        rtA.anchoredPosition = endA;
        rtB.anchoredPosition = endB;

        MapManager.Instance.IsPieceCorrect(a);
        MapManager.Instance.IsPieceCorrect(b);
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
    bool CanSwap(CheckCorrect a, CheckCorrect b)
    {
        // 不是第三关，禁止交换
        if (!MapManager.Instance.canDrag)
            return false;

        int indexA = System.Array.IndexOf(allPieces, a);
        int indexB = System.Array.IndexOf(allPieces, b);

        return MapManager.Instance.interactIndexes.Contains(indexA)
            && MapManager.Instance.interactIndexes.Contains(indexB);
    }

}
