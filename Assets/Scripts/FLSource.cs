using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class FLSource : MonoBehaviour{

    public Light source;
    public GameObject beam;

    private bool on = false, hasPower = true;

    private float power = 200.0f;


    private int batteryCount;
    private int tempCount;

    public Text powerText; 

    
    Vector3 shot, reflection;
    RaycastHit hit;


    // Start is called before the first frame update
    void Start(){

        batteryCount = GameObject.FindGameObjectsWithTag("Battery").Length;
        beam = GameObject.Find("light_cone");

        source = GetComponent<Light>(); 
        //bounce = transform.GetChild(1).gameObject.GetComponent<Light>();
        
        //bounce.enabled = false;
        source.enabled = false;
        beam.SetActive(false);
    }



    // Update is called once per frame
    void Update(){

        //tempCount = GameObject.FindGameObjectsWithTag("Battery").Length;
        tempCount = 10;
        if (tempCount < batteryCount) {
            power += 100; 
        }
        batteryCount = tempCount;

        //change the text 
        //setPowerText();

        if (Input.GetKeyDown(KeyCode.F)){
            source.enabled = !source.enabled;
            beam.SetActive(true);
            //bounce.enabled = !bounce.enabled;
            on = !on;
        }

        if (power <= 0.0f){
            source.enabled = false;
            beam.SetActive(false);
            //bounce.enabled = false;
            on = false;

        }


        if (on && (power > 0.0f)){
            power -= 0.05f;
        } 

        //void setPowerText() {
        //    powerText.text = "Power: " + power;
        //}
    }
}
