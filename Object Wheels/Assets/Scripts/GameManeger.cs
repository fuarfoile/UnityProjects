using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManeger : MonoBehaviour {
    public static GameManeger instance { get; private set; }

    public enum gameStates
    {
        MAIN_MENU,
        GAME_FREE,
        GAME_TIME_TRIAL,
        STATISTIC,
        SETTINGS,
        EXIT
    }

    public DebugText debugText;
    public Text statisticMaxScoreTimeTrial;

    public gameStates gameState = gameStates.MAIN_MENU;

    public int maxScoreTimeTrial = 0;

    RuntimePlatform platform = Application.platform;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        loadData();
    }

    void Update()
    {
        if (platform == RuntimePlatform.Android || platform == RuntimePlatform.IPhonePlayer)
        {
            if (Input.touchCount > 0)
            {
                if (Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    checkTouch(Input.GetTouch(0).position);
                }
            }
        }
        else if (platform == RuntimePlatform.WindowsEditor)
        {
            if (Input.GetMouseButtonDown(0))
            {
                checkTouch(Input.mousePosition);
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            escape();
        }
    }

    public void escape()
    {
        if (gameState != gameStates.MAIN_MENU)
        {
            switch (gameState)
            {
                case gameStates.STATISTIC:
                    GameObject.Find("Statistic").GetComponent<Animator>().SetTrigger("hide");
                    gameState = gameStates.MAIN_MENU;
                    break;
                case gameStates.SETTINGS:
                    break;
                default:
                    gameState = gameStates.MAIN_MENU;
                    SceneManager.LoadScene(Data.instance.scenes[(int)gameStates.MAIN_MENU], LoadSceneMode.Single);
                    break;
            }
        }
        else
        {
            Application.Quit();
        }
        Debug.Log("Pressed escape");
    }

    void checkTouch(Vector2 pos)
    {
        Vector3 wp = Camera.main.ScreenToWorldPoint(pos);
        Vector2 touchPos = new Vector2(wp.x, wp.y);
        Collider2D hit = Physics2D.OverlapPoint(touchPos);

        if (hit)
        {
            hit.transform.gameObject.SendMessage("clicked", 0, SendMessageOptions.DontRequireReceiver);
        }
    }

    public void loadData()
    {
        debugText.println("loadData entry"); //Debug
        maxScoreTimeTrial = PlayerPrefs.GetInt("maxScoreTimeTrial", 0);
        statisticMaxScoreTimeTrial.text = maxScoreTimeTrial.ToString();
        debugText.println("loadData, maxScoreTimeTrial: " + maxScoreTimeTrial.ToString()); //Debug
    }
    public void saveData()
    {
        debugText.println("saveData entry"); //Debug
        if (gameState == gameStates.GAME_TIME_TRIAL)
        {
            if (LevelManeger.instance.playerScore > maxScoreTimeTrial)
            {
                maxScoreTimeTrial = LevelManeger.instance.playerScore;
                PlayerPrefs.SetInt("maxScoreTimeTrial", maxScoreTimeTrial);
                debugText.println("saveData, maxScoreTimeTrial: " + maxScoreTimeTrial.ToString()); //Debug
            }
        }
    }
}