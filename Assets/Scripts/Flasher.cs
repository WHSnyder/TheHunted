﻿using UnityEngine;
using UnityEngine.SceneManagement; 


public class Flasher : MonoBehaviour{

    Vector2 mDir;

    private Vector3 keyLoc;

    float ButtonCooler;
    int ButtonCount;
    int speed;

    private Vector3 moveDirectionUp = Vector3.zero;

    private float jump = 5.0f, gravity = 9.8f;

    //Dan adds 5/12
    private bool win, dead, canTeleport, hasKey;
    private float lightDist, lightAngle, spotAngle, time = 5.0f, transparency;

    private CharacterController control;

    public GameObject crumb, key, cam, flashlight;

    private RaycastHit caster;


    // Start is called before the first frame update
    void Start(){

        key = GameObject.Find("key");
        keyLoc = key.transform.position; 

        control = GetComponent<CharacterController>();

        //Find camera and set position/parent
        cam = GameObject.Find("Main Camera");
        Vector3 cameraSpawn = transform.position + .2f * Vector3.forward;
        cam.transform.SetPositionAndRotation(cameraSpawn, Quaternion.identity);
        cam.transform.parent = transform;

        Vector3 lightPos = this.transform.position + .25f * Vector3.right + .65f * Vector3.forward + .15f*Vector3.down;

        flashlight = GameObject.Find("flashlight_withcone").gameObject;         flashlight.transform.SetPositionAndRotation(lightPos, Quaternion.Euler(0, 0, 0));         flashlight.transform.parent = GameObject.Find("Main Camera").gameObject.transform;     }



    // Update is called once per frame
    void Update(){
    
        setMouseParams();
        setMovementParams();
        //EnemyStun();
        
        if (win || dead) countdown();
        
        if (Input.GetKeyDown("m")) {
            transform.position = GameObject.Find("key_room_locked").transform.position + .1f*Vector3.up;
        }

        if (Input.GetKeyDown("l")){
            transform.position = GameObject.Find("Door").transform.position + 2 * GameObject.Find("Door").transform.up;
        }

        /*if (!win) {
            GameObject[] enemyList = GameObject.FindGameObjectsWithTag("Enemy");
            for (int z = 0; z < enemyList.Length; z++) { 
                if ((Vector3.Magnitude(transform.position - enemyList[z].transform.position) < 2)
                        || (transform.position.y < -10.0f)) {
                    if (enemyList[z].GetComponent<EnemyScript>().currState != EvilState.Stunned){
                        dead = true;
                        win = false;
                        countdown();// setInfoText();
                    }
                }
            }
        }*/ 

        if (Input.GetKeyDown("q")) SceneManager.LoadScene("MainMenu");
    }


    private bool EnemyStun() {
        if (Physics.Raycast(flashlight.transform.position, flashlight.transform.forward, out caster, 30, 1<<10)) { 
            GameObject head = caster.collider.gameObject;
            head.GetComponent<HeadRef>().slist.GetComponent<EnemyScript>().processCommand(Vector3.zero, EvilState.Stunned);
            return true;
        }
        return false;
    }


    private void setMouseParams(){

        Vector2 mc = new Vector2(Input.GetAxisRaw("Mouse X"),Input.GetAxisRaw("Mouse Y"));

        // Add new movement to current mouse direction.
        mDir += mc;

        // Rotate head up or down.
        cam.transform.localRotation = Quaternion.AngleAxis(-mDir.y, Vector3.right);

        // Rotate body left or right.
        gameObject.transform.localRotation = Quaternion.AngleAxis(mDir.x*2, Vector3.up);
    }


    private void setMovementParams(){

        if (Input.GetKeyDown("w") || Input.GetKeyDown("s")) speed = move();

        //jump if grounded
        if (Input.GetKeyDown("space") && control.isGrounded) moveDirectionUp.y = jump;
        
        if (ButtonCooler > 0.0f) ButtonCooler -= 1.0f * Time.deltaTime;
        else ButtonCount = 0;

        float x = Input.GetAxis("Horizontal") * Time.deltaTime * 150.0f;
        float ver = Input.GetAxis("Vertical") * Time.deltaTime * speed;

        transform.Rotate(0, x, 0);
        control.Move(transform.forward * ver*2);

        //jumps
        moveDirectionUp.y -= gravity * Time.deltaTime;
        control.Move(moveDirectionUp * Time.deltaTime);
    }



    private int move() {

        if (ButtonCooler > 0.0f && ButtonCount == 1){
            if (dead || win) return 0;
            return 3;
        }
        
        ButtonCooler = 0.5f;
        ButtonCount += 1;

        if (dead || win) return 0;

        return 1;
    }


    void countdown() {
        if (time >= 0.0f) {
            //var copyCol2 = img.color;
            //transparency += .005f;
            //copyCol2.a = transparency;
            //img.color = copyCol2;
            time -= Time.deltaTime; 
        } 
        else  SceneManager.LoadScene("MainMenu");
    }
}