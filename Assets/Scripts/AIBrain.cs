using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBrain : MonoBehaviour
{
    public bool allChase = false;
    private bool seen;
    private float time = 10.0f;
    //public GameObject player; 
    void Start()
    {
        seen = false;
    }


    // Update is called once per frame
    void Update()
    {

        GameObject[] enemyList = GameObject.FindGameObjectsWithTag("Enemy");
        for (int x = 0; x < enemyList.Length; x++) { 
            if (enemyList[x].GetComponent<EnemyScript>().currState == EvilState.Searching) {
                seen = true;
            }
            else if (seen) {
                Debug.Log("countdown");
                countDown(time);
            } 
            else {
                //enemyList[x].GetComponent<EnemyScript>().Update(); 
                seen = false;
                time = 5.0f;
            }
        }    

        if (time <= 0.0f) {
            Debug.Log("ooooh shit");
            allChase = true;
        }

    } 

    void countDown(float t) {
        time = time - Time.deltaTime;
    }
}
