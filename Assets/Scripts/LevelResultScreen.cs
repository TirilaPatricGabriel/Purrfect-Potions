using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class LevelResultScreen : MonoBehaviour
{
    public TextMeshProUGUI gradeText;
    public Image[] stars;

    public void ShowGradeAndStars(float grade)
    {
        // Display the grade in the GradeText
        if (gradeText != null)
        {
            gradeText.text = "Grade: " + grade.ToString("F1");
        }
        else
        {
            Debug.LogError("GradeText is not assigned!");
        }

        //gradeText.text = "Grade: " + grade.ToString("F1");

        int starsToShow = CalculateStars(grade);

        for (int i = 0; i < stars.Length; i++)
        {
            stars[i].enabled = i < starsToShow;
        }
    }

    private int CalculateStars(float grade)
    {
        if (grade >= 9.0f) return 5;
        else if (grade >= 7.0f) return 4;
        else if (grade >= 5.0f) return 3;
        else if (grade >= 3.0f) return 2;
        else if (grade >= 0f) return 1;
        else return 0;
    }

}
