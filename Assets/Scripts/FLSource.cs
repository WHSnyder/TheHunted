using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FLSource : MonoBehaviour
{


    private Light source;
    private bool on = false;

    int layerMask;


    // Start is called before the first frame update
    void Start()
    {

        layerMask = 1 << 8;
        layerMask = ~layerMask;



        source  = transform.GetChild(0).gameObject.GetComponent<Light>();



    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            source.enabled = true;
            on = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            source.enabled = false;
            on = false;
        }

        if (on)
        {
            RaycastHit hit;
            // Does the ray intersect any objects excluding the player layer
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity,layerMask))
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
                Debug.Log("Did Hit");
            }

        }

    }
}
