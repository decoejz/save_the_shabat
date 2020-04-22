using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class portoes : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (GameObject.Find("Player").GetComponent<PlayerController>().getVela &&
            GameObject.Find("Player").GetComponent<PlayerController>().getVinho &&
            GameObject.Find("Player").GetComponent<PlayerController>().getPao)
        {
            GetComponent<TilemapCollider2D>().enabled = false;
            GetComponent<TilemapRenderer>().enabled = false;
        }
    }
}
