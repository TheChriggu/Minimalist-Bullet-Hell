using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    public Text scoreWidget;
    public int score = 0;
    public void AddPoints(int points)
    {
        score += points;
        scoreWidget.text = score.ToString();
    }

    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }
}
