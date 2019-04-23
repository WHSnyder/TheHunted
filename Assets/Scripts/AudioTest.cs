using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioTest : MonoBehaviour
{
    // Start is called before the first frame update
    private float soundIncr = 0;
    private AudioSource source;

    void Start()
    {
        source = this.GetComponent<AudioSource>();
        source.Play();  
    }

    // Update is called once per frame
    void Update()
    {
       /*soundIncr += Time.deltaTime;

       if (soundIncr > .5)
        {
            soundIncr = 0;
            source.Play();
        }*/
    }
}
