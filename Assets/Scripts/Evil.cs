using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AI;


public enum EvilState {

    Patrolling, Looking, Leaping, Searching
};




public class Evil : MonoBehaviour{

    NavMeshAgent agent;
    bool paused = false;
    private Transform pausepos;
    private int just = 0;


    GameObject player;
    Transform playerTransform, enemyTransform;

    GameObject[] patrolPoints;

    public static bool hittable = true;

    RaycastHit caster;
    Vector3 toPlayer, navDest, toNavDest;

    EvilState currState;

    int index = 0;



    // Start is called before the first frame update
    void Start(){

        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindWithTag("Player");
        playerTransform = player.transform;
        enemyTransform = gameObject.transform;

        patrolPoints = GameObject.FindGameObjectsWithTag("PatrolPoint");
        currState = EvilState.Looking;

        navDest = GameObject.Find("PP1").transform.position;
        agent.SetDestination(navDest);

        Random.InitState((int) Time.time);
    }

    // Update is called once per frame
    void Update(){

       //agent.SetDestination(playerTransform.position);

        toPlayer = playerTransform.position - enemyTransform.position;
        toNavDest = enemyTransform.position - navDest;

        //Debug.Log("dist to mark: " + Vector3.Magnitude(toNavDest));


        if (Vector3.Magnitude(toNavDest) < 1){

            currState = EvilState.Looking;

            index = Random.Range(0, patrolPoints.Length);

            navDest = patrolPoints[(int) index].transform.position;
            agent.SetDestination(navDest);

        }




        float angle;

        //if (currState == EvilState.Looking){
            //keep turning around if havnt turned around full 180 degs
            //agent.isStopped = true;
            //........not sure how youd cleanly do this..

        //}



       /*if (currState == EvilState.Patrolling ||
            currState == EvilState.Looking ||
            currState == EvilState.Searching){

            Debug.DrawRay(enemyTransform.position, toPlayer, Color.green, 1f);

            if (Physics.Raycast(enemyTransform.position, toPlayer, out caster, Mathf.Infinity)){



                if (caster.collider.CompareTag("Player")){

                    angle = Vector3.Angle(enemyTransform.forward, toPlayer);

                    if (angle < 30){

                        //Debug.Log("attacking...");
                        //currState = EvilState.Leaping;
                        agent.SetDestination(playerTransform.position);


                        //do some stuff, maybe propel with force...
                        //              maybe jack up the speed and set height 
                        //dk how to compromise between navagent and rigidbody..


                        //also start keeping track of time...
                        return;
                    }
                }
            }
        }*/



        /*there were problems here last time...
        begin maybe irrelevant code
        if (paused) {
            agent.Stop();
        }

        //has to do with pausing I think...
        if (just == 0){
            agent.SetDestination(this.transform.position);
            just = 1;

        }
        else
        {
            just = 0;
            agent.Resume();
        }
        *///end perhaps irrelevant code....
    }
}
