using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FLSource : MonoBehaviour {

    private Light source, bounce;
    private bool on = false;
    private float power = 10.0f;
    private bool hasPower = true;

    int layerMask;

    Vector3 shot, reflection;
    RaycastHit hit;


    // Start is called before the first frame update
    void Start(){
        layerMask = 1 << 8;
        layerMask = ~layerMask;

        source  = transform.GetChild(0).gameObject.GetComponent<Light>();
        bounce = transform.GetChild(1).gameObject.GetComponent<Light>();

        bounce.enabled = false;
        source.enabled = false;
    }



    // Update is called once per frame
    void Update(){

        if (Input.GetMouseButtonDown(0)){
            source.enabled = true;
            on = true;


        }

        if (Input.GetMouseButtonUp(0)){
            source.enabled = false;
            bounce.enabled = false;
            on = false;
        }

        if ((on) && (hasPower)){
            // Does the ray intersect any objects excluding the player layer
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity,layerMask)){

                //print("Shot");

                Vector3 forward = transform.TransformDirection(Vector3.forward) * 10;
                Debug.DrawRay(transform.position, forward, Color.green);

                shot = hit.point - this.transform.position;
                reflection = Vector3.Reflect(shot, hit.normal);

                bounce.transform.SetPositionAndRotation(hit.point, Quaternion.LookRotation(reflection));
                bounce.enabled = true;

                Debug.DrawRay(hit.point, reflection*10, Color.red);

                power -= 1f; 
                if (power < 0) {
                    hasPower = false;
                }
                else {
                    hasPower = true;
                }
               // Debug.Log(power);


            }
        }
    }
}
