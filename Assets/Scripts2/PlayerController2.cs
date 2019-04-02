using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController2 : MonoBehaviour
{

    public Rigidbody bullet;

    // Start is called before the first frame update
    public float speed;

    private Rigidbody rb;

    public Text scoreText;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }


    private void FixedUpdate()
    {

        bool quit = Input.GetKeyDown("escape") || Input.GetKeyDown("q");

        bool whut = Input.GetKeyDown("f");

        if (whut)
        {
            scoreText.text = Random.Range(0f, 5f).ToString();
        }

        if (quit)
        {
            SceneManager.LoadScene(0);
        }

        float turn = Input.GetAxis("Horizontal");
        float ver = Input.GetAxis("Vertical");

        bool space = Input.GetKeyDown("space");
        bool shot = Input.GetMouseButtonDown(0);

        Vector3 forward = new Vector3(0.0f, 0.0f, ver);

        transform.Rotate(Vector3.up, turn * 10);
        transform.Translate(0f, 0f, ver * -.1f);

        rb.AddForce(forward * speed);

        if (space)
        {
            rb.AddForce(Vector3.up * 50);
        }

        if (shot)
        {
            ShootBullet();
        }

    }


    private void ShootBullet()
    {

        Vector3 spawn = transform.position + transform.forward * -1;

        Rigidbody bulletInstance =
                    Instantiate(bullet, spawn, Quaternion.identity);

        bulletInstance.AddForce(transform.forward * -300);
    }

}
