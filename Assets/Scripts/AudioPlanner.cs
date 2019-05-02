using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public enum nodeType {

    Quad, STurn, LTurn, OneSeg, TwoSeg, FourSeg, EightSeg, Key, Pump
};


public class AudioNode
{

    public nodeType type;
    public ArrayList neighbors;
    public Vector3 location, forwards;
    public bool dest, source, visited;
    public int cost;
    public float strength;

    public AudioNode(nodeType _type, Vector3 _loc, Vector3 _forwards)
    {


        type = _type;
        neighbors = new ArrayList();

        location = _loc;
        forwards = _forwards;

        strength = 0;

        dest = false;
        source = false;
        visited = false;

        switch (_type)
        {
            case nodeType.OneSeg:

                cost = 1;
                break;

            case nodeType.TwoSeg:

                cost = 2;
                break;

            case nodeType.FourSeg:

                cost = 4;
                break;

            case nodeType.EightSeg:

                cost = 8;
                break;

            case nodeType.STurn:

                cost = 4;
                break;

            case nodeType.LTurn:

                cost = 8;
                break;

            case nodeType.Quad:

                cost = 6;
                break;

            case nodeType.Key:

                cost = 1;
                break;
        }
    }

    public ArrayList searchPositions(){

        ArrayList result = new ArrayList();

        int num = 2;

        Vector3[] pos = new Vector3[4];

        if (type == nodeType.Quad){

            float mult = 10f;
            pos[0] = location + .5f * Vector3.up + mult * Vector3.right;
            pos[1] = location + .5f * Vector3.up + mult * Vector3.left;
            pos[2] = location + .5f * Vector3.up + mult * Vector3.forward;
            pos[3] = location + .5f * Vector3.up + mult * Vector3.back;
            num = 4;
        }

        else if (type == nodeType.OneSeg){

            pos[0] = location + 20f * Vector3.up + 2f * forwards;
            pos[1] = location + 20f * Vector3.up + -2f * forwards;
        }

        else if (type == nodeType.TwoSeg){

            pos[0] = location + 20f * Vector3.up + 4f * forwards;
            pos[1] = location + 20f * Vector3.up + -4f * forwards;
        }

        else if (type == nodeType.FourSeg){

            pos[0] = location + 20f * Vector3.up + 7f * forwards;
            pos[1] = location + 20f * Vector3.up + -7f * forwards;
        }

        else if (type == nodeType.EightSeg){


            pos[0] = location + 20f * Vector3.up + 13f * forwards;
            pos[1] = location + 20f * Vector3.up + -13f * forwards;
        }

        else if (type == nodeType.STurn){

            pos[0] = location + 20f * Vector3.up + 5f * forwards;
            pos[1] = location + 20f * Vector3.up + Quaternion.Euler(0, 220, 0) * (5f * forwards);
        }

        else if (type == nodeType.LTurn){

            pos[0] = location + 20f * Vector3.up + 8f * forwards;
            pos[1] = location + 20f * Vector3.up + Quaternion.Euler(0, 210, 0) * (8f * forwards);
        }

        else if (type == nodeType.Pump){

            pos[0] = location + 20f * Vector3.up + forwards * 10;
            num = 1;
        }

        else if (type == nodeType.Key){

            pos[0] = location + 20f * Vector3.up + 4f * forwards;
            num = 1;
        }

        else return null;

        for (int i = 0; i < num; i++){
            result.Add(pos[i]);
        }

        return result;
    }

    public static AudioNode AudioNodeFromObj(GameObject obj)
    {
        if (obj.CompareTag("segOne")){
            return new AudioNode(nodeType.OneSeg, obj.transform.position, obj.transform.right);
        }

        else if (obj.CompareTag("segTwo")){
            return new AudioNode(nodeType.TwoSeg, obj.transform.position, obj.transform.right);
        }

        else if (obj.CompareTag("segFour")){
            return new AudioNode(nodeType.FourSeg, obj.transform.position, obj.transform.right);
        }

        else if (obj.CompareTag("segEight")){
            return new AudioNode(nodeType.EightSeg, obj.transform.position, obj.transform.right);
        }

        else if (obj.CompareTag("QuadJoiner")){
            return new AudioNode(nodeType.Quad, obj.transform.position, obj.transform.forward);
        }

        else if (obj.CompareTag("LongTurn")){
            return new AudioNode(nodeType.LTurn, obj.transform.position, .6f*obj.transform.up + obj.transform.right);
        }

        else if (obj.CompareTag("ShortTurn")){
            return new AudioNode(nodeType.STurn, obj.transform.position, .3f*obj.transform.up + obj.transform.right);
        }

        else if (obj.CompareTag("KeyRoom")){
            return new AudioNode(nodeType.Key, obj.transform.position, -1*obj.transform.right);
        }

        else if (obj.CompareTag("PumpRoom")){
            return new AudioNode(nodeType.Pump, obj.transform.position, obj.transform.right);
        }

        else return null;
    }

    //returns an array list of neighbors and adds the neighbors to the objects list
    public ArrayList addNeighbors(Dictionary<Vector3, AudioNode> map){

        ArrayList result = new ArrayList(), positions = searchPositions();

        if (positions != null){

            foreach (Vector3 pos in positions){

                result.Add(castDetect(map, pos));
            }
        }
        else return null;

        return result;
    }


    private AudioNode castDetect(Dictionary<Vector3, AudioNode> map, Vector3 pos){
        RaycastHit hit;
        AudioNode anuda1;
        AudioNode result;

        Debug.DrawRay(pos + Vector3.up * 10, 1000*Vector3.down, Color.green, 100);

        if (Physics.Raycast(pos, Vector3.down, out hit, 10000))
        {
            anuda1 = AudioNodeFromObj(hit.collider.gameObject);
            //Debug.Log("not crazy!!");

            if (!map.ContainsKey(hit.collider.gameObject.transform.position))
            {
                //Debug.Log("found new....");

                result = anuda1;
                map.Add(hit.collider.gameObject.transform.position, anuda1);
            }
            else
            {
                //Debug.Log("found existing....");
                result = map[anuda1.location];
            }
        }
        else result = null;

        return result;
    }
}


public class AudioPlanner : MonoBehaviour {

    Dictionary<Vector3, AudioNode> hashMap = new Dictionary<Vector3, AudioNode>();

    ArrayList check = new ArrayList();
    Queue<AudioNode> queue = new Queue<AudioNode>();

    float timer = 0;

    public GameObject player;
    public GameObject source;

    public int dieDist, decayRate, startStrength;

    GameObject stepSourceOne, stepSourceTwo;


    public void Start(){

        //start on a quad joiner....
        AudioNode first = AudioNode.AudioNodeFromObj(source), curr;
        hashMap.Add(first.location, first);

        ArrayList neighbors = first.addNeighbors(hashMap);

        stepSourceOne = GameObject.Find("stepSourceOne");
        stepSourceTwo = GameObject.Find("stepSourceTwo");
        player = GameObject.Find("SkippyAlone");

        queue.Enqueue(first);

        while (queue.Count > 0){

            curr = queue.Dequeue();

            if (curr.visited == false){
                neighbors = curr.addNeighbors(hashMap);
                curr.neighbors = neighbors;
                curr.visited = true;
                
                foreach (AudioNode node in neighbors){

                    if (node != null && node.visited == false){
                        queue.Enqueue(node);
                    }
                }
            }
        }
        queue.Clear();
    }


    public void Update(){

        AudioNode playerNode, sourceNode, curr;
        timer = timer + Time.deltaTime;

        if (timer > 2){

            //Debug.Log("beginning");

            float startTime = Time.time; 

            timer = 0;
            RaycastHit hit1, hit2;
            ArrayList neighbors;

            if (Physics.Raycast(player.transform.position + Vector3.right + 30 * Vector3.up, Vector3.down, out hit1, 1000)
            && Physics.Raycast(source.transform.position + 20 * Vector3.up, Vector3.down, out hit2, 1000)){

                //Debug.Log("Player shot hit: " + hit1.collider.gameObject.ToString());

                playerNode = hashMap[hit1.collider.gameObject.transform.position];

                //Debug.Log("Nodes established.....");

                sourceNode = hashMap[hit2.collider.gameObject.transform.position];


                sourceNode.source = true;
                playerNode.dest = true;

                sourceNode.strength = (int)startStrength;

                queue.Enqueue(sourceNode);

                while (queue.Count > 0){

                    curr = queue.Dequeue();

                    if (curr.visited == false && curr.strength > 0 ){

                        if (Vector3.Magnitude(curr.location - playerNode.location) < dieDist){

                           //Debug.Log("At " + curr.type + " (" + curr.strength + ")");

                            curr.visited = true;

                            foreach (AudioNode node in curr.neighbors){

                                if (node != null && node.visited == false && node.dest == false){

                                    //Debug.Log("Updating neighbor: " + node.type);

                                    node.strength = curr.strength - decayRate * curr.cost;
                                    queue.Enqueue(node);
                                }
                            }
                        }
                    }
                }

                //Debug.Log("Done search, loggging neighbors of final now ("+ playerNode.type +")............");

                bool flag = false;
                foreach (AudioNode node in playerNode.neighbors) {

                    if (node == null)
                    {
                        Debug.Log("yep just null...");
                        continue;
                    }


                    if (node.strength > 0 && !flag){
                        //Debug.Log("Neighbor (" + node.type +") with strength " + node.strength);
                        stepSourceOne.transform.position = node.location;
                        stepSourceOne.GetComponent<AudioSource>().volume = node.strength / 100;
                        flag = true;
                        continue;
                    }
                    if (node.strength > 0 && flag){
                        //Debug.Log("Neighbor (" + node.type + ") with strength " + node.strength);
                        stepSourceTwo.transform.position = node.location;
                        stepSourceTwo.GetComponent<AudioSource>().volume = node.strength / 100;
                        flag = true;
                        break;
                    }
                }

                //Debug.Log("........................................................");

                


                foreach (Vector3 key in hashMap.Keys) {
                    AudioNode reset = hashMap[key];
                    reset.dest = false;
                    reset.source = false;
                    reset.strength = 0;
                    reset.visited = false;
                }
            }
        }
    }
}