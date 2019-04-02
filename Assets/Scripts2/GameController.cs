using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GameController : MonoBehaviour
{

    private int score;

    public int victoryScore;

    public Text endText2;
    public Text scoreText2;


    //public CanvasGroup canvas;



    private void Start()
    {
        score = 0;

        print("Score: " + scoreText2.ToString());
        print("End: " + endText2.ToString());

        endText2.text = "Nothing yet...";
        scoreText2.text = score.ToString();
    }


    private void Update()
    {
        print("Score: " + scoreText2.ToString());
        print("End: " + endText2.ToString());
    }


    public void UpdateScore()
    {
        score++;
        this.scoreText2.text = "Count: " + score.ToString();

        print("New score is: " + score);

        if (score >= victoryScore)
        {
            this.endText2.text = "Victory!";
            Invoke("EndGame", 2f);
        }
    }


    public void Lose()
    {
        //this.endText2.text = "Defeat.";
        Invoke("EndGame", 2f);
    }


    public void EndGame()
    {

        /*while (canvas.alpha > 0)
        {
            canvas.alpha -= Time.deltaTime / 100;
        }*/

        SceneManager.LoadScene(0);
    }
}
