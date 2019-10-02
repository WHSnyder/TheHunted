using UnityEngine;

/*
 * Class for controlling the player's device.
 */

public class LightDevice : MonoBehaviour{

    private Light source, caselight;
    private GameObject beam,bounce,current_light;
    Color case_color;

    private bool on;
    private float power = 0.0f, drain_time, drain_amt, spin_time, spin_speed, on_time;

    Vector3 shot, refl_forward;
    Transform objhit;
    RaycastHit hit;
    private Material beam_material,bounce_material, bulbmaterial,lightcasematerial;



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

        foreach (GameObject lightcase in GameObject.FindGameObjectsWithTag("lightcase")){
            Color color = lightcase.transform.Find("Point Light").gameObject.GetComponent<Light>().color;
            lightcase.transform.Find("Sphere").gameObject.GetComponent<Renderer>().material.SetColor("_EmissionColor", 3*color);
        }

        bulbmaterial = transform.Find("Bulb").gameObject.GetComponent<Renderer>().material;
        bulbmaterial.SetColor("_EmissionColor", Color.black);
        beam_material.color = Color.black;
    }



    // Update is called once per frame
    void Update(){

        if (Input.GetMouseButtonDown(0)){
            source.enabled = !source.enabled;
            on = !on;
            beam.SetActive(on);
            source.intensity = 0;
            bounce.SetActive(on);
            on_time = 0.0f;
        }

        if (on){

            power -= Time.deltaTime * .1f;
            power = Mathf.Clamp(power, 0.0f, 1.0f);
            beam_material.color *= power;

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

        if (Input.GetMouseButton(1)){
            if (Physics.Raycast(transform.position, transform.forward, out hit, 40, 1 << 12)){

                objhit = hit.transform;
                if (current_light != objhit.gameObject){
                    current_light = objhit.gameObject;
                    lightcasematerial = objhit.Find("Sphere").gameObject.GetComponent<Renderer>().material;
                    caselight = objhit.Find("Point Light").gameObject.GetComponent<Light>();
                    drain_time = 0.0f;
                    drain_amt = 0.0f;
                    case_color = caselight.color;
                    Debug.Log("Hit a new light");
                }
                drain_amt = Time.deltaTime * .2f;
                //drain_amt = Mathf.Clamp(drain_amt, 0.0f, 1.0f);

                //set the color of the emissive sphere
                lightcasematerial.SetColor("_EmissionColor", 3.0f*(case_color - drain_amt * case_color));

                //set the color of the point light
                caselight.color = case_color - drain_amt * case_color;

                bulbmaterial.SetColor("_EmissionColor",  drain_amt * 3.0f * case_color);
                beam_material.color = drain_amt * case_color;
            }
        }
        else current_light = null;

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
