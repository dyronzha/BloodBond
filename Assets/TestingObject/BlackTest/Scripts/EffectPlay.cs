using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectPlay : MonoBehaviour{

    public GameObject[] EffectPool;
    ParticleSystem[,] _PsPool;
    SkinnedMeshRenderer[] _KarolSkin;
    public GameObject Combo2_Phantom;
    public GameObject shawl;
    int MaxCount = 0;

    void Start(){
        _KarolSkin = GetComponentsInChildren<SkinnedMeshRenderer>();

        for (int i = 0; i < EffectPool.Length; i++) {if (MaxCount < EffectPool[i].transform.childCount) MaxCount = EffectPool[i].transform.childCount;}
        _PsPool = new ParticleSystem[EffectPool.Length, MaxCount];

        for (int i = 0; i < EffectPool.Length; i++){
            int j = 0;
            foreach (Transform _ps in EffectPool[i].transform){
                if (_ps.GetComponent<ParticleSystem>() != null) {
                    _PsPool[i, j] = _ps.GetComponent<ParticleSystem>();
                    j++;
                }
            }
        }

    }

    public void PlayWhichEffect(int num){
        if (EffectPool == null || EffectPool.Length <= num) Debug.Log("Wrong Number");
        EffectPool[num].SetActive(true);

        for (int i = 0; i < MaxCount; i++) {
            if (_PsPool[num, i] != null) _PsPool[num, i].Play();
        }
    }

    public void Phantom_ForCombo2() {
        Instantiate(Combo2_Phantom, transform.position, transform.rotation);
    }


    public void Disappear_ForCombo3(int i){
        if (i == 0) { 
            foreach (SkinnedMeshRenderer _skin in _KarolSkin) _skin.enabled = false;
            if (shawl != null) shawl.SetActive(false);
        }
        else { 
            foreach (SkinnedMeshRenderer _skin in _KarolSkin) _skin.enabled = true;
            if (shawl != null) shawl.SetActive(true);
        }
    }

}
