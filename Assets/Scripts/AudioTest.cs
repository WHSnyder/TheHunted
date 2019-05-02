using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioTest : MonoBehaviour
{
    // Start is called before the first frame update
    private float soundIncr = 0;
    public GameObject stepSourceOne;
    public GameObject stepSourceTwo;

    private Vector3 location;

    public AudioPlanner planner;



    void Update(){

        soundIncr += Time.deltaTime;

        if (soundIncr > .5){

            soundIncr = 0;

            stepSourceOne.GetComponent<AudioSource>().Play();
            stepSourceTwo.GetComponent<AudioSource>().Play();
        }
    }
}
