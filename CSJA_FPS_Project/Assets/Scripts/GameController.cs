using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController instance;


    public GameObject gameOver;


    void Start()
    {
        instance = this;
    }


    void Update()
    {

    }

    public void ShowGameOver()
    {
        gameOver.SetActive(true);
    }
}
