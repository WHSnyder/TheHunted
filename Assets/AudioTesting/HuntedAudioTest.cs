using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;


//Help with jumping 
//https://answers.unity.com/questions/574328/jumping-with-a-character-controller.html

public class HuntedAudioTest : MonoBehaviour
{

    //movement fields
    float ButtonCooler = 0.0f;
    int ButtonCount = 0;
    int speed = 0;

    //Who the fuck knows
    private Animator anim;
    int jumpHash = Animator.StringToHash("Base Layer.Jump");
    int walkHash = Animator.StringToHash("Base Layer.Movement");

    public GameObject crumb;
    public AudioClip stepSound;
    private AudioSource source;
    private float volLowRange = .5f;
    private float volHighRange = 1.0f;
    private Vector3 moveDirectionUp = Vector3.zero;
    private float jump = 5.0f;
    private float gravity = 9.8f;

    private int midair = 0;

    private Rigidbody rb;
    private CharacterController control;

    // Start is called before the first frame update
    void Start()
    {

        control = GetComponent<CharacterController>();



            crumb = Resources.Load("BreadCrumb") as GameObject;

            anim = GetComponent<Animator>();
            anim.SetFloat("MoveSpeed", 0f);
            //I think land state was auto set to change...
            source = GetComponent<AudioSource>();



            //rb = GetComponent<Rigidbody>();
            Vector3 cameraSpawn = this.transform.position - 2 * Vector3.forward + 1.2f * Vector3.up;
            GameObject.Find("Main Camera").gameObject.transform.SetPositionAndRotation(cameraSpawn, Quaternion.identity);
            GameObject.Find("Main Camera").gameObject.transform.parent = this.transform;
            
            Vector3 lightPos = this.transform.position + Vector3.right + .5f * Vector3.up;
            GameObject.Find("FlashLight").gameObject.transform.SetPositionAndRotation(lightPos, Quaternion.Euler(0, 0, 0));
            GameObject.Find("FlashLight").gameObject.transform.parent = this.transform;

    }


    // Update is called once per frame

    void Update(){

        if (Input.GetKeyDown("w") || (Input.GetKeyDown("s"))) speed = move();

        if (Input.GetKey("w") == false && ButtonCooler <= 0.0f)
        {
            anim.SetFloat("MoveSpeed", 0.0f);
        }

        if (Input.GetKeyDown("b"))
        {
            dropBread();
        }

        if (midair == 1)
        {
            if (control.isGrounded)
            {
                anim.SetBool("Grounded", true);
                midair = 0;
            }
        }


        //jump if grounded
        if (Input.GetKeyDown("space") && (control.isGrounded))
        {

            anim.Play(jumpHash);
            moveDirectionUp.y = jump;
            anim.SetBool("Grounded", false);
            midair = 1;
        }

        if (ButtonCooler > 0.0f) ButtonCooler -= 1.0f * Time.deltaTime;
        else ButtonCount = 0;

        float x = Input.GetAxis("Horizontal") * Time.deltaTime * 150.0f;
        float ver = Input.GetAxis("Vertical") * Time.deltaTime * speed;


        transform.Rotate(0, x, 0);
        control.Move(transform.forward * ver);

        //jumps
        moveDirectionUp.y -= gravity * Time.deltaTime;
        control.Move(moveDirectionUp * Time.deltaTime);

        // if (transform.position.y < -1.0f) {
        //     SceneManager.LoadScene("NetworkTest"); 
        // }

    }


    public int move()
    { //remember to credit him...

        if (ButtonCooler > 0.0f && ButtonCount == 1)
        {
            //print("Double tap!");
            anim.SetFloat("MoveSpeed", 1.0f);
            //if () {
            return 5;
            //}
            //else {
            //    return 3;
            //}

        }
        else
        {
            ButtonCooler = 0.5f;
            ButtonCount += 1;
            anim.SetFloat("MoveSpeed", .3f);

            return 3;
        }
    }

    public void Step() { 

        //source.PlayOneShot(stepSound, 1f);
    }

    public void dropBread()
    {
        Vector3 v = new Vector3(transform.position.x, transform.position.y - .1f,
                                    transform.position.z);
        GameObject b = Instantiate(crumb, v, transform.rotation);

    }

}
