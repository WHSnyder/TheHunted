using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AI;



public class EnemyScript : MonoBehaviour
{
    private GameObject brain; 
    public NavMeshAgent agent;
    //bool paused = false;
    bool hasScreamed = false;
    private Transform pausepos;
    private static int seed = 0;

    private AudioPlanner planner;

    public AudioClip stepSound;
    public int freq = 0;

    GameObject player;
    Transform playerTransform, enemyTransform;

    GameObject[] patrolPoints;


    Vector3 patrolPt;
    

    RaycastHit caster;
    Vector3 toPlayer, navDest, toNavDest;

    public EvilState currState;

    int index = 0;



    Animator animator;

    int moveHash = Animator.StringToHash("Base Layer.Running");

    private GameObject sourceOne, sourceTwo;

    private AudioSource one, two;

    public int maxDist;

    bool first = true;


    // Start is called before the first frame update
    void Start(){

        brain = GameObject.Find("AIBrain"); 

        sourceOne = Instantiate(Resources.Load<GameObject>("AudioEmitter")) as GameObject;
        sourceTwo = Instantiate(Resources.Load<GameObject>("AudioEmitter")) as GameObject;

        one = sourceOne.GetComponent<AudioSource>();
        two = sourceTwo.GetComponent<AudioSource>();

        planner = GameObject.Find("Main Camera").GetComponent<AudioPlanner>();


        AudioData data = new AudioData(freq, 1, gameObject, sourceOne, sourceTwo);


        planner.requestSearch(data);

        animator = GetComponent<Animator>();

        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindWithTag("Player");
        playerTransform = player.transform;
        enemyTransform = gameObject.transform;

        patrolPoints = GameObject.FindGameObjectsWithTag("PatrolPoint");
        currState = EvilState.Looking;

        Random.InitState(++seed);

        animator.Play(moveHash);
    }



    // Update is called once per frame
    public void Update(){ 


    
        toPlayer = playerTransform.position - enemyTransform.position;
        toNavDest = enemyTransform.position - navDest;
        float angle1 = Vector3.Angle(this.transform.forward, toPlayer);


        if (brain.GetComponent<AIBrain>().allChase) {
            agent.SetDestination(player.transform.position);
            currState = EvilState.Searching; 
        }

        else if (Vector3.Magnitude(toPlayer) < 5) {
            //Debug.Log("close");
            if (!hasScreamed) {
                //scream 
                hasScreamed = true;
            }

            //Debug.Log("Get him!");
            agent.SetDestination(player.transform.position);
            currState = EvilState.Searching;
        }  

        else if (Vector3.Magnitude(toPlayer) < 10  && angle1 < 90) {
            //Debug.Log("i can still see you");
            agent.SetDestination(player.transform.position);
            currState = EvilState.Searching;
        }

        else if (currState == EvilState.Searching) {
            //Debug.Log("searching");
            index = Random.Range(0, patrolPoints.Length);
            patrolPt = patrolPoints[(int)index].transform.position;
            navDest = new Vector3(patrolPt.x, 0, patrolPt.z);
            currState = EvilState.Looking;
            hasScreamed = false; 
            agent.SetDestination(navDest);
        }

        else  //(Vector3.Magnitude(toNavDest) < 1 || first)
        {
            //Debug.Log("searching");
            //Debug.Log("Where is he? ");
            if (Vector3.Magnitude(toNavDest) < 1 || first) {
                currState = EvilState.Looking;

                index = Random.Range(0, patrolPoints.Length);

                //Debug.Log("My index is: " + index);

                patrolPt = patrolPoints[(int)index].transform.position;

                navDest = new Vector3(patrolPt.x, 0, patrolPt.z);

                agent.SetDestination(navDest);
                first = false;
            }


        }



        //float angle;

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


    public void Step(){

        float vol = 1 - Vector3.Magnitude(transform.position - player.transform.position)/maxDist;

        one.PlayOneShot(stepSound, vol);
        two.PlayOneShot(stepSound, vol);
    }
}
