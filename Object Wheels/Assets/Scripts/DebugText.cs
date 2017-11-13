using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugText : MonoBehaviour {

    public int maxLines = 10;

    private Queue<string> queue = new Queue<string>();
    private Text text;

    void Awake () {
        text = GetComponent<Text>();
        queue.Enqueue(text.text);
    }

    public void println(string line)
    {
        if (!gameObject.activeSelf)
        {
            return;
        }

        queue.Enqueue(line);
        if (queue.Count > maxLines)
        {
            queue.Dequeue();
        }

        text.text = "";
        foreach (string strLine in queue.ToArray())
        {
            text.text += strLine + "\n";
        }
    }
}