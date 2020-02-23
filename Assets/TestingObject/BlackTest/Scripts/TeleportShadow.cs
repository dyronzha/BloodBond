using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportShadow : MonoBehaviour{

    float Speed = 30.0f;
    SkinnedMeshRenderer[] skinMeshList;
    MeshRenderer Combo2_Mesh;
    int RenderMode = 0; //1 =  SkinnedMeshRenderer，2 = MeshRenderer

    void Start(){
        if (GetComponent<MeshRenderer>() == null){
            skinMeshList = GetComponentsInChildren<SkinnedMeshRenderer>();
            RenderMode = 1;
        }
        else{
            Combo2_Mesh = GetComponent<MeshRenderer>();
            RenderMode = 2;
        }

    }

    void Update(){
        ShadowDisappear(Speed);
        if (Speed < 0.005f) Destroy(gameObject.transform.parent.gameObject);
    }

    void ShadowDisappear(float x) {
        Speed = Mathf.Lerp(Speed, 0.0f, 0.5f);
        if (RenderMode == 1){
            foreach (SkinnedMeshRenderer smr in skinMeshList) smr.material.SetVector("_FresnelAmount", new Vector4(x, x, x, x));
        }

        else if (RenderMode == 2) {
            foreach (Material mat in Combo2_Mesh.materials)mat.SetVector("_FresnelAmount", new Vector4(x, x, x, x));
        }
    }

}
