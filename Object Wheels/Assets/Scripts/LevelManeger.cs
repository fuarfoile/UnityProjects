using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelManeger : MonoBehaviour {
    public static LevelManeger instance { get; private set; }

    public GameObject playerWheel;
    public GameObject playableWheel;
    public GameObject nextPlayableWheel;

    public Text score;

    public Timer timer;

    public int playerScore = 0;
    public int wheelObjectsCount = 5;
    public bool wheelObjectscCountUp = false;

    public bool endGameTrigger = false;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        
    }

    public void turnAccepted()
    {
        if (!endGameTrigger)
        {
            if (SceneManager.GetActiveScene().name.Equals("GameTimeTrial"))
            {
                float timeToAdd = 1.2f + 1.0f / (playerScore / 25.0f + 1.0f);
                timer.time += timeToAdd;

                timer.timerAdd.GetComponent<Text>().text = "+" + timeToAdd.ToString("0.00");
                timer.timerAdd.GetComponent<Animator>().SetTrigger("timeAdd");
                timer.GetComponent<Animator>().SetTrigger("timeAdd");

                int newCount = Mathf.Min(Data.MAX_OBJECTS, Mathf.Max(wheelObjectsCount, playerScore / 25 + 3));
                if (newCount > wheelObjectsCount)
                {
                    wheelObjectscCountUp = true;
                }
                wheelObjectsCount = newCount;
            }

            playerScore++;
            score.text = "Score:\n" + playerScore;

            playerWheel.GetComponent<Animator>().SetTrigger("toBackTrigger");
            playableWheel.GetComponent<Animator>().SetTrigger("gettingTrigger");
            nextPlayableWheel.GetComponent<Animator>().SetTrigger("toFrontTrigger");

            playerWheel.GetComponent<Wheel>().inAnimation = true;
            playableWheel.GetComponent<Wheel>().inAnimation = true;
            nextPlayableWheel.GetComponent<Wheel>().inAnimation = true;

            GameObject go = playerWheel;
            playerWheel = playableWheel;
            playableWheel = nextPlayableWheel;
            nextPlayableWheel = go;

            playerWheel.GetComponent<Wheel>().isPlayerWheel = true;
            playerWheel.name = "playerWheel";
            playableWheel.GetComponent<Wheel>().isPlayerWheel = false;
            playableWheel.name = "playableWheel";
            nextPlayableWheel.GetComponent<Wheel>().isPlayerWheel = false;
            nextPlayableWheel.name = "nextPlayableWheel";
        }
    }

    public void turnFailed()
    {
        if (!endGameTrigger)
        {
            if (SceneManager.GetActiveScene().name.Equals("GameTimeTrial"))
            {
                timer.time /= 2.0f;
                timer.timerAdd.GetComponent<Text>().text = "-" + timer.time.ToString("0.00");
                timer.timerAdd.GetComponent<Animator>().SetTrigger("timeMinus");
                timer.GetComponent<Animator>().SetTrigger("timeMinus");
            }
        }
    }
}