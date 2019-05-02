using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterScript : MonoBehaviour
{
    bool switchHit = false;
    float speed = .1f;
    // Update is called once per frame
    void Update()
    {
        if (GameObject.FindGameObjectsWithTag("WaterSwitch").Length == 0) {
            switchHit = true;
        }

        if (switchHit) {
            transform.Translate(Vector3.down * Time.deltaTime * speed);
        } 

        if (transform.position.y < -2.0f) {
            switchHit = false;
            speed = 0.0f;
        }
    }
}
