using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class FLSource : MonoBehaviour{

    //public VolumetricLight source;
    public Light source;
    private bool on = false;
    private float power = 200.0f;
    private bool hasPower = true;
    private int batteryCount;
    private int tempCount; 
    public Text powerText; 

    
    Vector3 shot, reflection;
    RaycastHit hit;


    // Start is called before the first frame update
    void Start(){

        batteryCount = GameObject.FindGameObjectsWithTag("Battery").Length; 


        source = GetComponent<Light>(); 
        //bounce = transform.GetChild(1).gameObject.GetComponent<Light>();
        
        //bounce.enabled = false;
        source.enabled = false;
    }



    // Update is called once per frame
    void Update(){

        tempCount = GameObject.FindGameObjectsWithTag("Battery").Length; 
        if (tempCount < batteryCount) {
            power += 100; 
        }
        batteryCount = tempCount;

        //change the text 
        setPowerText();

        if (Input.GetKeyDown(KeyCode.F)){
            source.enabled = !source.enabled;
            //bounce.enabled = !bounce.enabled;
            on = !on;
        }

        if (power <= 0.0f){
            source.enabled = false;
            //bounce.enabled = false;
            on = false;
        }


        if (on && (power > 0.0f)){
            power -= 0.05f;
        } 

        void setPowerText() {
            powerText.text = "Power: " + power;
        }






        // else
        //{
        //    source.enabled = false;
        //    bounce.enabled = false;
        //    on = false;
        //}

        // if (Input.GetKey(KeyCode.F)){
        //  source.enabled = false;
        //    bounce.enabled = false;
        //     on = false;
        // }

        //if ((on) && (hasPower)){
        // Does the ray intersect any objects excluding the player layer
        //if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity,layerMask)){

        //    //print("Shot");

        //    Vector3 forward = transform.TransformDirection(Vector3.forward) * 10;
        //    Debug.DrawRay(transform.position, forward, Color.green);

        //    shot = hit.point - this.transform.position;
        //    reflection = Vector3.Reflect(shot, hit.normal);

        //    bounce.transform.SetPositionAndRotation(hit.point, Quaternion.LookRotation(reflection));
        //    //bounce.enabled = true;

        //    Debug.DrawRay(hit.point, reflection*10, Color.red);

        //    power -= 1f; 
        //    if (power < 0) {
        //        hasPower = false;
        //    }
        //    else {
        //        hasPower = true;
        //    }
        //   // Debug.Log(power);


        //}
        //    }
        //}
    }
}
