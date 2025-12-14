using UnityEngine;
using System.Collections;

public class MouseInteraction : MonoBehaviour
{
    [Header("缩放速度")]
    public float zoomSpeed = 0.1f;

    [Header("操作的拼图图片数组")]
    public CheckCorrect[] allPieces;

    [Header("Canvas 引用")]
    public Canvas canvas;

    [Header("滚轮缩放范围")]
    public float minScale = 0.3f;
    public float maxScale = 6f;

    // 拖拽状态
    private bool isDragging = false;
    private CheckCorrect draggingPiece;
    private Vector3 offsetToMouse;       // 鼠标与图片中心的偏移
    private Transform originalParent;    // 拖拽前的父物体
    private Vector2 originalAnchoredPos; // 拖拽前在父物体中的位置
    private int draggingIndex = -1;

    void Update()
    {
        HandleMouse();
    }

    void HandleMouse()
    {
        Vector2 mousePos = Input.mousePosition;

        // 找到鼠标下的可交互图片（用于滚轮缩放）
        CheckCorrect currentPieceForScroll = null;
        foreach (var piece in allPieces)
        {
            if (!piece.isInteractable) continue;

            RectTransform rt = piece.GetComponent<RectTransform>();
            if (RectTransformUtility.RectangleContainsScreenPoint(rt, mousePos, null))
            {
                currentPieceForScroll = piece;
                break;
            }
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

                // 默默检查正确性
                MapManager.Instance.IsPieceCorrect(currentPieceForScroll);
            }
        }

        // 拖拽逻辑
        if (Input.GetMouseButtonDown(0))
        {
            // 只允许第三关可拖拽索引
            if (!MapManager.Instance.canDrag) return;

            for (int i = 0; i < allPieces.Length; i++)
            {
                var piece = allPieces[i];
                if (!piece.isInteractable || !MapManager.Instance.interactIndexes.Contains(i)) continue;

                RectTransform rt = piece.GetComponent<RectTransform>();
                if (RectTransformUtility.RectangleContainsScreenPoint(rt, mousePos, null))
                {
                    isDragging = true;
                    draggingPiece = piece;
                    draggingIndex = i;

                    originalParent = piece.transform.parent;
                    originalAnchoredPos = piece.GetComponent<RectTransform>().anchoredPosition;

                    // 计算鼠标与图片中心偏移
                    Vector2 localMousePos;
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(
                        rt, mousePos, null, out localMousePos);
                    offsetToMouse = rt.localPosition - (Vector3)localMousePos;

                    // 将图片放到 Canvas 顶层（保持视觉在最上层，不脱离 slot 只改变 siblingIndex）
                    piece.transform.SetAsLastSibling();

                    break;
                }
            }
        }

        if (isDragging && draggingPiece != null)
        {
            // 跟随鼠标，中心对齐
            RectTransform rt = draggingPiece.GetComponent<RectTransform>();
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform, mousePos, null, out localPoint);
            rt.position = canvas.transform.TransformPoint(localPoint) + offsetToMouse;
        }

        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            isDragging = false;

            bool swapped = false;

            for (int i = 0; i < allPieces.Length; i++)
            {
                if (i == draggingIndex) continue;
                var piece = allPieces[i];

                if (!piece.isInteractable || !MapManager.Instance.interactIndexes.Contains(i)) continue;

                RectTransform rt = piece.GetComponent<RectTransform>();
                if (RectTransformUtility.RectangleContainsScreenPoint(rt, mousePos, null))
                {
                    // 交换 slot 下的图片
                    StartCoroutine(SwapPieces(draggingPiece, piece));
                    swapped = true;
                    break;
                }
            }

            if (!swapped)
            {
                // 回到原 slot
                StartCoroutine(MoveToSlot(draggingPiece, originalParent, originalAnchoredPos));
            }

            draggingPiece = null;
            draggingIndex = -1;
        }
    }

    IEnumerator SwapPieces(CheckCorrect a, CheckCorrect b)
    {
        Transform slotA = a.transform.parent;
        Transform slotB = b.transform.parent;

        // 暂存位置
        Vector2 posA = a.GetComponent<RectTransform>().anchoredPosition;
        Vector2 posB = b.GetComponent<RectTransform>().anchoredPosition;

        // 交换父物体
        a.transform.SetParent(slotB, false);
        b.transform.SetParent(slotA, false);

        // 重置锚点位置
        a.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        b.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

        // 可选：简单动画
        float t = 0;
        float duration = 0.15f;
        RectTransform rtA = a.GetComponent<RectTransform>();
        RectTransform rtB = b.GetComponent<RectTransform>();
        while (t < duration)
        {
            t += Time.deltaTime;
            float k = t / duration;
            rtA.anchoredPosition = Vector2.Lerp(Vector2.zero, Vector2.zero, k); // 本质位置已经在 slot 下
            rtB.anchoredPosition = Vector2.Lerp(Vector2.zero, Vector2.zero, k);
            yield return null;
        }

        // 检查正确性
        MapManager.Instance.IsPieceCorrect(a);
        MapManager.Instance.IsPieceCorrect(b);
    }

    IEnumerator MoveToSlot(CheckCorrect piece, Transform slot, Vector2 anchoredPos)
    {
        piece.transform.SetParent(slot, false);
        piece.GetComponent<RectTransform>().anchoredPosition = anchoredPos;
        yield return null;

        MapManager.Instance.IsPieceCorrect(piece);
    }

    public void ResetDraggingState()
    {
        isDragging = false;
        draggingPiece = null;
        draggingIndex = -1;
    }
}
