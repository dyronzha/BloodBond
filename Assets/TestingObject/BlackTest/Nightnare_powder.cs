using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.VFX;

public class Nightnare_powder : MonoBehaviour{

    float Trigger_Moment = 0.0f;
    bool Powder = false;
    SkinnedMeshRenderer _skin;
    VisualEffect VFX_Powder;

    void Start(){
        _skin = transform.GetChild(0).GetComponent<SkinnedMeshRenderer>();
        VFX_Powder = transform.GetChild(1).GetComponent<VisualEffect>();
        VFX_Powder.Stop();
    }


    void Update(){
        if (Powder == true && Time.time > Trigger_Moment + 2.0f) {
            Powder = false;
            _skin.enabled = false;
            VFX_Powder.Play();
        }
    }

    public void Become_Powder() {
        Trigger_Moment = Time.time;
        Powder = true;
    }

}
