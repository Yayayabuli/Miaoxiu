using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class GlowBorder : MonoBehaviour
{
    public Image glowImage;
    public float glowDuration = 0.8f;
    public float maxAlpha = 0.8f;

    private Action onGlowFinished;

    void Awake()
    {
        glowImage.color = new Color(1f, 0.84f, 0.4f, 0f);
    }

    public void PlayGlow(Action callback = null)
    {
        onGlowFinished = callback;
        StopAllCoroutines();
        StartCoroutine(GlowRoutine());
    }

    IEnumerator GlowRoutine()
    {
        float t = 0f;

        while (t < glowDuration)
        {
            t += Time.deltaTime;
            float a = Mathf.Sin(t / glowDuration * Mathf.PI) * maxAlpha;
            glowImage.color = new Color(1f, 0.84f, 0.4f, a);
            yield return null;
        }

        glowImage.color = new Color(1f, 0.84f, 0.4f, 0f);
        onGlowFinished?.Invoke();
    }
}