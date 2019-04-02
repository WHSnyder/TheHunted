using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Audio;

public class Hunted : NetworkBehaviour
{

    //movement fields
    float ButtonCooler = 0.0f;
    int ButtonCount = 0;
    int speed = 0;

    //Who the fuck knows
    private Animator anim;
    int jumpHash = Animator.StringToHash("Base Layer.Jump");
    int walkHash = Animator.StringToHash("Base Layer.Movement");


    public AudioClip stepSound;
    private AudioSource source;
    private float volLowRange = .5f;
    private float volHighRange = 1.0f;


    public Rigidbody rb;


    // Start is called before the first frame update
    void Start() {

        if (isLocalPlayer)
        {

            anim = GetComponent<Animator>();
            anim.SetFloat("MoveSpeed", 0f);
            //I think land state was auto set to change...
            source = GetComponent<AudioSource>();

            rb = GetComponent<Rigidbody>();

            GameObject.Find("Main Camera").gameObject.transform.parent = this.transform;
        }
        else return;
    }


    // Update is called once per frame

    void Update() {

        if (!isLocalPlayer) return;

        if (Input.GetKeyDown("w")) speed = move();

        if (Input.GetKey("w") == false && ButtonCooler <= 0.0f)
        {
            anim.SetFloat("MoveSpeed", 0.0f);
        }

        if (Input.GetKeyDown("space"))
        {
            anim.Play(jumpHash);
            rb.AddForce(Vector3.up * 200);
        }

        if (ButtonCooler > 0.0f) ButtonCooler -= 1.0f * Time.deltaTime;
        else ButtonCount = 0;

        float x = Input.GetAxis("Horizontal") * Time.deltaTime * 150.0f;
        float ver = Input.GetAxis("Vertical") * Time.deltaTime * speed;

        transform.Rotate(0, x, 0);
        transform.Translate(0, 0, ver);
    }


    public int move() { //remember to credit him...
    
        if (ButtonCooler > 0.0f && ButtonCount == 1) {
            print("Double tap!");
            anim.SetFloat("MoveSpeed", 1.0f);

            return 6;
        }
        else {
            ButtonCooler = 0.5f;
            ButtonCount += 1;
            anim.SetFloat("MoveSpeed", .3f);

            return 2;
        }
    }

    public void Step() { source.PlayOneShot(stepSound,1f); }
}
