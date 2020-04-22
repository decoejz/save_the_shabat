using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        PlayerPrefs.SetInt("Pontos", 0);
        PlayerPrefs.SetInt("Vidas", 3);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Start_Game()
    {
        SceneManager.LoadScene("Transition");
    }
}
