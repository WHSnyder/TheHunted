using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Randomizer : MonoBehaviour
{

    private GameObject i1;
    private GameObject i2;
    private GameObject i3;
    private GameObject i4;
    private GameObject i5;
    private GameObject key;
    private GameObject[] batteryList; 
    private int random;
    private int random2; 
    private Vector3 loc1 = new Vector3(75.0f, 0.0f, 15.0f);
    private Vector3 loc2 = new Vector3(116.0f, 0.0f, -15.0f);
    private Vector3 loc3 = new Vector3(-64.0f, 0.0f, 57.0f);
    private Vector3 loc4 = new Vector3(-123.0f, 0.0f, 57.0f);
    private Vector3 loc5 = new Vector3(-186.0f, 0.0f, -42.0f);
    private Vector3 banished = new Vector3(0.0f, -100.0f, 0.0f);


    // Start is called before the first frame update
    void Start()
    {
        //room stuff 
        Random.InitState(System.DateTime.Now.Millisecond);
        i1 = GameObject.Find("item_room1");
        i2 = GameObject.Find("item_room2");
        i3 = GameObject.Find("item_room3");
        i4 = GameObject.Find("item_room4");
        i5 = GameObject.Find("item_room5");
        //key = GameObject.Find("key_room"); 

        random = Random.Range(1, 6);
        Debug.Log(random);
        if (random == 1) {
            i1.transform.position = banished; 
            //key.transform.position = loc1; 
        } 
        else if (random == 2){
            i2.transform.position = banished;
            //key.transform.position = loc2; 
        }
        else if (random == 3)
        {
            i3.transform.position = banished;
            //key.transform.position = loc3; 
        }
        else if (random == 4)
        {
            i4.transform.position = banished;
            //key.transform.position = loc4; 
        } 
        else {
            i5.transform.position = banished;
            //key.transform.position = loc5; 
        }


        //battery stuff 
        //iterate through all objects with tag battery 
        // randomly choose 50,50 to decide whether or not the battery will appear 
        batteryList = GameObject.FindGameObjectsWithTag("Battery"); 
        for (int x = 0; x < batteryList.Length; x++) {
            random2 = Random.Range(1, 5); 
            if (random2 < 3) {
                Destroy(batteryList[x]); 
            } 
            
        }
    }
}
