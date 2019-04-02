using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Exp : NetworkBehaviour
{

    float ButtonCooler = 0.0f;
    int ButtonCount = 0;
    int speed = 3;
    float rand;
    GameObject mainCam;

    private Rigidbody rb;


    // Start is called before the first frame update
    void Start()
    {
        if (isLocalPlayer)
        {
            mainCam = GameObject.Find("Main Camera");
            mainCam.transform.parent = this.transform;

            rb = GetComponent<Rigidbody>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer) return;
        //Speed control
        if (Input.GetKeyDown("w"))
        {

            if (ButtonCooler > 0.0f && ButtonCount == 1)
            {
                print("Double tap!");
                speed = 3;
            }
            else
            {
                ButtonCooler = 0.5f;
                ButtonCount += 1;
                speed = 1;
            }
        }

        if (ButtonCooler > 0.0f)
        {
            ButtonCooler -= 1.0f * Time.deltaTime;
        }

        else
        {
            ButtonCount = 0;
        }

        if (Input.GetKeyDown("space"))
        {
            rb.AddForce(Vector3.up * 250);
        }


        float x = Input.GetAxis("Horizontal") * Time.deltaTime * 150.0f;
        float z = Input.GetAxis("Vertical") * Time.deltaTime * 2;


        transform.Rotate(0, x, 0);
        transform.Translate(0, 0, z*speed);
    }
}
