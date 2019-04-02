using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class EnemyController : MonoBehaviour
{
    Vector3 startPos = new Vector3();
    Vector3 newPos = new Vector3();
    float rand;

    public GameController overlord;

    void Update()
    {
        transform.Rotate(Vector3.up, 45 * Time.deltaTime);
        transform.Rotate(Vector3.forward, 45 * Time.deltaTime);

        newPos = startPos;
        newPos.y += Mathf.Sin((Time.fixedTime + rand) * Mathf.PI * .5f) * 1f;

        transform.position = newPos;
    }


    private void Start()
    {
        startPos = transform.position;
        rand = Random.Range(2, 5);
    }

    private void OnCollisionEnter(Collision other)
    {

        if (other.gameObject.CompareTag("Player2"))
        {
            Explode();
            overlord.Lose();
        }

        if (other.gameObject.CompareTag("Bullet"))
        {
            Explode();
            overlord.UpdateScore();
        }
    }


    private void Explode()
    {
        ParticleSystem explosion = GetComponent<ParticleSystem>();
        explosion.Play();
        Destroy(gameObject, explosion.main.duration);
    }
}


