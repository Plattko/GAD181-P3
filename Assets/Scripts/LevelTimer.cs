using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelTimer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timeText;
    public float time = 0; // Measured in seconds
    public static bool isTimerRunning = true;

    private void Awake()
    {
        isTimerRunning = true;
    }

    private void Update()
    {
        if (isTimerRunning)
        {
            time += Time.deltaTime;

            int minutes = Mathf.FloorToInt(time / 60);
            int seconds = Mathf.FloorToInt(time % 60);

            string formattedTime = string.Format("{0:00}:{1:00}", minutes, seconds);

            timeText.text = formattedTime;
        }
    }
}
