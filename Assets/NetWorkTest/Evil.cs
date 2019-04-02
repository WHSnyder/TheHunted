using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Evil : MonoBehaviour {


    float ButtonCooler = 0.0f;
    int ButtonCount = 0;
    int speed = 3;
    Vector3 pos;
    GameObject mom;

    // Start is called before the first frame update
    void Start(){

        // rb = gameObject.GetComponent<Rigidbody>();

            
           /* pos = this.transform.position;

            mainCam = GameObject.Find("Main Camera");
            mainCam.transform.Translate(mainCam.transform.up * -.5f);*/

    }


    // Update is called once per frame
    void Update()
    {

        transform.Rotate(Vector3.up, 45 * Time.deltaTime);
        transform.Rotate(Vector3.forward, 45 * Time.deltaTime);

        pos = transform.parent.position;
        pos.y = pos.y + .2f * Mathf.Sin((Time.fixedTime) * Mathf.PI * .5f);
        transform.position = pos;

        //Speed control
        /*if (Input.GetKeyDown("w"))
        {

            if (ButtonCooler > 0.0f && ButtonCount == 1)
            {
                print("Double tap!");
                speed = 5;
            }
            else
            {
                ButtonCooler = 0.5f;
                ButtonCount += 1;
                speed = 3;
            }
        }

        if (ButtonCooler > 0.0f)
        {

            ButtonCooler -= 1.0f * Time.deltaTime;

        }
        else
        {
            ButtonCount = 0;
        }*/


        /*float x = Input.GetAxis("Horizontal") * Time.deltaTime;
        float ver = Input.GetAxis("Vertical") * Time.deltaTime * speed;

        mainCam.transform.RotateAround(this.transform.position, Vector3.up, x * 60);
        mainCam.transform.position += mainCam.transform.forward * ver;

        transform.Rotate(0, x, 0);
        pos += mainCam.transform.forward * ver;
        */
    }
}
