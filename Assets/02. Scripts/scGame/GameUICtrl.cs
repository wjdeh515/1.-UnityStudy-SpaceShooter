using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUICtrl : MonoBehaviour
{
    public Text textScore;
    private int totScore = 0;

    void Start()
    {
        PlusScore(0);
    }

    public void PlusScore(int score)
    {
        totScore += score;
        textScore.text = "score <color=#ffff00>" + totScore + "</color>";
    }
}
