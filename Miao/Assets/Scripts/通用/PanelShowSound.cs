using UnityEngine;

public class PanelShowSound : MonoBehaviour
{
    [Header("出现音效")]
    public AudioSource audioSource;
    public AudioClip showClip;

    void OnEnable()
    {
        if (audioSource != null && showClip != null)
        {
            audioSource.PlayOneShot(showClip);
        }
    }
}