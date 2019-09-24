using UnityEngine;


public class FLSource : MonoBehaviour{

    public Light source;
    public GameObject beam,bounce;

    private bool on;
    private float power = 200.0f;

    Vector3 shot, reflection;
    RaycastHit hit;



    // Start is called before the first frame update
    void Start(){

        beam = GameObject.Find("light_cone");
        bounce = GameObject.Find("light_cone_bounce");

        source = GameObject.Find("light").GetComponent<Light>(); 
        source.enabled = false;
        source.intensity = 0;

        beam.SetActive(false);
        bounce.SetActive(false);
    }



    // Update is called once per frame
    void Update(){

        if (Input.GetKeyDown(KeyCode.F)){
            source.enabled = !source.enabled;
            //bounce.enabled = !bounce.enabled;
            on = !on;
            beam.SetActive(on);
            source.intensity = 0;

            if (on){
                source.intensity = 10;
            }
        }

        if (power <= 0.0f){
            source.enabled = false;
            beam.SetActive(false);
            //bounce.enabled = false;
            on = false;
        }

        if (on && (power > 0.0f)){
            power -= 0.05f;
        } 
    }
}
