using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        gameObject.SetActive(false);
        Destroy(this.gameObject);
    }
}
