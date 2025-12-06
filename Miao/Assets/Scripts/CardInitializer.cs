using UnityEngine;

public class CardInitializer : MonoBehaviour
{
    public RectTransform[] panels;

    void Start()
    {
        foreach (Transform child in transform)
        {
            var drag = child.GetComponent<UIDrag>();
            drag.panels = panels;
        }
    }
}