using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour {

    private float RotationSpeed = 5.0f;


    private AudioPlanner planner;
    private GameObject sourceOne, sourceTwo;
    private AudioSource one, two;


    private int maxAudioDist = 90;
    private Vector3 toPlayer;
    private Transform playerTransform;
    private float magToPlayer;


    private void Start()
    {

        planner = GameObject.Find("Main Camera").GetComponent<AudioPlanner>();

        sourceOne = Instantiate(Resources.Load<GameObject>("AudioEmitter")) as GameObject;
        sourceTwo = Instantiate(Resources.Load<GameObject>("AudioEmitter")) as GameObject;

        one = sourceOne.GetComponent<AudioSource>();
        two = sourceTwo.GetComponent<AudioSource>();

        one.clip = Resources.Load<AudioClip>("hum");

        one.playOnAwake = true;

        one.loop = true;

        playerTransform = GameObject.Find("Player").gameObject.transform;

        //planner.requestSearch(new AudioData(0, 0, gameObject, sourceOne, sourceTwo));
    }




    // Update is called once per frame
    void Update(){
        transform.Rotate(Vector3.up * (RotationSpeed * Time.deltaTime));

        one.volume = 1.0f;
    }

    private void OnCollisionEnter(Collision collision){
        if (collision.gameObject.tag == "Player") {
            Debug.Log("hit key");
        }
    }





}
