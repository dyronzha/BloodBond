using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.VFX;

public class Phantom : MonoBehaviour{

    float _Rate = 40.0f;
    float Emission_Moment;
    float AliveTime = 1f;
    float DisappearTime = 2f;
    int Emission_State;

    private void Start()
    {
        Emission_State = 0;
    }

    void Update(){
        Debug.Log("Emission_State = " + Emission_State + "in update");
        Debug.Log(Emission_State);
        if (Emission_State == 1 && Time.time > Emission_Moment + AliveTime) {
            Debug.Log("line19");
            GetComponent<VisualEffect>().SetInt("Number_of_Particles", 0);
            Emission_State = 2;
        }

        if (Emission_State == 2 && Time.time > Emission_Moment + DisappearTime) {
            Debug.Log("line25");
            //Emission_State = 0;
            Destroy(gameObject);
        }
    }

    //都有改到，但Update被重置(Rate,EmissionState)
    public void PhantomEmission() {
        GetComponent<VisualEffect>().SetInt("Number_of_Particles", 3000);
        GetComponent<VisualEffect>().playRate = _Rate;
        Emission_Moment = Time.time;
        Emission_State = 1;
        Debug.Log("Emission_State = " + Emission_State + "in function");
    }

}
