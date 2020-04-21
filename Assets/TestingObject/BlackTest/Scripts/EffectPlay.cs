using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectPlay : MonoBehaviour{

    public GameObject[] EffectPool;
    ParticleSystem[,] _PsPool;
    SkinnedMeshRenderer[] _KarolSkin;
    public GameObject Teleport_Phantom;
    public GameObject Combo2_Phantom;
    public GameObject FlyingDust;
    public GameObject shawl;
    int MaxCount = 0;
    ParticleSystem.ShapeModule a;

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

    public void Phantom_ForTeleport() {
        Instantiate(Teleport_Phantom, transform.position, transform.rotation);
        Instantiate(FlyingDust, transform.position, transform.rotation);
    }
    public void Phantom_ForTeleport(Vector3 dir,float _length,Vector3 _midPos)
    {
        Instantiate(Teleport_Phantom, transform.position, Quaternion.LookRotation(dir));
        GameObject fly = Instantiate(FlyingDust, transform.position, Quaternion.LookRotation(dir));

        for (int i = 0; i < 2; i++) {
            fly.transform.GetChild(i).GetComponent<ParticleSystem>().Stop();
            a = fly.transform.GetChild(i).GetComponent<ParticleSystem>().shape;
            a.length = _length;
            fly.transform.GetChild(i).GetComponent<ParticleSystem>().Play();
        }

        for (int i = 2; i < 4; i++) {
            fly.transform.GetChild(i).GetComponent<ParticleSystem>().Stop();
            a = fly.transform.GetChild(i).GetComponent<ParticleSystem>().shape;
            a.scale = new Vector3(0.5f, _length*0.66f,0.5f);
            fly.transform.GetChild(i).position = _midPos;
            fly.transform.GetChild(i).GetComponent<ParticleSystem>().Play();
        }

        fly.GetComponent<ParticleSystem>().Stop();
        a = fly.GetComponent<ParticleSystem>().shape;
        a.length = _length;
        fly.GetComponent<ParticleSystem>().Play();
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

    public void StopEffect(int _num) {

    }

}
