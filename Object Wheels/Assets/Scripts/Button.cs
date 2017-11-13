using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Button : MonoBehaviour {

    public GameManeger.gameStates state;
    public bool isSceneLoader = false;

    public void clicked()
    {
        Debug.Log("clicked, " + state); //Debug
        GetComponent<Animator>().SetTrigger("Normal");
        if (isSceneLoader)
        {
            GameManeger.instance.gameState = state;
            GameManeger.instance.debugText.println("loadScene: " + Data.instance.scenes[(int)state].ToString()); //Debug
            SceneManager.LoadScene(Data.instance.scenes[(int) state], LoadSceneMode.Single);
        }

        if (state == GameManeger.gameStates.EXIT)
        {
            Debug.Log("exit button, " + state); //Debug
            GameManeger.instance.escape();
        }

        if (state == GameManeger.gameStates.STATISTIC)
        {
            GameManeger.instance.gameState = state;
            GameObject.Find("Statistic").GetComponent<Animator>().SetTrigger("show");
            return;
        }
    }
}