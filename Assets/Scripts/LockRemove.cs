using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockRemove : MonoBehaviour
{


    // Update is called once per frame
    void Update()
    {
        if (GameObject.FindGameObjectsWithTag("Key").Length == 0) {
            Destroy(this.gameObject);
        }
    }
}
