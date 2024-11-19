using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelTimer : MonoBehaviour
{
    [SerializeField] float levelDuration = 300f; // level duration in seconds
    private float timer;
    private bool isTimeUp = false;

    [SerializeField] GameObject player;
    [SerializeField] TextMeshProUGUI timerText;

    void Start()
    {
        timer = levelDuration;
        UpdateTimerText();
    }

    void Update()
    {
        if (!isTimeUp)
        {
            timer -= Time.deltaTime;

            if (timer <= 0f)
            {
                timer = 0f;
                TimerFinished();
            }
        }

        UpdateTimerText();
    }

    void TimerFinished()
    {
        isTimeUp = true;

        if (player != null)
        {
            player.GetComponent<PlayerMovement>().enabled = false;
        }

        Debug.Log("Time's up!");
    }

    void UpdateTimerText()
    {
        int minutes = Mathf.FloorToInt(timer / 60f);
        int seconds = Mathf.FloorToInt(timer % 60f);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}

