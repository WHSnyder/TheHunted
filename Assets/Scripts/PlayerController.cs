using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{

    public float speed;

    private Rigidbody rb;
    private int count;

    public Text score1;
    public Text endText1;

    public CanvasGroup canvas;

    public int VictoryScore;


    private void Start(){

        count = 0;
        score1.text = "Count: " + count.ToString();

        rb = GetComponent<Rigidbody>();
    }


    private void FixedUpdate() {

        bool quit = Input.GetKeyDown("escape") || Input.GetKeyDown("q");

        if (quit)
        {
            SceneManager.LoadScene(0);
        }

        float turn = Input.GetAxis("Horizontal");
        float ver = Input.GetAxis("Vertical");

        Vector3 forward = new Vector3 (0.0f, 0.0f, ver);

        transform.Rotate(Vector3.up, turn * 10);
        transform.Translate(0f, 0f, ver * -.1f);

        rb.AddForce(forward * speed);
    }


   


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PickUp"))
        {
            other.gameObject.SetActive(false);

            count = count + 1;
            SetCountText(count);
        }
    }



    private void SetCountText(int newCount)
    {
        if (newCount == VictoryScore)
        {
            endText1.text = "Victory :)";

            Invoke("EndThisGame",2);

        }
        else
        {
            score1.text = "Count: " + newCount.ToString();
        }
    }

    private void EndThisGame()
    {
    
        while (canvas.alpha > 0)
        {
            canvas.alpha -= Time.deltaTime / 100;
            print("Alpha = " + canvas.alpha);
        }

        SceneManager.LoadScene(0);

    }
}
