using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class MouseInteraction : MonoBehaviour
{
    [Header("缩放速度")]
    public float zoomSpeed = 0.1f;

    [Header("操作的拼图图片数组")]
    public CheckCorrect[] allPieces;

    [Header("Canvas 引用（用于坐标转换）")]
    public Canvas canvas;

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

        // 1️⃣ 找到鼠标下的可交互图片
        CheckCorrect currentPiece = null;
        foreach (var piece in allPieces)
        {
            if (!piece.isInteractable) continue;

            RectTransform rt = piece.GetComponent<RectTransform>();
            if (RectTransformUtility.RectangleContainsScreenPoint(rt, mousePos))
            {
                currentPiece = piece;
                break;
            }
        }

        // 2️⃣ 滚轮缩放
        if (currentPiece != null && !isDragging)
        {
            float scroll = Input.mouseScrollDelta.y;
            if (Mathf.Abs(scroll) > 0.01f)
            {
                RectTransform rt = currentPiece.GetComponent<RectTransform>();
                float newScale = rt.localScale.x + scroll * zoomSpeed;
                newScale = Mathf.Clamp(newScale, 0.3f, 3f);
                rt.localScale = Vector3.one * newScale;

                MapManager.Instance.CheckSingle(currentPiece);
            }
        }

        // 3️⃣ 拖拽逻辑
        if (Input.GetMouseButtonDown(0) && currentPiece != null)
        {
            isDragging = true;
            draggingPiece = currentPiece;
            draggingIndex = System.Array.IndexOf(allPieces, draggingPiece);
            originalPosition = draggingPiece.GetComponent<RectTransform>().anchoredPosition;
        }

        if (isDragging)
        {
            // 跟随鼠标
            RectTransform rt = draggingPiece.GetComponent<RectTransform>();
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                mousePos, canvas.worldCamera, out localPoint);
            rt.anchoredPosition = localPoint;
        }

        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            isDragging = false;
            // 检查是否放在其他可交互图片上
            bool swapped = false;
            for (int i = 0; i < allPieces.Length; i++)
            {
                if (i == draggingIndex) continue;
                if (!allPieces[i].isInteractable) continue;

                RectTransform rt = allPieces[i].GetComponent<RectTransform>();
                if (RectTransformUtility.RectangleContainsScreenPoint(rt, mousePos))
                {
                    StartCoroutine(SwapPieces(draggingPiece, allPieces[i]));
                    swapped = true;
                    break;
                }
            }

            if (!swapped)
            {
                // 回到原位置
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

    // 回弹
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
}
