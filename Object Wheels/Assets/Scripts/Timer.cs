using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour {

    public GameObject timerAdd;
    public float time = 0;
    public bool reverce = false;

    private int hours = 0;
    private int minutes = 0;

    private Text text;

    void Start () {
        text = GetComponent<Text>();
	}
	
	void Update () {
        if (!reverce)
        {
            time += Time.deltaTime;
        }
        else if (!LevelManeger.instance.endGameTrigger)
        {
            time -= Time.deltaTime;

            if (time < 0)
            {
                GameManeger.instance.debugText.println("endGameTrigger"); //Debug
                LevelManeger.instance.endGameTrigger = true;
                GameManeger.instance.saveData();
                GameObject.Find("End game text").GetComponent<Text>().text = "Game\nover";
                time = 0.0f;
            }
        }

        minutes = ((int)time % 3600) / 60;
        hours = (int)time / 3600;

        text.text = (time % 60).ToString("0.00");
        if (minutes > 0 || hours > 0)
        {
            if (time % 60 < 10)
            {
                text.text = "0" + text.text;
            }
            text.text = minutes.ToString() + ":" + text.text;
        }
        if (hours > 0)
        {
            if (minutes < 10)
            {
                text.text = "0" + text.text;
            }
            text.text = hours.ToString() + ":" + text.text;
        }
    }
}