using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KarolShader : MonoBehaviour{

    public Material[] ShaderType;
    public SkinnedMeshRenderer[] MeshType_1;
    public SkinnedMeshRenderer[] MeshType_2;
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
            ShaderType[2].SetFloat("Vector1_F4D760A8", DissolveValue);
            ShaderType[3].SetFloat("Vector1_F4D760A8", DissolveValue);

            if (DissolveValue <= 0.0f) {
                GetComponent<MoveTest>().DissolveEnd();
                ShaderMode = 0;
                ChangeMaterial(ShaderMode);
            }
        }

        else if (ShaderMode == 2) {
            DodgeGlowValue = DodgeGlowValue - DodgeGlowSpeed;
            ShaderType[4].SetVector("Vector4_83C1BE40", new Vector4(DodgeGlowValue, DodgeGlowValue, DodgeGlowValue, DodgeGlowValue));
            ShaderType[5].SetVector("Vector4_83C1BE40", new Vector4(DodgeGlowValue, DodgeGlowValue, DodgeGlowValue, DodgeGlowValue));
            if (DodgeGlowValue <= 0.0f){
                ShaderMode = 0;
                ChangeMaterial(ShaderMode);
            }
        }
    }

    public void ChangeMaterial(int Type) {
        if (Type == 2) {
            DissolveValue = 1.0f;
            ShaderMode = 1;
        }

        if (Type == 4) {
            DodgeGlowValue = 1.0f;
            ShaderMode = 2;
        }

        foreach (SkinnedMeshRenderer _skin in MeshType_1) {_skin.material = ShaderType[Type];}
        foreach (SkinnedMeshRenderer _skin in MeshType_2) { _skin.material = ShaderType[Type+1]; }
    }


}
