using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.VFX;

public class Dodge : MonoBehaviour{

    public VisualEffect VFX_Dodge;
    float _Rate = 40.0f;
    public float Phantom_Time = 0.3f;
    float Trigger_Moment = 0.0f;
    bool PhantomAlive = false;



    void Update(){
        if (Input.GetKeyDown(KeyCode.E)) {
            //VFX_Dodge.SetInt("Number_of_Particles", 0);
            //VFX_Dodge.Stop();

            VFX_Dodge.SetInt("Number_of_Particles", 5000);
            VFX_Dodge.playRate = _Rate;
            GetComponent<Animator>().Play("Dodge_Backward");
            Trigger_Moment = Time.time;
            PhantomAlive = true;
        }

        if (PhantomAlive == true && Time.time > Trigger_Moment + Phantom_Time) {
            PhantomAlive = false;
            VFX_Dodge.SetInt("Number_of_Particles", 0);
        }

    }
}
