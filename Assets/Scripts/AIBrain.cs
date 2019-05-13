using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBrain : MonoBehaviour
{
    public bool allChase = false;
    private bool seen;
    private float time = 10.0f;
    //public GameObject player; 

    GameObject[] enemyInitList;
    EnemyScript[] enemyList;


    void Start()
    {
        seen = false;
        enemyInitList = GameObject.FindGameObjectsWithTag("Enemy");
        enemyList = new EnemyScript[enemyInitList.Length];

        for (int i = 0; i < enemyInitList.Length; i++){
            enemyList[i] = enemyInitList[i].GetComponent<EnemyScript>();
        }
    }


    // Update is called once per frame
    void Update(){

    } 

    void countDown(float t) {
        time = time - Time.deltaTime;
    }


    public void notifyFound(Vector3 playerLoc){

        foreach (EnemyScript enemy in enemyList){

            enemy.processCommand(playerLoc, EvilState.Searching);
        }
    }



}
