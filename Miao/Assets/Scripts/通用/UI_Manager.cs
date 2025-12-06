using UnityEngine;

public class UI_Manager : MonoBehaviour
{
    public GameObject successUI;
    public GameObject unfinishedUI;
    public GameObject partialErrorUI;

    public void ShowSuccess()
    {
        successUI.SetActive(true);
    }

    public void ShowUnfinished()
    {
        unfinishedUI.SetActive(true);
    }

    public void ShowPartialError()
    {
        partialErrorUI.SetActive(true);
    }
}
