using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public Vector3 firstPlayerPosition;
    public Vector3 secondPlayerPosition;
    public float goldEarned;
    public List<string> unlockedAchievements = new List<string>();

    public void Start()
    {
        goldEarned = 0;
        firstPlayerPosition = new Vector3(34.8f, 4.2464f, -25.91f);
        secondPlayerPosition = new Vector3(43.8f, 4.2464f, -0.1f);
        unlockedAchievements = new List<string>();
    }
}
