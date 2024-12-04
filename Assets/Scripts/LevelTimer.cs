using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelTimer : MonoBehaviour
{
    [SerializeField] float levelDuration = 300f; // level duration in seconds
    private float timer;
    private bool isTimeUp = false;

    [SerializeField] GameObject player1;
    [SerializeField] GameObject player2;

    [SerializeField] TextMeshProUGUI timerText;

    [SerializeField] LevelResultScreen levelResultScreen;
    [SerializeField] GameObject levelCompletedScreen;

    [SerializeField] float hardcodedGrade = 8.5f;


    void Start()
    {
        timer = levelDuration;
        UpdateTimerText();

        if (levelCompletedScreen != null)
        {
            levelCompletedScreen.SetActive(false);
        }
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

        if (player1 != null)
        {
            player1.GetComponent<PlayerMovement>().enabled = false;
        }
        if (player2 != null)
        {
            player2.GetComponent<PlayerMovement2>().enabled = false;
        }

        Debug.Log("Time's up!");

        ShowLevelResult();
    }

    void UpdateTimerText()
    {
        int minutes = Mathf.FloorToInt(timer / 60f);
        int seconds = Mathf.FloorToInt(timer % 60f);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    void ShowLevelResult()
    {
        float grade = hardcodedGrade;

        if (levelCompletedScreen != null)
        {
            levelCompletedScreen.SetActive(true);
        }

        if(levelResultScreen != null)
        {
            levelResultScreen.ShowGradeAndStars(grade);
        }
        else
        {
            Debug.LogError("LevelResultScreen is not assigned!");
        }
    }
}

