using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimerUI : MonoBehaviour
{

    public TMP_Text timerText;
    public int time = 5;

    void Start()
    {
        StartCoroutine(coTimer());
    }

    IEnumerator coTimer()
    {
        timerText.text = (time / 60%60).ToString() + ":" + (time % 60).ToString();

        yield return new WaitForSeconds(1.0f);

        if(time > 1)
        {
            time -= 1;
            StartCoroutine(coTimer());
        }
        else
        {
            timerText.text = "Game Start!";
        }
    }
}
