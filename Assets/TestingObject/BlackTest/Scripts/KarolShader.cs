﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KarolShader : MonoBehaviour{

    public Material[] ShaderType;
    public SkinnedMeshRenderer[] Mesh_Head;
    public SkinnedMeshRenderer[] Mesh_Body;
    public SkinnedMeshRenderer[] Mesh_Foot;
    int ShaderMode = 0;

    //瞬移
    float DissolveValue = 1.0f;
    public float DissolveSpeed = 0.04f;

    //衝刺閃躲
    float DodgeGlowValue = 1.0f;
    public float DodgeGlowSpeed = 0.004f;

    float Duration = 2.0f;

    //隱蔽
    float Hidden_Moment;
    float HiddenRecoverProcess = 0.0f;
    public float Hidden_Time = 0.0f;

    void Update(){

        if (ShaderMode == 1){
            DissolveValue = DissolveValue - DissolveSpeed;
            ShaderType[3].SetFloat("Vector1_F4D760A8", DissolveValue);
            ShaderType[4].SetFloat("Vector1_F4D760A8", DissolveValue);
            ShaderType[5].SetFloat("Vector1_F4D760A8", DissolveValue);

            if (DissolveValue <= 0.0f){
                GetComponent<MoveTest>().DissolveEnd();
                ShaderMode = 0;
                ChangeMaterial(ShaderMode);
            }
        }

        else if (ShaderMode == 2){
            DodgeGlowValue = DodgeGlowValue - Time.deltaTime * 1.5f;
            ShaderType[6].SetVector("Vector4_83C1BE40", new Vector4(DodgeGlowValue, DodgeGlowValue, DodgeGlowValue, DodgeGlowValue));
            ShaderType[7].SetVector("Vector4_83C1BE40", new Vector4(DodgeGlowValue, DodgeGlowValue, DodgeGlowValue, DodgeGlowValue));
            ShaderType[8].SetVector("Vector4_83C1BE40", new Vector4(DodgeGlowValue, DodgeGlowValue, DodgeGlowValue, DodgeGlowValue));
            if (DodgeGlowValue <= 0.0f){
                ShaderMode = 0;
                ChangeMaterial(ShaderMode);
            }
        }

        else if (ShaderMode == 3) {
            if (Time.time > Hidden_Moment + Hidden_Time) {
                HiddenRecoverProcess = HiddenRecoverProcess + Time.deltaTime * 1.5f;
                ShaderType[15].SetFloat("Vector1_B5CD4EBB", HiddenRecoverProcess);
                ShaderType[16].SetFloat("Vector1_B5CD4EBB", HiddenRecoverProcess);
                ShaderType[17].SetFloat("Vector1_B5CD4EBB", HiddenRecoverProcess);
                if (HiddenRecoverProcess >=1.0f) {
                    HiddenRecoverProcess = 1.0f;
                    ShaderMode = 0;
                    ChangeMaterial(ShaderMode);
                }

            }
        }

    }

    public void ChangeMaterial(int Type) {

        switch (Type) {
            case 3:
                DissolveValue = 1.0f;
                ShaderMode = 1;
                break;
            case 6:
                DodgeGlowValue = 1.0f;
                ShaderMode = 2;
                break;
            case 15:
                HiddenRecoverProcess = 0.0f;
                ShaderType[15].SetFloat("Vector1_B5CD4EBB", HiddenRecoverProcess);
                ShaderType[16].SetFloat("Vector1_B5CD4EBB", HiddenRecoverProcess);
                ShaderType[17].SetFloat("Vector1_B5CD4EBB", HiddenRecoverProcess);
                Hidden_Moment = Time.time;
                ShaderMode = 3;
                break;
        }

        foreach (SkinnedMeshRenderer _skin in Mesh_Head) {_skin.material = ShaderType[Type];}
        foreach (SkinnedMeshRenderer _skin in Mesh_Body) { _skin.material = ShaderType[Type+1]; }
        foreach (SkinnedMeshRenderer _skin in Mesh_Foot) { _skin.material = ShaderType[Type + 2]; }
    }

    public void LerpMaterial(int Before, int After) {
        //float lerp = Mathf.PingPong(Time.time, Duration) / Duration;
        foreach (SkinnedMeshRenderer _skin in Mesh_Head) { _skin.material.Lerp(ShaderType[Before], ShaderType[After], 0.5f); }
        foreach (SkinnedMeshRenderer _skin in Mesh_Head) { _skin.material.Lerp(ShaderType[Before+1], ShaderType[After+1], 0.5f); }
        foreach (SkinnedMeshRenderer _skin in Mesh_Head) { _skin.material.Lerp(ShaderType[Before+2], ShaderType[After+2], 0.5f); }
    }

}
