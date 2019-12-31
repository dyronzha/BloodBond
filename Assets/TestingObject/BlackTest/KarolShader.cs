using System.Collections;
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

    void Update(){

        if (ShaderMode == 1){
            DissolveValue = DissolveValue - DissolveSpeed;
            ShaderType[3].SetFloat("Vector1_F4D760A8", DissolveValue);
            ShaderType[4].SetFloat("Vector1_F4D760A8", DissolveValue);
            ShaderType[5].SetFloat("Vector1_F4D760A8", DissolveValue);

            if (DissolveValue <= 0.0f) {
                GetComponent<MoveTest>().DissolveEnd();
                ShaderMode = 0;
                ChangeMaterial(ShaderMode);
            }
        }

        else if (ShaderMode == 2) {
            DodgeGlowValue = DodgeGlowValue - Time.deltaTime*1.5f;
            ShaderType[6].SetVector("Vector4_83C1BE40", new Vector4(DodgeGlowValue, DodgeGlowValue, DodgeGlowValue, DodgeGlowValue));
            ShaderType[7].SetVector("Vector4_83C1BE40", new Vector4(DodgeGlowValue, DodgeGlowValue, DodgeGlowValue, DodgeGlowValue));
            ShaderType[8].SetVector("Vector4_83C1BE40", new Vector4(DodgeGlowValue, DodgeGlowValue, DodgeGlowValue, DodgeGlowValue));
            if (DodgeGlowValue <= 0.0f){
                ShaderMode = 0;
                ChangeMaterial(ShaderMode);
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
        }

        foreach (SkinnedMeshRenderer _skin in Mesh_Head) {_skin.material = ShaderType[Type];}
        foreach (SkinnedMeshRenderer _skin in Mesh_Body) { _skin.material = ShaderType[Type+1]; }
        foreach (SkinnedMeshRenderer _skin in Mesh_Foot) { _skin.material = ShaderType[Type + 2]; }
    }


}
