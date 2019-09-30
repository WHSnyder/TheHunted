using UnityEngine;
using UnityEngine.SceneManagement; 


public class Flasher : MonoBehaviour {

    Vector2 mDir;

    float buttonCooler;
    int buttonCount, speed;

    private Vector3 moveDirectionUp = Vector3.zero;

    private float jump = 5.0f, gravity = 9.8f;

    //Dan adds 5/12
    private bool win, dead, canTeleport, hasKey;
    private float lightDist, lightAngle, spotAngle, time = 5.0f, transparency;

    private CharacterController control;

    public GameObject crumb, cam, flashlight;

    private RaycastHit caster;


    // Start is called before the first frame update
    void Start(){

        control = GetComponent<CharacterController>();

        //Find camera and set position/parent
        cam = GameObject.Find("Main Camera");
        Vector3 cameraSpawn = transform.position + .2f * Vector3.forward;
        cam.transform.SetPositionAndRotation(cameraSpawn, Quaternion.identity);
        cam.transform.parent = transform;

        Vector3 lightPos = this.transform.position + .25f * Vector3.right + .65f * Vector3.forward + .15f*Vector3.down;

        flashlight = GameObject.Find("device").gameObject;         flashlight.transform.SetPositionAndRotation(lightPos, Quaternion.Euler(0, 0, 0));         flashlight.transform.parent = GameObject.Find("Main Camera").gameObject.transform;     }



    // Update is called once per frame
    void Update(){
    
        setMouseParams();
        setMovementParams();
        //EnemyStun();
        
        if (win || dead) countdown();
        
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

        // Add new movement to current mouse direction.
        mDir += new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

        // Rotate head up or down.
        cam.transform.localRotation = Quaternion.AngleAxis(-mDir.y, Vector3.right);

        // Rotate body left or right.
        gameObject.transform.localRotation = Quaternion.AngleAxis(mDir.x*2, Vector3.up);
    }


    private void setMovementParams(){

        if (Input.GetKeyDown("w") || Input.GetKeyDown("s")) speed = move();

        //jump if grounded
        if (Input.GetKeyDown("space") && control.isGrounded) moveDirectionUp.y = jump;
        
        if (buttonCooler > 0.0f) buttonCooler -= 1.0f * Time.deltaTime;
        else buttonCount = 0;

        float x = Input.GetAxis("Horizontal") * Time.deltaTime * 150.0f;
        float ver = Input.GetAxis("Vertical") * Time.deltaTime * speed;

        transform.Rotate(0, x, 0);
        control.Move(transform.forward * ver*2);

        if (Input.GetKey("a")) control.Move(transform.right * Time.deltaTime * -2.0f);
        if (Input.GetKey("d")) control.Move(transform.right * Time.deltaTime * 2.0f);

        //jumps
        moveDirectionUp.y -= gravity * Time.deltaTime;
        control.Move(moveDirectionUp * Time.deltaTime);
    }



    private int move() {

        if (buttonCooler > 0.0f && buttonCount == 1){
            if (dead || win) return 0;
            return 3;
        }
        
        buttonCooler = 0.5f;
        buttonCount += 1;

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