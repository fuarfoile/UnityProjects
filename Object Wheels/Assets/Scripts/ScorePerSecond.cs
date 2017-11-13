using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScorePerSecond : MonoBehaviour {

    public Timer timer;

    private Text text;

	void Start () {
        text = GetComponent<Text>();
    }
	
	void Update () {
        text.text = "S/T\n " + (LevelManeger.instance.playerScore / timer.time).ToString("0.0000");
    }
}