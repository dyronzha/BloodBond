using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectPlay : MonoBehaviour{

    public GameObject[] EffectPool;
    public SkinnedMeshRenderer[] _KarolSkin;
    public GameObject Combo2_Phantom;

    public void PlayWhichEffect(int num){
        if (EffectPool == null || EffectPool.Length <= num) Debug.Log("Wrong Number");
        EffectPool[num].SetActive(true);
        foreach (Transform Child in EffectPool[num].transform) {
            if (Child.GetComponent<ParticleSystem>() == null) Debug.Log("A child without Particle");
            else Child.GetComponent<ParticleSystem>().Play();
        }
    }

    public void Phantom_ForCombo2() {
        Instantiate(Combo2_Phantom, transform.position, transform.rotation);
    }


    public void Disappear_ForCombo3(int i){
        if (i == 0) { foreach (SkinnedMeshRenderer _skin in _KarolSkin) _skin.enabled = false; }
        else { foreach (SkinnedMeshRenderer _skin in _KarolSkin) _skin.enabled = true; }
    }

}
