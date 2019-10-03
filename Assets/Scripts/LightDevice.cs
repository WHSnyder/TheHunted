using UnityEngine;

/*
 * Class for controlling the player's device.
 */

public class LightDevice : MonoBehaviour{

    private Light beam_light, bounce_light, caselight;
    private GameObject beam,bounce,current_light;
    Color case_color;

    private bool on;
    private float power = 0.0f, drain_time, drain_amt, spin_time, spin_speed, on_time, drain_intensity, recip = 1/255.0f,frac;

    Vector3 shot, refl_forward;
    Transform objhit;
    RaycastHit hit;
    private Material beam_material,bounce_material, bulbmaterial,lightcasematerial;

    private Color bulb_color;

    private Color[] colors = { Color.blue, Color.red, Color.green, Color.cyan, Color.magenta, Color.yellow, Color.white };


    // Start is called before the first frame update
    void Start(){

        beam = GameObject.Find("light_cone");
        beam_material = beam.GetComponent<Renderer>().material;

        beam_light = beam.transform.Find("light").gameObject.GetComponent<Light>();
        beam_light.enabled = false;
        beam_light.intensity = 0;

        bounce = GameObject.Find("light_cone_bounce");
        bounce_material = bounce.GetComponent<Renderer>().material;
        bounce_light = bounce.transform.Find("Spot Light").gameObject.GetComponent<Light>();
        bounce_light.intensity = 0;

        beam.SetActive(false);
        bounce.SetActive(false);

        Random.InitState(10344);

        foreach (GameObject lightcase in GameObject.FindGameObjectsWithTag("lightcase")){
            Color color = colors[ Mathf.FloorToInt( Random.Range(0.0f,7.0f) )];
            lightcase.transform.Find("Point Light").gameObject.GetComponent<Light>().color = color;
            lightcase.transform.Find("Sphere").gameObject.GetComponent<Renderer>().material.SetColor("_EmissionColor", 3*color);
        }

        bulbmaterial = transform.Find("Bulb").gameObject.GetComponent<Renderer>().material;
        bulbmaterial.SetColor("_EmissionColor", Color.black);
        beam_material.color = Color.black;
    }



    // Update is called once per frame
    void Update(){

        if (Input.GetKeyDown("f") || Input.GetKeyUp("f")){
            on = !on;

            beam.SetActive(on);
            beam_light.intensity = 1.0f;
            beam_light.enabled = on;

            bounce.SetActive(on);
            bounce_light.enabled = on;

            current_light = null;
        }


        if (on && power > .0001f){

            EnemyStun();

            drain_amt = Time.deltaTime;
            power -= drain_amt;

            //Sets color of bulb on player's device
            bulbmaterial.SetColor("_EmissionColor", 2.0f * .3f * power * bulb_color);

            //Sets color of light beam mesh
            beam_material.color = 1.0f * .3f * power * bulb_color;

            //Sets color and intensity of beam's spotlight 
            beam_light.color = bulb_color;
            beam_light.intensity = 180.0f * .3f * power;


            if (Physics.Raycast(transform.position, transform.forward, out hit, 40, 1 << 11)){ //11 = reflectives

                objhit = hit.transform;

                refl_forward = Vector3.Reflect(transform.forward, hit.normal);

                //Active the bounce beam
                bounce.SetActive(true);
                bounce.transform.position = hit.point;
                bounce.transform.forward = refl_forward;

                //Set color of bounce beam mesh
                bounce_material.color = objhit.gameObject.GetComponent<Renderer>().material.color * beam_material.color;

                //Set color and intensity of bounce's spot light
                bounce_light.enabled = true;
                bounce_light.intensity = beam_light.intensity;
                bounce_light.color = bounce_material.color;
            }
            else {

                bounce.SetActive(false);
                bounce_light.enabled = false;

                //For giving light back 
                if (Physics.Raycast(transform.position, transform.forward, out hit, 40, 1 << 12)){ //12 = lights

                    objhit = hit.transform;

                    if (current_light != objhit.gameObject){

                        current_light = objhit.gameObject;
                        lightcasematerial = objhit.Find("Sphere").gameObject.GetComponent<Renderer>().material;
                        caselight = objhit.Find("Point Light").gameObject.GetComponent<Light>();
                    }

                    //set the color  of the point light
                    caselight.intensity = Mathf.Clamp(caselight.intensity + 50.0f * drain_amt, 0.0f, 300.0f);
                    caselight.color += .5f * (-1.0f * drain_amt * caselight.color + drain_amt * bulb_color);

                    //set the color of the emissive sphere
                    lightcasematerial.SetColor("_EmissionColor", .015f * caselight.intensity * caselight.color);
                }
            }
        }

        if (Input.GetMouseButton(0)){
            if (Physics.Raycast(transform.position, transform.forward, out hit, 40, 1 << 12)){

                objhit = hit.transform;

                if (current_light != objhit.gameObject){
                    current_light = objhit.gameObject;
                    lightcasematerial = objhit.Find("Sphere").gameObject.GetComponent<Renderer>().material;
                    caselight = objhit.Find("Point Light").gameObject.GetComponent<Light>();

                    drain_intensity = caselight.intensity;

                    case_color = caselight.color;
                    bulb_color = Color.black;
                }

                drain_amt = 2.0f * Time.deltaTime;

                //set the color  of the point light
                caselight.intensity = Mathf.Clamp(caselight.intensity - 100.0f*drain_amt, 0.0f, 300.0f);

                //set the color of the emissive sphere
                lightcasematerial.SetColor("_EmissionColor", .01f * caselight.intensity * case_color);

                //set color of bulb on flashlight
                bulb_color += .3f * drain_amt * case_color;
                bulbmaterial.SetColor("_EmissionColor", 2.0f * bulb_color);

                power += drain_amt;//  Mathf.Clamp(power + drain_amt, 0.0f, 3.0f);
            }
        }
        else current_light = null;
    }


    private bool EnemyStun(){

        if (Physics.Raycast(transform.position, transform.forward, out hit, 30, 1 << 10)){ //Heads lol
            GameObject head = hit.collider.gameObject;
            head.GetComponent<HeadRef>().slist.GetComponent<EnemyScript>().processCommand(Vector3.zero, EvilState.Stunned);
            return true;
        }
        return false;
    }
}
