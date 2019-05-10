using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flasher : MonoBehaviour{


    Vector3 mousePos;

    Vector2 mDir;

    float ButtonCooler = 0.0f;
    int ButtonCount = 0;
    int speed = 0;

    private Vector3 moveDirectionUp = Vector3.zero;

    private float jump = 5.0f;
    private float gravity = 9.8f;


    private CharacterController control;

    public GameObject crumb;


    // Start is called before the first frame update
    void Start(){


        control = GetComponent<CharacterController>();
        crumb = Resources.Load("BreadCrumb") as GameObject;

        Vector3 cameraSpawn = this.transform.position + .1f * Vector3.forward;
        GameObject.Find("Main Camera").gameObject.transform.SetPositionAndRotation(cameraSpawn, Quaternion.identity);
        GameObject.Find("Main Camera").gameObject.transform.parent = this.transform;

        //Vector3 lightPos = this.transform.position + .25f * Vector3.right + .25f*Vector3.forward;
        //GameObject.Find("FlashLight").gameObject.transform.SetPositionAndRotation(lightPos, Quaternion.Euler(0, 0, 0));
        //GameObject.Find("FlashLight").gameObject.transform.parent = this.transform;

    }

    // Update is called once per frame
    void Update(){
        setMouseParams();
        setMovementParams();
    }




    private void setMouseParams(){

        Vector2 mc = new Vector2(Input.GetAxisRaw("Mouse X"),
            Input.GetAxisRaw("Mouse Y"));

        // Add new movement to current mouse direction.
        mDir += mc;

        // Rotate head up or down.
        // This rotates the camera on X-axis.
        this.transform.localRotation =
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
        if (ButtonCooler > 0.0f && ButtonCount == 1){return 5;}
        else{
            ButtonCooler = 0.5f;
            ButtonCount += 1;
            return 3;
        }
    }


    public void Step(){
        //source.PlayOneShot(stepSound, 1f);
    }

    public void dropBread(){
        Vector3 v = new Vector3(transform.position.x, transform.position.y - .1f,
                                    transform.position.z);
        GameObject b = Instantiate(crumb, v, transform.rotation);
    }
}



