using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public enum nodeType {

    Quad, STurn, LTurn, OneSeg, TwoSeg, FourSeg, EightSeg, Key, Pump
};


public class AudioData
{

    public int time, freq;
    public GameObject source, audio1, audio2;

    public AudioData(int _freq, int _time, GameObject _source, GameObject _audio1, GameObject _audio2){

        time = _time;
        freq = _freq;
        source = _source;
        audio1 = _audio1;
        audio2 = _audio2;
    }
}



public class AudioNode{

    public nodeType type;
    public ArrayList neighbors;
    public Vector3 location, forwards;
    public bool dest, source, visited, closed;
    public int cost;
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

        if (Physics.Raycast(pos, Vector3.down, out hit, 10000)){

            anuda1 = AudioNodeFromObj(hit.collider.gameObject);

            if (!map.ContainsKey(hit.collider.gameObject.transform.position)){
            
                result = anuda1;
                map.Add(hit.collider.gameObject.transform.position, anuda1);
            }
            else{
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

    ArrayList check = new ArrayList(), toClear = new ArrayList();
    Queue<AudioNode> queue = new Queue<AudioNode>();

    public GameObject init;

    RaycastHit hit, hit1, hit2;
    int layerMask = ~(1 << 9);

    float timer = 0;

    Vector3[] searchResult;

    public int dieDist, decayRate, startStrength;

    ArrayList requests = new ArrayList(), constants = new ArrayList();



    public void Start(){
        initAudioGraph(AudioNode.AudioNodeFromObj(init));
    }



    public void Update(){

        timer += Time.deltaTime;
        bool toPrint = false; ;

        if (timer > .5) {

            timer = 0;

            foreach (AudioData datum in requests) {

                if (datum.freq == 1){
                    toPrint = true;
                }

                searchResult = audioSearch(position(datum.source.transform.position), toPrint);

                if (searchResult[0] != Vector3.zero){
                    datum.audio1.transform.position = searchResult[0];
                }

                if (searchResult[1] != Vector3.zero){
                    datum.audio1.transform.position = searchResult[1];
                }
                toPrint = false;
            }
        }
    }


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


    public Vector3[] audioSearch(AudioNode source, bool print){


        Vector3[] result = new Vector3[2];
        AudioNode dest = position(gameObject.transform.position), curr = null;

        if (source.location.Equals(dest.location)){
            result[0] = source.location;
            return result;
        }

        if (Vector3.Magnitude(source.location - dest.location) >= dieDist)
        {
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


                    //if (node.dest) { Debug.Log("Adding to result"); }


                    if (!node.dest) {
                        //if (print) { Debug.Log("lil more"); }
                        node.strength = vol;
                        queue.Enqueue(node);
                    }   
                    else {
                       // Debug.Log("Adding to result"); 
                        result[index++] = curr.location;
                        if (index == 2) { stop = true; break; }
                    }
                }
            }
            else
            {
                Debug.Log("Agent out of range...");
            }
        }

        queue.Clear();
        clearNodes(toClear);
        return result;
    }


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