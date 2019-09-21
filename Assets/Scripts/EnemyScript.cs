using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


/*
 * looking is when the slist ARRIVES at a PATROL POINT and looks around
 * searching is when the PLAYERS LOCATION is KNOWN
 * patrolling is moving to randomly selected patrol point 
 * will add ambush if have time...
 */

public enum EvilState
{
    Patrolling, Looking, Attacking, Seeking, Ambush, Stunned, Init 
};


/* commands can be sent from brain (to search), from the player (to stun),
 * or from the slist itself (to queue an obvious state transition)
 */

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
    public int id = 0;
    public static int idCounter = 0;


    //for random patrol initialization...
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
    public EvilState currState = EvilState.Init;

    //this var represents a command the slist has yet to process, they can 
    //be sent from the player (when stunning) or from the brain..
    private Command queuedCommand = null;


    //animator params
    Animator animator;
    int moveHash = Animator.StringToHash("Base Layer.Running");
    int lookHash = Animator.StringToHash("Base Layer.Looking");
    int stunHash = Animator.StringToHash("Base Layer.Stun");


    //audio params
    private AudioPlanner planner;
    private GameObject sourceOne, sourceTwo;
    public AudioClip crank;

    private AudioSource one, two;
    private int freq = 0;
    public int maxDistAudio = 30;


    //Public vars to be set in editor for tweaking behavior (myhead is head collider, prolly fine as is)
    public GameObject myHead;
    public static int sightRange = 15; 


    //Timers for each state, can be set in editor for performance tweaks
    //the time far stays as is, the timer vars change...
    public static float stunTime = 10;
    private float stunTimer = stunTime;

    public static float lookTime = 4;
    private float lookTimer = lookTime;

    public static float trackingTime = 10;
    private float trackingTimer = trackingTime;

    public static float ambushTime = 15;
    private float ambushTimer = ambushTime;

    private int ambushOrPatrol;
    private bool ambshing = false;



    // Start is called before the first frame update
    void Start() {

        brain = GameObject.Find("AIBrain").GetComponent<AIBrain>();

        id = idCounter++;
        
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

        Random.InitState(System.DateTime.Now.Millisecond);
        ambushOrPatrol = Random.Range(1, 3);

        StartCoroutine("crankSound");
    }


    //Test method...
    IEnumerator crankSound(){
        float vol = 1 - Vector3.Magnitude(transform.position - player.transform.position) / maxDistAudio;
        one.PlayOneShot(crank, vol);
        //two.PlayOneShot(crank, vol);

        yield return new WaitForSeconds(.5f);
    }



    public void Update() {

    
        //set important vectors and quantities we often need
        toPlayer = playerTransform.position - myHead.transform.position;
        toNavDest = myTransform.position - navDest;
        angleToPlayer = Vector3.Angle(myHead.transform.forward, toPlayer);
        magToPlayer = Vector3.Magnitude(toPlayer);

        // if in 20 to 25 distance randomly go into ambush or keep patrolling
        if ((magToPlayer >20) && (magToPlayer < 25) && (currState == EvilState.Patrolling) && (id % 2 == 0)) {
            if (ambushOrPatrol == 1) {
                //transitionToAmbush();
            } 
        }

        if (Input.GetKeyDown("t")){

            if (ambshing){
                transitionToPatrolling();
            }
            else {
                transitionToAmbush();
            }
            ambshing = !ambshing;
        }



        switch (currState) {

            //start state that always patrols...
            case EvilState.Init:
                transitionToPatrolling();
                break;


            /* if player is seen they will be attacked and brain notified of position
             * no matter what, a searching slist will follow orders eg; if brain
             * says the player is somewhere, they go, if the player says to get stunned
             * the slist stuns...
             */

            case EvilState.Seeking:

                if (queuedCommand != null){
                    transitionFromCommand(queuedCommand);
                    //Debug.Log("tranning");
                    return;
                }

                if (withinRange()) {
                    if (checkForPlayer()){
                        brain.notifyFound(player.transform.position, id);
                        transitionToAttacking();
                        return;
                    }
                }

                if (Vector3.Magnitude(toNavDest) < 1){
                    transitionToLooking();
                }
                
                break;


            /* no matter what, if the patrolling slist is commanded, it stops
             * patrolling and follows orders
             *
             * if the player is within range and can be seen directly, slist attacks
             *
             * if slist is within one of navdest, the transition to looking
             * they will patrol again once the looking state ends, unless they
             * are commanded otherwise...
             */

            case EvilState.Patrolling:

                //Debug.Log("in patrol...");

                if (queuedCommand != null) {
                    transitionFromCommand(queuedCommand);
                    return;
                }

                if (withinRange()) { //no need to do raycast if out of range

                    if (checkForPlayer()) {
                        //Debug.Log("found!");
                        brain.notifyFound(player.transform.position, id);
                        transitionToAttacking();
                        return;
                    }
                }
                if (Vector3.Magnitude(toNavDest) < 1) {
                    transitionToLooking();
                }

                break;


            /* remains stunned for timer countdown, does whatever is queued the queue afterwards
             * if nothing on queue, they look around
             */

            case EvilState.Stunned:

                Debug.Log("yes here..");

                if (stunTimer < 0) {

                    Debug.Log("unstunned");
                    stunTimer = stunTime;//reset timer

                    if (queuedCommand != null){
                        transitionFromCommand(queuedCommand);
                    }
                    else transitionToLooking();
                }
                else stunTimer -= Time.deltaTime;

                break;


            /* plays through slist look-around anim once (still needs sight checking!!! from the head bone..)
             * IF there is a command queued, the slist will stop looking around and follow the order
             * the order can be from the player or brain
             */

            case EvilState.Looking:

                if (queuedCommand != null){
                    lookTimer = lookTime;
                    transitionFromCommand(queuedCommand);
                    return;
                }

                if (lookTimer < 0) {
                    lookTimer = lookTime;
                    transitionToPatrolling();
                }
                else lookTimer -= Time.deltaTime;

                break;

            case EvilState.Attacking:

                //play some anim
                if (queuedCommand != null && queuedCommand.action == EvilState.Stunned){
                    transitionToStunned();
                    queuedCommand = null;
                    return;
                }

                agent.SetDestination(playerTransform.position);
                break;

            //stick slist to ceiling and wait...
            case EvilState.Ambush:
                if (magToPlayer < 8) {
                    agent.enabled = true;
                    animator.enabled = true;
                    transitionToAttacking();
                    ambushTimer = ambushTime;
                    return;
                }

                if (ambushTimer < 0){
                    transitionToPatrolling();
                    ambushTimer = ambushTime;
                }
                else{
                    ambushTimer -= Time.deltaTime;
                }

                break;
        }
    }

    //simple method branch...
    public void transitionFromCommand(Command command){

        switch (command.action){

            case EvilState.Looking:

                transitionToLooking();
                break;

            case EvilState.Seeking:

                transitionToSeeking(command.loc);
                break;

            case EvilState.Patrolling:

                transitionToPatrolling();
                break;

            case EvilState.Stunned:

                transitionToStunned();
                break;
        }
        queuedCommand = null;
    }


    //if slist stunned, she will search when wake up, else they move to that location
    private void transitionToSeeking(Vector3 loc) {

        if (currState == EvilState.Stunned){
            queuedCommand = new Command(loc, EvilState.Seeking);
        }
        else{
            currState = EvilState.Seeking;
            navDest = loc;
            agent.enabled = true;
            agent.SetDestination(loc);
            animator.Play(moveHash);

            queuedCommand = null;
        }
    }


    /*
     * used after slist stunned, she looks around for a bit (dk if should)
     * no queued command specified, they will look around then patrol by default,
     * unless the brain or player sends a command
     */

    private void transitionToLooking() {

        currState = EvilState.Looking;
        animator.Play(lookHash);
    }


    //no idea how to animate/do physics for this yet..
    private void transitionToAttacking() {

        currState = EvilState.Attacking;
        queuedCommand = null;
        agent.enabled = true;
        agent.SetDestination(playerTransform.position);
        agent.speed = agent.speed * 3f;
    }


    //go through stunned stage and will look around afterwards
    private void transitionToStunned() {
        Debug.Log("Worked!!");
        currState = EvilState.Stunned;
        queuedCommand = new Command(Vector3.zero, EvilState.Looking);

        agent.enabled = false;
        animator.Play(stunHash);
    }


    //get random patrol point then go to it...
    private void transitionToPatrolling() {
        index = Random.Range(0, patrolPoints.Length);
        patrolPt = patrolPoints[(int)index].transform.position;
        navDest = new Vector3(patrolPt.x, 0, patrolPt.z);

        currState = EvilState.Patrolling;
        queuedCommand = null;

        animator.enabled = true;
        animator.Play(moveHash);

        agent.enabled = true;
        agent.SetDestination(navDest);
    }


    //slist sticks to the ceiling by disabling navmesh and moving 
    //important bones in line with the angle of the face directly above...
    /*private void transitionToAmbush(){

        //animator.WriteDefaultValues();
        animator.enabled = false;
        agent.enabled = false;

        currState = EvilState.Ambush;

        //straighten out
        transform.Rotate(new Vector3(90, 0, 0));
        transform.position += Vector3.up;

        int i = 0;
        int mask = ~(1 << 9);

        float boneAngle;

        Transform animRoot = transform.Find("Armature").Find("Root");

        ArrayList angles = new ArrayList(), points = new ArrayList(), bones;

        Queue<Transform> animQ = new Queue<Transform>();
        animQ.Enqueue(animRoot);
        bones = traverseAnimTree(animQ);

        Vector3 crossProd;

        foreach (Transform bone in bones){
        
            Physics.Raycast(bone.position, Vector3.up, out caster, 20, mask);
            angles.Add(caster.normal);
            points.Add(caster.point);
        }


        foreach (Transform bone in bones){

            Debug.Log("transforming bone: " + bone.gameObject.name);

            bone.position = (Vector3) points[i];
            crossProd = Vector3.Cross((Vector3) angles[i], bone.forward);

            boneAngle = Vector3.SignedAngle(bone.forward, (Vector3) angles[i], crossProd);

            if (bone.gameObject.name.Equals("Thigh.R")){
                Debug.DrawRay(bone.position, 2 * bone.forward, Color.blue, 160);
                Debug.DrawRay(bone.position, 2 * crossProd, Color.red, 160);
                Debug.DrawRay(bone.position, 2 * (Vector3)angles[i], Color.green, 160);
            }
            
            bone.Rotate(crossProd, boneAngle, Space.World);

            if (bone.gameObject.name.Equals("Thigh.R")){
                Debug.DrawRay(bone.position, 2 * bone.forward, Color.yellow, 160);
            }

            ++i;
        }
    }*/


    //slist sticks to the ceiling by disabling navmesh and moving 
    //important bones in line with the angle of the face directly above...
    private void transitionToAmbush(){

        //animator.WriteDefaultValues();
        animator.enabled = false;
        agent.enabled = false;

        currState = EvilState.Ambush;

        //straighten out
        transform.localEulerAngles = new Vector3(90, 0, 0);
        transform.position += .2f*Vector3.up;

        int i = 0;
        int mask = ~(1 << 9);

        float boneAngle;

        Transform animRoot = transform.Find("Armature").Find("Root");

        ArrayList angles = new ArrayList(), points = new ArrayList(), bones;

        Queue<Transform> animQ = new Queue<Transform>();
        animQ.Enqueue(animRoot);
        bones = traverseAnimTree(animQ);

        Vector3 crossProd;

        Vector3 boneFwd;

        foreach (Transform bone in bones){

            Physics.Raycast(bone.position, Vector3.up, out caster, 20, mask);
            angles.Add(caster.normal);
            points.Add(caster.point);
        }


        foreach (Transform bone in bones){

            Debug.Log("transforming bone: " + bone.gameObject.name);

            bone.position = (Vector3)points[i];



            if (bone.gameObject.name.Contains("nkle")){
                boneFwd = bone.up;
            }
            else if (bone.gameObject.name.Contains("iddle") ||
                     bone.gameObject.name.Contains("hin")){
                boneFwd = bone.right;
            }
            else{
                boneFwd = bone.forward;
            }



            crossProd = Vector3.Cross((Vector3)angles[i], boneFwd);

            boneAngle = Vector3.SignedAngle(boneFwd, (Vector3)angles[i], crossProd);

            if (bone.gameObject.name.Contains("iddle")){
                Debug.DrawRay(bone.position, 2 * bone.forward, Color.blue, 160);
                Debug.DrawRay(bone.position, 2 * crossProd, Color.red, 160);
                Debug.DrawRay(bone.position, 2 * (Vector3)angles[i], Color.green, 160);
            }

            bone.Rotate(crossProd, boneAngle, Space.World);

            if (bone.gameObject.name.Contains("iddle")){
                Debug.DrawRay(bone.position, 2 * boneFwd, Color.yellow, 160);
            }

            ++i;
        }
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
    private bool checkForPlayer() {
        if (Physics.Raycast(myHead.transform.position, toPlayer, out caster, sightRange) && (angleToPlayer < 30)){
            if (caster.collider.CompareTag("Player")){
                return true;
            }
            else return false;
        }
        else if (magToPlayer < 5 && angleToPlayer > 90){
            return true;
        }
        else return false;
    }


    //takes a command from the brain OR the player
    //does not directly set the transition, but the queued 
    public void processCommand(Vector3 loc, EvilState order){

        if (currState == EvilState.Attacking && order != EvilState.Stunned){
            //do nothing, ignores everything when attacking EXCEPT when player stuns them

            return;
        }
        else queuedCommand = new Command(loc, order);
    }


    //step (this works)
    public void Step(){
        //float vol = 1 - Vector3.Magnitude(transform.position - player.transform.position) / maxDistAudio;
        //one.PlayOneShot(stepSound, vol);
        //two.PlayOneShot(stepSound, vol);
    }


    //for setting up the abmush pose
    private ArrayList traverseAnimTree(Queue<Transform> queue) {

        ArrayList result = new ArrayList();

        while (queue.Count > 0){

            Transform curr = queue.Dequeue();

            string boneName = curr.gameObject.name;

            if (!boneName.Contains("end")){

                result.Add(curr);

                for (int i = 0; i < curr.childCount; i++){
                    queue.Enqueue(curr.GetChild(i));
                }
            }
        }
        return result;
    }
}
