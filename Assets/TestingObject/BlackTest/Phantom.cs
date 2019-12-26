using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.VFX;

public class Phantom : MonoBehaviour{

    float _Rate = 40.0f;
    float Emission_Moment;
    float AliveTime = 0.3f;
    float DisappearTime = 0.6f;
    bool Emission_State = false;
    public bool CheckOnce = false;

    void Update(){

        if (CheckOnce == true) {
            CheckOnce = false;
            GetComponent<VisualEffect>().SetInt("Number_of_Particles", 3000);
            GetComponent<VisualEffect>().playRate = _Rate;
            Emission_Moment = Time.time;
            Emission_State = true;
        }

        GetComponent<VisualEffect>().playRate = _Rate;

        if (Emission_State == true) {
            if (Time.time > Emission_Moment + AliveTime){
                Debug.Log("line21");
                GetComponent<VisualEffect>().SetInt("Number_of_Particles", 0);
            }

            if (Time.time > Emission_Moment + DisappearTime){
                Debug.Log("line24");
                Destroy(gameObject);
            }
        }
    }
}
