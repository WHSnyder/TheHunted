using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioTest : MonoBehaviour
{

    GameObject source;
    AudioSource one;
    public AudioClip stepSound;
    float timer;
    int index = 0;

    Vector3[] positions = new Vector3[4];


    void Start()
    {
        source = GameObject.Find("AudioTest");
        one = source.GetComponent<AudioSource>();
        //one.clip = stepSound;
        source.AddComponent<AudioReverbFilter>().reverbPreset = AudioReverbPreset.StoneCorridor;
        //one.spatialBlend = 1;
        //one.spread = 180;
        //one.panStereo = 1;



        one.rolloffMode = AudioRolloffMode.Custom;

        positions[0] = Vector3.zero;
        positions[1] = new Vector3(9, 0, -12);
        positions[2] = new Vector3(9, 0, 9);
        positions[3] = new Vector3(17,0,1);
    }


    void Update(){

        timer += Time.deltaTime;

        if (timer > .5){
            timer = 0;
            //source.transform.position = positions[index++ % 4];
            one.PlayOneShot(stepSound, 1);
        }
    }
}
