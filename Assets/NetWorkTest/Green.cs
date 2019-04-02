using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Green : NetworkBehaviour
{


    // Start is called before the first frame update
    void Start()
    {

        // rb = gameObject.GetComponent<Rigidbody>();

        if (isLocalPlayer)
        {
            GameObject.Find("Main Camera").gameObject.transform.parent = this.transform;
        }
    }


    // Update is called once per frame
    void Update()
    {

        if (!isLocalPlayer)
        {
            return;
        }

        float x = Input.GetAxis("Horizontal") * Time.deltaTime * 150.0f;
        float z = Input.GetAxis("Vertical") * Time.deltaTime * 2;


        transform.Rotate(0, x, 0);
        transform.Translate(0, 0, z);
    }
}
