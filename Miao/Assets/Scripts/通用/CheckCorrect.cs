using UnityEngine;

public class CheckCorrect : MonoBehaviour
{
    [Header("初始缩放")]
    public float initialScale = 0.8f;

    [Header("容错范围 ±")]
    public float tolerance = 0.1f;

    [Header("是否为可交互图片")]
    public bool isInteractable = true;

    private RectTransform rt;

    void Awake()
    {
        rt = GetComponent<RectTransform>();
        rt.localScale = Vector3.one * initialScale;
    }

    public bool CheckNow()
    {
        float s = rt.localScale.x;
        float min = 1f - tolerance;
        float max = 1f + tolerance;

        bool ok = (s >= min && s <= max);
        return ok;
    }
}
