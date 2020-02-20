using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportShadow : MonoBehaviour{

    float Speed = 30.0f;

    void Start(){
        
    }

    void Update(){
        Speed = Mathf.Lerp(Speed, 0.0f, 0.5f);
        ShadowDisappear(Speed);

        if (Speed < 0.005f) Destroy(gameObject.transform.parent.gameObject);
    }

    void ShadowDisappear(float x) {
        SkinnedMeshRenderer[] skinMeshList = GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (SkinnedMeshRenderer smr in skinMeshList) {
            smr.material.SetVector("_FresnelAmount", new Vector4(x, x, x, x));
        }

    }

}
