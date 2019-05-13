﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AI;


//looking is when the slist ARRIVES at a PATROL POINT and looks around
//searching is when the PLAYERS LOCATION is KNOWN
//patrolling is moving to randomly selected patrol point 
//will add ambush if have time...
public enum EvilState
{
    Patrolling, Looking, Attacking, Searching, Ambush, Stunned, Init 
};

//commands can be sent from brain (to search), from the player (to stun),
//or from the slist itself (to queue an obvious state transition)
public class Command
{
    public Vector3 loc;
    public EvilState action;

    public Command(Vector3 ploc, EvilState paction){
        loc = ploc;
        action = paction;
    }
}



public class EnemyScript : MonoBehaviour{

    //AI stuff
    private AIBrain brain; 
    private NavMeshAgent agent;

    //for random patrol initialization...
    private static int seed = 0;
    int index = 0;


    //data necessary for movement, sight, distance calculations...
    Vector3 toPlayer, navDest, toNavDest;
    private float angleToPlayer, magToPlayer;
    RaycastHit caster;



    //variables referring to other relevant objects and the player
    GameObject player;
    Transform playerTransform, myTransform;
    GameObject[] patrolPoints;
    Vector3 patrolPt;


    //basic dummy state
    private EvilState currState = EvilState.Init;

    //this var represents a command the slist has yet to process, they can 
    //be sent from the player (when stunning) or from the brain..
    private Command queuedCommand = null;



    //animator params
    Animator animator;
    int moveHash = Animator.StringToHash("Base Layer.Running");
    int lookHash = Animator.StringToHash("Base Layer.Looking");
    int stunHash = Animator.StringToHash("Base Layer.Stunning");



    //audio params
    private AudioPlanner planner;
    private GameObject sourceOne, sourceTwo;
    public AudioClip stepSound;
    private AudioSource one, two;
    private int freq = 0;
    public int maxDistAudio = 30;




    //PUBLIC vars to be set in editor for tweaking behavior (myhead is head collider, prolly fine as is)
    public GameObject myHead;
    public int sightRange = 20; 


    //Timers for each state, can be set in editor for performance
    //the time far stays as is, the timer vars change...
    public float stunTime = 5;
    private float stunTimer;
    private float lookTime = 10;
    private float lookTimer;





    // Start is called before the first frame update
    void Start() {

        brain = GameObject.Find("AIBrain").GetComponent<AIBrain>();

        sourceOne = Instantiate(Resources.Load<GameObject>("AudioEmitter")) as GameObject;
        sourceTwo = Instantiate(Resources.Load<GameObject>("AudioEmitter")) as GameObject;

        one = sourceOne.GetComponent<AudioSource>();
        two = sourceTwo.GetComponent<AudioSource>();

        planner = GameObject.Find("Main Camera").GetComponent<AudioPlanner>();

        stunTimer = stunTime;
        lookTimer = lookTime;

        AudioData data = new AudioData(freq, 1, gameObject, sourceOne, sourceTwo);

        planner.requestSearch(data);
        animator = GetComponent<Animator>();

        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindWithTag("Player");
        playerTransform = player.transform;
        myTransform = gameObject.transform;

        patrolPoints = GameObject.FindGameObjectsWithTag("PatrolPoint");

        Random.InitState(++seed);
    }



    public void Update() {
    
        //set important vectors and quantities we often need
        toPlayer = playerTransform.position - myTransform.position;
        toNavDest = myTransform.position - navDest;
        angleToPlayer = Vector3.Angle(myTransform.forward, toPlayer);
        magToPlayer = Vector3.Magnitude(toPlayer);


        switch (currState) {

            //start state that always patrols...
            case EvilState.Init:

                transitionToPatrolling();
                //Debug.Log("now patrolling");
                break;


            //if player is seen they will be attacked and brain notified of position
            // no matter what, a searching slist will follow orders eg; if brain
            //says the player is somewhere, they go, if the player says to get stunned
            //the slist stuns...
            case EvilState.Searching:

                if (queuedCommand != null){
                    transitionFromCommand(queuedCommand);
                }

                if (checkSight()) {
                    if (withinRange()) {
                        brain.notifyFound(player.transform.position);
                        transitionToAttacking();
                    }
                }
                break;


            //no matter what, if the patrolling slist is commanded, it stops patrolling and follows orders
            //
            //if the player is within range and can be seen directly, slist attacks
            //
            //if slist is within one of navdest, the transition to looking
            //they will patrol again once the looking state ends, unless the are commanded otherwise
            case EvilState.Patrolling:

                //Debug.Log("in patrol...");

                if (queuedCommand != null) {
                    transitionToSearching(queuedCommand.loc);
                    return;
                }

                if (withinRange()) {//no need to do raycast if out of range
                    Debug.Log("within range!");
                    if (checkSight()) {
                        Debug.Log("sighted!");
                        brain.notifyFound(player.transform.position);
                        transitionToAttacking();
                        return;
                    }
                }

                if (Vector3.Magnitude(toNavDest) < 1) {
                    transitionToLooking();
                }

                break;


            //bastard remains stunned for timer countdown, does whatever is queued the queue afterwards
            //if nothing on queue, they look around
            case EvilState.Stunned:

                if (stunTimer < 0) {
                    stunTimer = stunTime;//reset timer

                    if (queuedCommand != null){
                        transitionFromCommand(queuedCommand);
                    }
                    else transitionToLooking();
                }
                else stunTimer -= Time.deltaTime;

                break;


            //plays through slist look-around anim once (still needs sight checking!!! from the head bone..)
            //IF there is a command queued, the slist will stop looking around and follow the order
            //the order can be from the player or brain
            case EvilState.Looking:

                if (queuedCommand != null){
                    lookTimer = lookTime;
                    transitionFromCommand(queuedCommand);
                }

                if (lookTimer < 0) {
                    lookTimer = lookTime;
                }

                else lookTimer -= Time.deltaTime;

                break;
        }
    }

    //simple method branch...
    public void transitionFromCommand(Command command){

        switch (command.action){

            case EvilState.Looking:

                transitionToLooking();
                break;

            case EvilState.Searching:

                transitionToSearching(command.loc);
                break;

            case EvilState.Patrolling:
                transitionToPatrolling();
                break;
        }
        queuedCommand = null;
    }

    //if slist stunned, she will search when wake up, else they move to that location
    private void transitionToSearching(Vector3 loc) {

        if (currState == EvilState.Stunned){
            queuedCommand = new Command(loc, EvilState.Searching);
        }
        else{
            currState = EvilState.Searching;
            navDest = loc;
            agent.SetDestination(loc);
            animator.Play(moveHash);

            queuedCommand = null;
        }
    }

    //used after slist stunned, she looks around for a bit (dk if should)
    //no queued command specified, they will look around then patrol by default,
    //unless the brain or player sends a command
    private void transitionToLooking() {
        currState = EvilState.Looking;
        animator.Play(lookHash);
    }

    //no idea how to animate/do physics for this yet..
    private void transitionToAttacking() {
        currState = EvilState.Attacking;
        queuedCommand = null;
    }


    //he/she/it/they/Jabba's gender go through stunned stage and will look around afterwards
    private void transitionToStunned() {
        currState = EvilState.Stunned;
        queuedCommand = new Command(Vector3.zero, EvilState.Looking);
    }


    //get random patrol point then go to it...
    private void transitionToPatrolling() {

        index = Random.Range(0, patrolPoints.Length);
        patrolPt = patrolPoints[(int)index].transform.position;
        navDest = new Vector3(patrolPt.x, 0, patrolPt.z);

        currState = EvilState.Patrolling;
        queuedCommand = null;

        agent.SetDestination(navDest);
    }

    //uses sight range public var to determine if player could possibly be seen
    private bool withinRange() {
        if (magToPlayer < sightRange) {
            return true;
        }
        else return false;
    }

    //if player is at too far an angle, they cant see (this should be changed...)
    //if not, raycast to see if theyre in sight...
    private bool checkSight() {

        if (angleToPlayer < 30){
            if (Physics.Raycast(transform.position, toPlayer, out caster, sightRange)){
                if (caster.collider.CompareTag("Player")){
                    return true;
                }
            }
            else return false;
        }
        return false;
    }


    //takes a command from the brain OR the player
    //does not directly set the transition, but the queued 
    public void processCommand(Vector3 loc, EvilState state){

        if (currState == EvilState.Attacking && state != EvilState.Stunned){
            //do nothing, ignores everything when attacking EXCEPT when player stuns them
            return;
        }
        else queuedCommand = new Command(loc, state);
    }






    //step (this works)
    public void Step(){
        float vol = 1 - Vector3.Magnitude(transform.position - player.transform.position) / maxDistAudio;
        one.PlayOneShot(stepSound, vol);
        two.PlayOneShot(stepSound, vol);
    }


}
