using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Basic linked list node class containing an AudioNode and its fscore.
 */

public class LLNode<T> {

    public LLNode<T> next;
    public T node;
    public float distance;

    public LLNode(T _node, float _distance){

        next = null;
        node = _node;
        distance = _distance;
    }
}


/*
 * Basic queue wrapping a linked list.
 */

public class LLQueue<T> {

    public LLNode<T> head;
    public static float counter;

    public LLQueue(LLNode<T> start){
        head = start;
    }

    public LLQueue(){
        head = null;
    }

    public LLQueue(T start, float dist){
        head = new LLNode<T>(start, dist);
    }


    public void insert(T nodein, float dist){

        LLNode<T> curr = head, new_node = new LLNode<T>(nodein, dist);

        if (head == null){
            head = new_node;
            return;
        }

        if (curr.distance > dist) {
            new_node.next = curr;
            head = new_node;
            return;
        }

        while (curr.next != null) {

            if (curr.next.distance > dist) {
                new_node.next = curr.next;
                curr.next = new_node;
                return;
            }
            curr = curr.next;
        }

        curr.next = new_node;
    }


    public LLNode<T> dequeue(){
        LLNode<T> ret = head;
        if (head != null){
            head = head.next;
        }
        return ret;
    }

}




public enum nodeType {

    Quad, STurn, LTurn, OneSeg, TwoSeg, FourSeg, EightSeg, Key, Pump
};


public class AudioData{

    public int time, freq;
    public GameObject source, audio1, audio2;

    public AudioData(int _freq, int _time, GameObject _source, GameObject _audio1, GameObject _audio2){

        time = _time;
        freq = _freq;
        source = _source;
        audio1 = _audio1;
        //audio2 = _audio2; Not used unless double audiopath tracing is enabled...
    }
}

/*
 * Class for representing the level as a graph.  The graph is used primarily for
 * tracing the path of sounds, thus encapsulating relvent metadata, but could also
 * be used for special AI abilities...
 */

public class AudioNode {

    public static int counter;

    public nodeType type;
    public ArrayList neighbors;
    public Vector3 location, forwards;
    public bool dest, source, visited, closed;
    public int cost, id;
    public float strength;


    public AudioNode(nodeType _type, Vector3 _loc, Vector3 _forwards){

        type = _type;
        neighbors = new ArrayList();

        location = _loc;
        forwards = _forwards;

        strength = 0;

        dest = false;
        source = false;
        visited = false;
        closed = false;

        id = counter++;

        //Costs are hardcoded and will eventually be used in place of gscore calculation...

        switch (_type){

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

                cost = 5;
                break;

            case nodeType.Key:

                cost = 1;
                break;
        }
    }
    
    /*
     * Returns list of locations of possible neighboring nodes for each node type.
     */

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


    /*
     * Return an AudioNode derived from a segment of tunnel or room.
     */

    public static AudioNode AudioNodeFromObj(GameObject obj){
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


    //Returns an array list of neighbors and adds the neighbors to the objects list

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

    
    /*
     * Casts a ray to find a neighboring level segment.
     */

    private AudioNode castDetect(Dictionary<Vector3, AudioNode> map, Vector3 pos){

        RaycastHit hit;
        AudioNode anuda1, result;

        if (Physics.Raycast(pos, Vector3.down, out hit, 10000)){

            anuda1 = AudioNodeFromObj(hit.collider.gameObject);

            if (!map.ContainsKey(hit.collider.gameObject.transform.position)){
            
                result = anuda1;
                map.Add(hit.collider.gameObject.transform.position, anuda1);
            }
            else{
                result = map[anuda1.location];
            }
        }
        else result = null;

        return result;
    }
}


/*
 * Monobehaviour responsible for tracing the path of a sound from source to
 * player via a star search.  Uses the classes above extensivley;
 */

public class AudioPlanner : MonoBehaviour {

    Dictionary<Vector3, AudioNode> hashMap = new Dictionary<Vector3, AudioNode>();

    ArrayList check = new ArrayList(), toClear = new ArrayList();
    Queue<AudioNode> queue = new Queue<AudioNode>();
    
    RaycastHit hit, hit1, hit2;
    int layerMask = ~((1 << 9) + (1 << 10));

    float timer = 0;

    Vector3[] searchResult;

    Vector3 toPlayer;

    public int dieDist, decayRate, startStrength;

    ArrayList requests = new ArrayList(), constants = new ArrayList();

    GameObject brain;


    public void Start(){

        //Init audio graph from random node.
        initAudioGraph(AudioNode.AudioNodeFromObj(GameObject.Find("seg_four").transform.GetChild(1).gameObject));

        //Obselete soon
        brain = GameObject.Find("AIBrain");

        StartCoroutine("ExecuteAudio");
    }


    /*
     * Coroutine for executing searches on the audiograph. Executes a search
     * every frame, unless the queue of search requests is empty.
     */

    IEnumerator ExecuteAudio(){

        bool toPrint = false;

        while (true){

            foreach (AudioData datum in requests){

                if (datum.freq == 1){
                    toPrint = true;
                }

                AudioNode curr = position(datum.source.transform.position + .1f * Vector3.forward + .1f * Vector3.right);

                if (curr == null){
                    continue;
                }

                searchResult = audioSearch(curr, toPrint);

                if (searchResult[0] != Vector3.zero){
                    datum.audio1.transform.position = searchResult[0];
                }

                if (searchResult[1] != Vector3.zero){
                    datum.audio1.transform.position = searchResult[1];
                }
                toPrint = false;
            }

            yield return new WaitForSeconds(2.0f);
        }

    }


    /*
     * Build the graph by populating each node's neighbors array with nearby
     * tunneling.  Done by ray casting method initially chosen when level
     * was to have multiple floors. 
     */

    private void initAudioGraph(AudioNode first) {

        AudioNode curr;
        hashMap.Add(first.location, first);
        ArrayList neighbors, _toClear = new ArrayList();

        queue.Enqueue(first);

        while (queue.Count > 0){

            curr = queue.Dequeue();

            _toClear.Add(curr);

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
        clearNodes(_toClear);
    }


    /*
     * Execute an a* search through the audio graph.  Returns the location of the
     * node adjacent to the player.  Should be optimized in a C++ library to allow
     * for multiple traces.  Cache locality also not optimized here.
     *
     * ~~Needs update for early sound dieout.
     */

    public Vector3 audio_astar(AudioNode source, AudioNode dest, out float dist){

        LLQueue<AudioNode> q = new LLQueue<AudioNode>();

        AudioNode next, curr = source;

        float gscore, d, temp_gscore, neighbor_gscore;

        int id, neigh_id;

        bool[] flags = new bool[AudioNode.counter];
        float[] gscores = new float[AudioNode.counter];
        int[] preds = new int[AudioNode.counter];


        for (int i = 0; i < AudioNode.counter; i++){
            gscores[i] = 100000000000.0f;
        }


        while (curr != null){

            id = curr.id;
            gscore = gscores[id];

            foreach (AudioNode node in curr.neighbors){

                neigh_id = node.id;

                if (!flags[neigh_id]) { continue; }

                d = Vector3.Magnitude(curr.location - node.location);

                temp_gscore = gscore + d;
                neighbor_gscore = gscores[neigh_id];

                if (temp_gscore < neighbor_gscore) {

                    preds[neigh_id] = id;
                    gscores[neigh_id] = temp_gscore;

                    if (!flags[neigh_id]) {
                        q.insert(node, temp_gscore + Vector3.Magnitude(dest.location - node.location));
                    }
                }
            }

            next = q.dequeue().node;

            if (next == dest){
                dist = gscores[id];
                return curr.location;
            }

            flags[curr.id] = true;
            curr = next;
        }

        dist = -1;
        return Vector3.zero;
    }

    /*
    public Vector3[] audioSearch(AudioNode source, bool print){

        Vector3[] result = new Vector3[2];
        AudioNode dest = position(gameObject.transform.position), curr = null;

        if (source == null){
            Debug.Log("source null");
        }

        if (dest == null){
            Debug.Log("dest null");
        }


        if (source.location.Equals(dest.location)){
            result[0] = source.location;
            return result;
        }

        if (Vector3.Magnitude(source.location - dest.location) >= dieDist){
            return result;
        }

        toClear.Add(source);
        toClear.Add(dest);

        int index = 0, vol = 0;

        bool stop = false;

        source.source = true; dest.dest = true;
        source.strength = (int)startStrength;


        queue.Enqueue(source);

        while (queue.Count > 0 && stop == false) {

            curr = queue.Dequeue();
            toClear.Add(curr);

            if (Vector3.Magnitude(curr.location - source.location) < dieDist) {

                //if (print) { Debug.Log("Visiting...."); }

                vol = (int) curr.strength - decayRate * curr.cost;

                foreach (AudioNode node in curr.neighbors) {

                    if (node == null || vol <= node.strength || vol <= 0 || node.closed) { 
                        continue;
                    }

                    if (!node.dest) {
                        //if (print) { Debug.Log("lil more"); }
                        node.strength = vol;
                        queue.Enqueue(node);
                    }   
                    else {
                        //Debug.Log("Adding to result"); 
                        result[index++] = curr.location;
                        stop = true;
                    }
                }
            }
            else{
               // Debug.Log("Agent out of range...");
            }
        }

        queue.Clear();
        clearNodes(toClear);
        return result;
    }*/


    private AudioNode position(Vector3 pos){
        if (Physics.Raycast(pos, Vector3.down, out hit, 10000, layerMask)){
            return (hashMap[hit.collider.transform.position]);
        }
        else return null;
    }


    public void requestSearch(AudioData req){
        requests.Add(req);
    }


    public void Ray(Vector3 pos, Color color){
        Debug.DrawRay(pos + Vector3.up * 10, 1000 * Vector3.down, color, 3);
    }

    //No longer used...
    public void clearNodes(ArrayList nodes) { 

        foreach (AudioNode node in nodes){

            node.dest = false;
            node.source = false;
            node.strength = 0;
            node.visited = false;
        }
        nodes.Clear();
    }
}
