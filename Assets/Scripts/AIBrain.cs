using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBrain : MonoBehaviour
{
    public bool allChase = false;
    private bool seen;
    private bool flag; 
    private float time = 30.0f;
    //public GameObject player; 
    void Start()
    {
        seen = false;
        flag = false;
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
                //Debug.Log(time);
                countDown(time);
            } 
            else {
                //enemyList[x].GetComponent<EnemyScript>().Update();  
                //Debug.Log(time);
                seen = false;
                time = 30.0f;
            }
        }    

        if (time <= 0.0f) {
            //Debug.Log(time);
            allChase = true;
        }

    } 

    void countDown(float t) {
        time = time - Time.deltaTime;
    }
}
