using UnityEngine;
using TMPro;
using System;

public class TimerManager : MonoBehaviour
{
    [Header("时间设置（秒）")]
    public float totalTime = 180f;

    [Header("UI")]
    public TMP_Text timerText;

    public bool isRunning { get; private set; }

    float currentTime;

    // 超时事件
    public Action OnTimeUp;

    void Start()
    {
        StartTimer();
    }

    void Update()
    {
        if (!isRunning) return;

        currentTime -= Time.deltaTime;

        if (currentTime <= 0)
        {
            currentTime = 0;
            UpdateUI();
            TimeUp();
            return;
        }

        UpdateUI();
    }

    void UpdateUI()
    {
        int min = Mathf.FloorToInt(currentTime / 60);
        int sec = Mathf.FloorToInt(currentTime % 60);
        timerText.text = $"{min:00}:{sec:00}";
    }

    public void StartTimer()
    {
        currentTime = totalTime;
        isRunning = true;
        UpdateUI();
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    void TimeUp()
    {
        isRunning = false;
        OnTimeUp?.Invoke();
    }
}