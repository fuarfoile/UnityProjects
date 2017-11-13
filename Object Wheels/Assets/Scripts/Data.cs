using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Data : MonoBehaviour
{
    public static Data instance { get; private set; }

    public const int MAX_OBJECTS = 8;

    public string[] scenes;

    public Sprite[] wheelObjectSprites;
    public GameObject wheelObjectPrefab;

    void Awake ()
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
	}
}