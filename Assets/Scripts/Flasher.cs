using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 
using UnityEngine.UI;

public class Flasher : MonoBehaviour{


    Vector3 mousePos;

    Vector2 mDir;

    float ButtonCooler = 0.0f;
    int ButtonCount = 0;
    int speed = 0;

    private Vector3 moveDirectionUp = Vector3.zero;

    private float jump = 5.0f;
    private float gravity = 9.8f;

    //Dan adds 5/12
    private GameObject door;
    private bool win = false;
    private bool dead = false; 
    private float time = 5.0f;
    private float transparency = 0.0f;
    private float power = 100.0f;



    private CharacterController control;

    public Text victoryText;
    public Text powerText;
    public Text objectiveText; 
    public GameObject crumb;
    private GameObject key;
    public Image img;


    // Start is called before the first frame update
    void Start(){

        objectiveText.text = "Find the key";

        door = GameObject.Find("Door");
        key = GameObject.Find("key");  

        var copyCol = img.color;
        copyCol.a = 0.0f;
        img.color = copyCol; 

        control = GetComponent<CharacterController>();
        //acrumb = Resources.Load("BreadCrumb") as GameObject;

        Vector3 cameraSpawn = this.transform.position + .1f * Vector3.forward;
        GameObject.Find("Main Camera").gameObject.transform.SetPositionAndRotation(cameraSpawn, Quaternion.identity);
        GameObject.Find("Main Camera").gameObject.transform.parent = this.transform;

        Vector3 lightPos = this.transform.position + .25f * Vector3.right + .25f * Vector3.forward;         GameObject.Find("FlashLight").gameObject.transform.SetPositionAndRotation(lightPos, Quaternion.Euler(0, 0, 0));         GameObject.Find("FlashLight").gameObject.transform.parent = GameObject.Find("Main Camera").gameObject.transform;          GameObject.Find("Flashlight2").gameObject.transform.SetPositionAndRotation(this.transform.position + .7f * Vector3.forward         + .2f * Vector3.right - .15f * Vector3.up, Quaternion.Euler(90, 0, 0));         GameObject.Find("Flashlight2").gameObject.transform.parent = GameObject.Find("Main Camera").gameObject.transform;
    }

    // Update is called once per frame
    void Update(){

        setMouseParams();
        setMovementParams();

        powerText.text = "Power: " + power; 

        if (win) {
            countdown();
        } 

        if (dead) {
            countdown();
        }

        GameObject [] batteryList  = GameObject.FindGameObjectsWithTag("Battery"); 

        for (int a = 0; a < batteryList.Length; a++) { 
            if (Vector3.Magnitude(transform.position - batteryList[a].transform.position) < 1) {
                Destroy(batteryList[a]);
                power += 25.0f;
            }
        }

        if (GameObject.FindGameObjectsWithTag("Key").Length != 0) {
            if (Vector3.Magnitude(transform.position - key.transform.position) < 1.5)
            {
                objectiveText.text = "Get back to the start!";
                Destroy(key);
            }
        }

        if (GameObject.FindGameObjectsWithTag("Lock").Length == 0) { 
            if (Vector3.Magnitude(transform.position - door.transform.position) < 2) { 
                win = true;
                setInfoText();
                //countdown();

            }
        }  

        if (!win) {
            GameObject[] enemyList = GameObject.FindGameObjectsWithTag("Enemy");
            for (int z = 0; z < enemyList.Length; z++) { 
                if ((Vector3.Magnitude(transform.position - enemyList[z].transform.position) < 2)
                        || (transform.position.y < -10.0f)) {
                    dead = true;
                    win = false; 
                    setInfoText();
                }
            }
        }
    }


    private void setMouseParams(){

        Vector2 mc = new Vector2(Input.GetAxisRaw("Mouse X"),
            Input.GetAxisRaw("Mouse Y"));

        // Add new movement to current mouse direction.
        mDir += mc;

        // Rotate head up or down.
        // This rotates the camera on X-axis.
        GameObject.Find("Main Camera").gameObject.transform.localRotation =
            Quaternion.AngleAxis(-mDir.y, Vector3.right);

        // Rotate body left or right.
        // This rotates the parent body (a capsule), not the camera,
        // on the Y-axis.
        gameObject.transform.localRotation =
            Quaternion.AngleAxis(mDir.x*2, Vector3.up);

        mousePos = Input.mousePosition;

        //Debug.Log("Mouse position: " + mousePos);
    }


    private void setMovementParams(){

        if (Input.GetKeyDown("b")) {
            dropBread();
        }
        if (Input.GetKeyDown("w") || (Input.GetKeyDown("s"))) speed = move();

        //jump if grounded
        if (Input.GetKeyDown("space") && (control.isGrounded)){

            moveDirectionUp.y = jump;
        }

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
            return 4;}
        else{
            ButtonCooler = 0.5f;
            ButtonCount += 1; 
            if ((dead) || (win)) {
                return 0;
            }
            return 2;
        }
    }


    public void dropBread(){
        Vector3 v = new Vector3(transform.position.x, transform.position.y - .3f,
                                    transform.position.z);
        GameObject b = Instantiate(crumb, v, transform.rotation);
    } 

    public void setInfoText() { 
        if (win) {

            victoryText.color = Color.green;
            victoryText.text = "You Escaped!";
            countdown();

        }
        if (dead) {
            victoryText.color = Color.red;
            victoryText.text = "You're Dead";
            countdown();
        }
    } 

    void countdown() {
        if (time >= 0.0f) {
            var copyCol2 = img.color;
            transparency += .005f;
            copyCol2.a = transparency;
            img.color = copyCol2;
            time -= Time.deltaTime; 
        } 
        else {
            SceneManager.LoadScene("MainMenu");
        }
    }
}



