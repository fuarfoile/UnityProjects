using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Wheel : MonoBehaviour {

    public Sprite[] wheelObjectSprites;
    public GameObject[] wheelObjects;

    public bool isPlayerWheel;
    public bool inAnimation = false;

    void Start () {
        if (isPlayerWheel)
        {
            instantiate(Data.instance.wheelObjectSprites);
            LevelManeger.instance.playableWheel.GetComponent<Wheel>().instantiate(wheelObjectSprites); 
        }
	}
	
	void Update () {
		
	}

    public void instantiate(Sprite[] goSprites)
    {
        wheelObjectSprites = goSprites.Clone() as Sprite[];

        if (isPlayerWheel)
        {
            arrayShuffle(wheelObjectSprites, 0, wheelObjectSprites.Length);
        }

        int objectsCount = LevelManeger.instance.wheelObjectsCount;
        randomizeArray(wheelObjectSprites, objectsCount);

        wheelObjects = new GameObject[Data.MAX_OBJECTS];
        for (int i = 0; i < wheelObjects.Length; i++)
        {
            wheelObjects[i] = Instantiate(Data.instance.wheelObjectPrefab,
                gameObject.transform.position + 1.5f * new Vector3(Mathf.Sin(2 * Mathf.PI * i / objectsCount), Mathf.Cos(2 * Mathf.PI * i / objectsCount), -0.05f),
                Quaternion.identity,
                gameObject.transform) as GameObject;
            wheelObjects[i].GetComponent<WheelObject>().id = i;
            wheelObjects[i].name = "Wheel object " + i.ToString();
            wheelObjects[i].SetActive(false);
        }
        for (int i = 0; i < objectsCount; i++)
        {
            wheelObjects[i].SetActive(true);
            wheelObjects[i].GetComponent<SpriteRenderer>().sprite = wheelObjectSprites[i];
        }

        if (gameObject.name.Equals("Playable wheel"))
        {
            LevelManeger.instance.nextPlayableWheel.GetComponent<Wheel>().instantiate(wheelObjectSprites);
        }
    }

    public void setObjects() {
        wheelObjectSprites = LevelManeger.instance.playableWheel.GetComponent<Wheel>().wheelObjectSprites.Clone() as Sprite[];

        int objectsCount = LevelManeger.instance.wheelObjectsCount;
        randomizeArray(wheelObjectSprites, objectsCount);

        for (int i = 0; i < objectsCount; i++)
        {
            wheelObjects[i].GetComponent<SpriteRenderer>().sprite = wheelObjectSprites[i];
        }

        setObjectsPos();
    }

    public void setObjectsPos()
    {
        int objectsCount = LevelManeger.instance.wheelObjectsCount;
        for (int i = 0; i < objectsCount; i++)
        {
            wheelObjects[i].transform.position =
                gameObject.transform.position + 1.5f * new Vector3(Mathf.Sin(2 * Mathf.PI * i / objectsCount), Mathf.Cos(2 * Mathf.PI * i / objectsCount), -0.05f);
            wheelObjects[i].SetActive(true);
        }
        for (int i = objectsCount; i < wheelObjects.Length; i++)
        {
            wheelObjects[i].SetActive(false);
        }
    }

    public void animationEnd()
    {
        inAnimation = false;
    }

    public void setNextPlayable ()
    {
        setObjects();
    }

    public void objectClicked(string spriteName)
    {
        bool turnAccepted = false;
        if (isPlayerWheel)
        {
            foreach (SpriteRenderer spriteRenderer in LevelManeger.instance.playableWheel.GetComponentsInChildren<SpriteRenderer>())
            {
                if (spriteRenderer.sprite.name.Equals(spriteName))
                {
                    turnAccepted = true;
                    break;
                }
            }

            if (turnAccepted)
            {
                LevelManeger.instance.turnAccepted();
            }
            else
            {
                LevelManeger.instance.turnFailed();
            }
        }
    }

    private void randomizeArray(Object[] array, int length)
    {
        length = Mathf.Min(length, array.Length);
        if (LevelManeger.instance.wheelObjectscCountUp)
        {
            LevelManeger.instance.wheelObjectscCountUp = false;
            length--;
        }

        arrayShuffle(array, 0, length);
        arrayShuffle(array, length, array.Length - length);

        for (int i = 1; i < length; i++)
        {
            int r = array.Length - i;
            Object tmp = array[i];
            array[i] = array[r];
            array[r] = tmp;
        }

        arrayShuffle(array, 0, length);
    }

    private void arrayShuffle(Object[] array, int start, int length)
    {
        length = Mathf.Min(length, array.Length - start);
        start = Mathf.Max(start, 0);

        for (int i = length + start - 1; i > start; i--)
        {
            int r = Random.Range(start, length + start - 1);
            Object tmp = array[i];
            array[i] = array[r];
            array[r] = tmp;
        }
    }
}