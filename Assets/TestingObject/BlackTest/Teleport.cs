using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.VFX;

public class Teleport : MonoBehaviour{

    public Material[] Karol_Dissolve = new Material[2];
    float CurrentValue = 1.0f;
    bool OnDissolve = false;
    public float DissolveSpeed = 0.04f;

    public VisualEffect VFX_Teleport;

    public float _Rate = 3.5f;
    float _Drag = 2.0f;

    void Update(){

        if (Input.GetKeyDown(KeyCode.Q) && OnDissolve == false){
            CurrentValue = 1.0f;
            OnDissolve = true;
        }

        else if (Input.GetKeyDown(KeyCode.W) && OnDissolve == false)
        {
            CurrentValue = 1.0f;
            OnDissolve = true;
            transform.position = transform.position + new Vector3(0.0f, 0.0f, -1.5f);
            VFX_Teleport.playRate = _Rate;
            VFX_Teleport.SetInt("Number_of_Particles", 1000000);
            VFX_Teleport.SetFloat("AttractDrag", 0.0f);
            transform.eulerAngles = new Vector3(0.0f, 180.0f, 0.0f);
            GetComponent<Animator>().Play("DashForward_Mod");
        }

        else if (Input.GetKeyDown(KeyCode.A) && OnDissolve == false)
        {
            CurrentValue = 1.0f;
            OnDissolve = true;
            transform.position = transform.position + new Vector3(1.5f, 0.0f, 0.0f);
            VFX_Teleport.playRate = _Rate;
            VFX_Teleport.SetInt("Number_of_Particles", 1000000);
            VFX_Teleport.SetFloat("AttractDrag", 0.0f);
            transform.eulerAngles = new Vector3(0.0f, 90.0f, 0.0f);
            GetComponent<Animator>().Play("DashSide");
        }

        else if (Input.GetKeyDown(KeyCode.S) && OnDissolve == false)
        {
            CurrentValue = 1.0f;
            OnDissolve = true;
            transform.position = transform.position + new Vector3(0.0f, 0.0f, 1.5f);
            VFX_Teleport.playRate = _Rate;
            VFX_Teleport.SetInt("Number_of_Particles", 1000000);
            VFX_Teleport.SetFloat("AttractDrag", 0.0f);
            transform.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
            GetComponent<Animator>().Play("DashBackward");
        }

        else if (Input.GetKeyDown(KeyCode.D) && OnDissolve == false)
        {
            CurrentValue = 1.0f;
            OnDissolve = true;
            transform.position = transform.position + new Vector3(-1.5f, 0.0f, 0.0f);
            VFX_Teleport.playRate = _Rate;
            VFX_Teleport.SetInt("Number_of_Particles", 1000000);
            VFX_Teleport.SetFloat("AttractDrag", 0.0f);
            transform.eulerAngles = new Vector3(0.0f, 270.0f, 0.0f);
            GetComponent<Animator>().Play("DashSide");
        }

        if (CurrentValue <= 0.0f) {
            OnDissolve = false;
            CurrentValue = 0.0f;
            VFX_Teleport.SetInt("Number_of_Particles", 0);
            VFX_Teleport.SetFloat("AttractDrag", _Drag);
        }

        else{
            CurrentValue = CurrentValue - DissolveSpeed;
            Karol_Dissolve[0].SetFloat("Vector1_F4D760A8", CurrentValue);
            Karol_Dissolve[1].SetFloat("Vector1_F4D760A8", CurrentValue);
        }

    }
}
