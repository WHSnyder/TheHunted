using UnityEngine;

public class AIBrain : MonoBehaviour{

    GameObject[] enemyInitList;
    EnemyScript[] enemyList;

    void Start(){

        enemyInitList = GameObject.FindGameObjectsWithTag("Enemy");
        enemyList = new EnemyScript[enemyInitList.Length];

        for (int i = 0; i < enemyInitList.Length; i++){
            enemyList[i] = enemyInitList[i].GetComponent<EnemyScript>();
        }
    }


    public void notifyFound(Vector3 playerLoc,  int id){

        foreach (EnemyScript enemy in enemyList){

            if (enemy.id != id){
                enemy.processCommand(playerLoc, EvilState.Seeking);
            }
        }
    }
}