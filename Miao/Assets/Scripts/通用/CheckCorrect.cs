using UnityEngine;

public class CheckCorrect : MonoBehaviour
{
    [Header("初始缩放")]
    public float initialScale = 0.8f; // 每张图片不同

    [Header("容错范围 ±")]
    public float tolerance = 0.1f;

    [Header("是否为可交互图片")]
    public bool isInteractable = true;

    private RectTransform rt;

    // 正确的缩放大小
    private const float correctScale = 4f;

    void Awake()
    {
        rt = GetComponent<RectTransform>();
        rt.localScale = Vector3.one * initialScale;
    }

    /// <summary>
    /// 检查当前图片是否达到正确缩放
    /// </summary>
    public bool CheckNow()
    {
        float s = rt.localScale.x;
        return Mathf.Abs(s - correctScale) <= tolerance;
    }
}
