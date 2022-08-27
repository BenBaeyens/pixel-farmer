using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMover : MonoBehaviour
{

    public float speed = 1f;
    public float height = 0.25f;
    public float dropshadowScaleMultiplier = 100;
    public float xOffset = -4f;

    public GameObject tileParent;
    public GameObject dropshadow;

    private void Update()
    {
        Vector3 pos = tileParent.transform.position;

        float newY = Mathf.Sin(Time.time * speed);

        tileParent.transform.position = new Vector3(pos.x, newY * height, pos.z);
        dropshadow.transform.localScale = new Vector3(1 + newY / dropshadowScaleMultiplier, 1 + newY / dropshadowScaleMultiplier, 1);
    }
}
