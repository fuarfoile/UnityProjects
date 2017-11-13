using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelObject : MonoBehaviour {

    public int id = 0;
    public SpriteRenderer spriteRenderer;

	void Start () {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void clicked()
    {
        if (!transform.parent.GetComponent<Wheel>().inAnimation)
        {
            transform.parent.GetComponent<Wheel>().objectClicked(spriteRenderer.sprite.name);
        }
    }
}