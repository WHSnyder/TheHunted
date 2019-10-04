using UnityEngine;

/*
 * Class for controlling the player's device.
 */

public class LightDevice : MonoBehaviour{

    private Light beam_light, bounce_light, caselight;
    private GameObject beam,bounce,current_light;
    Color case_color;

    private bool on;
    private float power, drain_amt, spin_time, spin_speed;

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

        Random.InitState((int)System.DateTime.Now.Ticks);

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

        if (power > .0001f && Input.GetKeyDown("f")){
            on = !on;
            SetLights(on);
        }
        
        if (on && power < .0001f){
            on = false;
            SetLights(on);
        }

        if (on){

            EnemyStun();

            drain_amt = .2f * Time.deltaTime;
            power -= drain_amt;

            //Sets color of bulb on player's device
            bulbmaterial.SetColor("_EmissionColor", 2.0f * .3f * power * bulb_color);

            //Sets color of light beam mesh
            beam_material.color = 1.0f * .3f * power * bulb_color;

            //Sets color and intensity of beam's spotlight 
            beam_light.color = bulb_color;
            beam_light.intensity = 180.0f * .3f * power;

            //Testing if flashlight hits reflective surfaces...
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

                //For giving light back...
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

        //Handles light drain
        if (Input.GetMouseButton(0)){

            on = false;
            SetLights(on);

            if (Physics.Raycast(transform.position, transform.forward, out hit, 40, 1 << 12)){

                objhit = hit.transform;

                if (current_light != objhit.gameObject){
                    current_light = objhit.gameObject;
                    lightcasematerial = objhit.Find("Sphere").gameObject.GetComponent<Renderer>().material;

                    caselight = objhit.Find("Point Light").gameObject.GetComponent<Light>();
                    case_color = caselight.color;
                }

                if (caselight.intensity > 0.0f){

                    drain_amt = 2.0f * Time.deltaTime;

                    //set the color  of the point light
                    caselight.intensity = Mathf.Clamp(caselight.intensity - 100.0f * drain_amt, 0.0f, 300.0f);

                    //set the color of the emissive sphere
                    lightcasematerial.SetColor("_EmissionColor", .01f * caselight.intensity * case_color);

                    //set color of bulb on flashlight
                    bulb_color += .25f * (-1.0f * drain_amt * bulb_color + drain_amt * caselight.color);
                    bulbmaterial.SetColor("_EmissionColor", 2.0f * bulb_color);

                    power += .5f * drain_amt;
                }
            }
        }
        else current_light = null;
    }


    private bool EnemyStun(){

        if (Physics.Raycast(transform.position, transform.forward, out hit, 30, 1 << 10)){ //Heads layer
            GameObject head = hit.collider.gameObject;
            head.GetComponent<HeadRef>().slist.GetComponent<EnemyScript>().processCommand(Vector3.zero, EvilState.Stunned);
            return true;
        }
        return false;
    }

    private void SetLights(bool status){

        beam.SetActive(status);
        beam_light.intensity = status ? 1.0f : 0.0f;
        beam_light.enabled = status;

        bounce.SetActive(status);
        bounce_light.enabled = status;

        current_light = null;
    }
}
