using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTimer : MonoBehaviour, IDataPersistence
{
    [SerializeField] float levelDuration = 180f;
    private float timer = -1;
    private bool isTimeUp = false;

    [SerializeField] GameObject player1;
    [SerializeField] GameObject player2;
    [SerializeField] TextMeshProUGUI timerText;

    public void LoadData(GameData data)
    {
        timer = data.levelDuration;
    }

    public void SaveData(ref GameData data)
    {
        data.levelDuration = timer;
    }


    void Start()
    {
        if (timer <= 0)
        {
            timer = levelDuration;
        }
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

        if (player1 != null)
        {
            player1.GetComponent<PlayerMovement>().enabled = false;
        }

        if (player2 != null)
        {
            player2.GetComponent<PlayerMovement2>().enabled = false;
        }

        Debug.Log("LEVEL ENDED");

        
        UIManager uiManager = FindObjectOfType<UIManager>();
        if (uiManager != null)
        {
            uiManager.EndLevel(); 
        }

        LoadNextScene();
    }

    void LoadNextScene()
    {
        SceneManager.LoadScene("EndingLevelScene");
    }

    void UpdateTimerText()
    {
        int minutes = Mathf.FloorToInt(timer / 60f);
        int seconds = Mathf.FloorToInt(timer % 60f);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
