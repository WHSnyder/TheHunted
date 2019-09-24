using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 
using UnityEngine.UI;

public class Flasher : MonoBehaviour{

    Vector2 mDir,mousePos;

    private Vector3 keyLoc;

    float ButtonCooler;
    int ButtonCount;
    int speed;

    private Vector3 moveDirectionUp = Vector3.zero;

    private float jump = 5.0f, gravity = 9.8f;

    //Dan adds 5/12
    private GameObject door;
    private GameObject[] teleporters;

    private bool win, dead, canTeleport, hasKey;

    private FLSource lit;
    private float lightDist, lightAngle, spotAngle, time = 5.0f, transparency;

    private CharacterController control;

    public GameObject crumb, key, cam, flashlight;


    Vector3 flBottom, flUR, flUL;

    private RaycastHit caster;
    


    // Start is called before the first frame update
    void Start(){

        door = GameObject.Find("Door"); key = GameObject.Find("key");
        keyLoc = key.transform.position; 

        control = GetComponent<CharacterController>();

        //Find camera and set position/parent
        cam = GameObject.Find("Main Camera");
        Vector3 cameraSpawn = transform.position + .2f * Vector3.forward;
        cam.transform.SetPositionAndRotation(cameraSpawn, Quaternion.identity);
        cam.transform.parent = transform;

        Vector3 lightPos = this.transform.position + .25f * Vector3.right + .65f * Vector3.forward + .15f*Vector3.down;

        flashlight = GameObject.Find("flashlight_withcone").gameObject;         flashlight.transform.SetPositionAndRotation(lightPos, Quaternion.Euler(0, 0, 0));         flashlight.transform.parent = GameObject.Find("Main Camera").gameObject.transform; 
        lit = flashlight.GetComponent<FLSource>();

        Vector3 help = Quaternion.Euler(new Vector3(60, 0, 0)) * flashlight.transform.forward;

        //flBottom = help;
        flUL = Quaternion.Euler(0, 0, 60) * help;
        //flUR = Quaternion.Euler(0, 0, 60) * flUL;

        /*
        Debug.DrawRay(flashlight.transform.position, 2 * flBottom, Color.yellow, 15);
        Debug.DrawRay(flashlight.transform.position, 2 * flUR, Color.green, 15);
        Debug.DrawRay(flashlight.transform.position, 2 * flUL, Color.blue, 15);
        */
    }



    // Update is called once per frame
    void Update(){
    
        setMouseParams();
        setMovementParams();
        //Debug.DrawRay(flashlight.transform.position, flashlight.transform.forward * 20, Color.red);
        //EnemyStun();
        
        if (win) countdown();
        if (dead) countdown();
        
        if (Input.GetKeyDown("m")) {
            transform.position = GameObject.Find("key_room_locked").transform.position + .1f*Vector3.up;
        }

        if (Input.GetKeyDown("l")){
            transform.position = GameObject.Find("Door").transform.position + 2 * GameObject.Find("Door").transform.up;
        }

        if (!win) {
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
        } 

        if (Input.GetKeyDown("q")) {
            SceneManager.LoadScene("MainMenu");
        }
    }


    private bool EnemyStun() {

        if (!lit.source.enabled) return false;
       
        if (Physics.Raycast(flashlight.transform.position, flashlight.transform.forward, out caster, 30)) { 

            if (caster.collider.gameObject.CompareTag("Enemy")){
                GameObject head = caster.collider.gameObject;
                head.gameObject.GetComponent<EnemyScript>().processCommand(Vector3.zero, EvilState.Stunned);
                return true;
            }
            return false;
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

        mousePos = Input.mousePosition;
    }


    private void setMovementParams(){

        if (Input.GetKeyDown("w") || (Input.GetKeyDown("s"))) speed = move();

        //jump if grounded
        if (Input.GetKeyDown("space") && (control.isGrounded)) moveDirectionUp.y = jump;
        

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
            if ((dead) || (win)) {
                return 0;
            }
            return 3;
        }
        else {
            ButtonCooler = 0.5f;
            ButtonCount += 1; 
            if ((dead) || (win)) {
                return 0;
            }
            return 1;
        }
    }


    void countdown() {
        if (time >= 0.0f) {
            //var copyCol2 = img.color;
            //transparency += .005f;
            //copyCol2.a = transparency;
            //img.color = copyCol2;
            time -= Time.deltaTime; 
        } 
        else {
            SceneManager.LoadScene("MainMenu");
        }
    }
}