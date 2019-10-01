using UnityEngine;


public class LightDevice : MonoBehaviour{

    private Light source;
    private GameObject beam,bounce;

    private bool on;
    private float power = 200.0f;

    Vector3 shot, refl_forward;
    Transform objhit;
    RaycastHit hit;
    private Material beam_material,bounce_material;



    // Start is called before the first frame update
    void Start(){

        beam = GameObject.Find("light_cone");
        beam_material = beam.GetComponent<Renderer>().material;

        bounce = GameObject.Find("light_cone_bounce");
        bounce_material = bounce.GetComponent<Renderer>().material;

        source = GameObject.Find("light").GetComponent<Light>(); 
        source.enabled = false;
        source.intensity = 0;

        beam.SetActive(false);
        bounce.SetActive(false);
    }



    // Update is called once per frame
    void Update(){

        if (Input.GetMouseButtonDown(0)){
            source.enabled = !source.enabled;
            on = !on;
            beam.SetActive(on);
            source.intensity = 0;
            bounce.SetActive(on);
        }

        if (on){
            source.intensity = 60;

            if (Physics.Raycast(transform.position, transform.forward, out hit, 40, 1 << 11)){

                objhit = hit.transform;

                refl_forward = Vector3.Reflect(transform.forward, hit.normal);

                bounce.SetActive(true);
                bounce.transform.position = hit.point;
                bounce.transform.forward = refl_forward;
                bounce_material.color = objhit.gameObject.GetComponent<Renderer>().material.color * beam_material.color;
            }
            else bounce.SetActive(false);
        }

        if (power <= 0.0f){
            source.enabled = false;
            beam.SetActive(false);
            //bounce.enabled = false;
            on = false;
        }

        if (on && (power > 0.0f)){
            //power -= 0.05f;
        } 
    }
}
