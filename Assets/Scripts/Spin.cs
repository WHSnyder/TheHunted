using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up, 45 * Time.deltaTime);
        transform.Rotate(Vector3.forward, 45 * Time.deltaTime);
    }

}
