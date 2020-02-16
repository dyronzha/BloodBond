using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.VFX;

public class MoveTest : MonoBehaviour{

    //瞬移
    public Material[] Karol_Dissolve = new Material[2];
    float CurrentValue = 1.0f;
    bool OnDissolve = false;
    public float DissolveSpeed = 0.04f;
    public VisualEffect VFX_Teleport;
    public float _Rate = 6.0f;
    public float _Drag = 0.6f;

    //衝刺閃避
    public float Phantom_Time = 0.3f;
    float Trigger_Moment = 0.0f;
    bool On_Dodging = false;
    Vector3 Dodge_Target;
    Vector3 Dodge_Current;
    public float Dodge_Dis = 0.5f;
    public GameObject PhantomCreate;
    public int Max_PhantomCount = 5;
    int Current_Max;
    public Vector3[] Phantom_Spacing;
    Vector3 Dis_BetweenPhantom;
    int ArriveCount = 0;
    float[] PhantomMoment;

    //衝刺瞬移(有傷害判定的)
    public GameObject TeleportShadow;
    bool On_Teleport = false;
    float AquaMoment = 0.0f;
    public float Teleport_Time = 0.2f;
    float Teleport_Moment;
    public float Teleport_Dis = 0.8f;
    bool On_Aqua = false;
    public ParticleSystem Ps_Trail;
    public ParticleSystem Ps_Particle;
    public GameObject Ps_Arrive;

    void Start() {
        Phantom_Spacing = new Vector3[Max_PhantomCount];
        PhantomMoment = new float[Max_PhantomCount];
        Current_Max = Max_PhantomCount;
    }

    void Update(){

        //衝刺瞬移(有傷害判定的)
        if (Input.GetKeyDown(KeyCode.Q) && On_Teleport == false && On_Aqua == false) {
            //Ps_Trail.Play();
            //Ps_Particle.Play();
            Instantiate(TeleportShadow, transform.position, transform.rotation);
            transform.position = transform.position + new Vector3(0.0f, 0.0f, 6.0f);
            On_Teleport = true;
            GetComponent<Animator>().Play("Dodge");
            GetComponent<Animator>().speed = 0.0f;
            Teleport_Moment = Time.time;
            GetComponent<KarolShader>().ChangeMaterial(9);
        }

        //瞬移
        if (Input.GetKeyDown(KeyCode.W) && OnDissolve == false){
            OnDissolve = true;
            transform.position = transform.position + new Vector3(0.0f, 0.0f, -1.5f);
            VFX_Teleport.playRate = _Rate;
            VFX_Teleport.SetInt("Number_of_Particles", 1000000);
            VFX_Teleport.SetFloat("AttractDrag", 0.0f);
            transform.eulerAngles = new Vector3(0.0f, 180.0f, 0.0f);
            GetComponent<KarolShader>().ChangeMaterial(3);
            GetComponent<Animator>().Play("DashForward_Mod");
        }

        else if (Input.GetKeyDown(KeyCode.A) && OnDissolve == false){
            OnDissolve = true;
            transform.position = transform.position + new Vector3(1.5f, 0.0f, 0.0f);
            VFX_Teleport.playRate = _Rate;
            VFX_Teleport.SetInt("Number_of_Particles", 1000000);
            VFX_Teleport.SetFloat("AttractDrag", 0.0f);
            transform.eulerAngles = new Vector3(0.0f, 90.0f, 0.0f);
            GetComponent<KarolShader>().ChangeMaterial(3);
            GetComponent<Animator>().Play("DashSide");
        }

        else if (Input.GetKeyDown(KeyCode.S) && OnDissolve == false){
            OnDissolve = true;
            transform.position = transform.position + new Vector3(0.0f, 0.0f, 1.5f);
            VFX_Teleport.playRate = _Rate;
            VFX_Teleport.SetInt("Number_of_Particles", 1000000);
            VFX_Teleport.SetFloat("AttractDrag", 0.0f);
            transform.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
            GetComponent<KarolShader>().ChangeMaterial(3);
            GetComponent<Animator>().Play("DashBackward");
        }

        else if (Input.GetKeyDown(KeyCode.D) && OnDissolve == false){
            OnDissolve = true;
            transform.position = transform.position + new Vector3(-1.5f, 0.0f, 0.0f);
            VFX_Teleport.playRate = _Rate;
            VFX_Teleport.SetInt("Number_of_Particles", 1000000);
            VFX_Teleport.SetFloat("AttractDrag", 0.0f);
            transform.eulerAngles = new Vector3(0.0f, 270.0f, 0.0f);
            GetComponent<KarolShader>().ChangeMaterial(3);
            GetComponent<Animator>().Play("DashSide");
        }

        //衝刺閃躲
        if (Max_PhantomCount != Current_Max){
            Phantom_Spacing = new Vector3[Max_PhantomCount];
            PhantomMoment = new float[Max_PhantomCount];
            Current_Max = Max_PhantomCount;
        }

        if (Input.GetKeyDown(KeyCode.E)){
            GetComponent<Animator>().Play("Dodge");
            GetComponent<Animator>().speed = 0.0f;
            Trigger_Moment = Time.time;
            On_Dodging = true;
            Dodge_Current = transform.position;
            Dodge_Target = transform.position + new Vector3(0.0f, 0.0f, 3.0f);
            PhantomDistance();
            GetComponent<KarolShader>().ChangeMaterial(6);
        }

        if (On_Dodging == true){
            transform.position = Vector3.Lerp(transform.position, Dodge_Target, Dodge_Dis);
            //產生殘影
            if (ArriveCount < Max_PhantomCount && Mathf.Abs(transform.position.x - Dodge_Current.x) >= Phantom_Spacing[ArriveCount].x){
                Instantiate(PhantomCreate, Dodge_Current + new Vector3(0.0f, 1.0f, 0.0f) + Phantom_Spacing[ArriveCount], transform.rotation);
                PhantomCreate.GetComponent<Phantom>().CheckOnce = true;
                ArriveCount++;
            }

            if (Time.time > Trigger_Moment + Phantom_Time){
                On_Dodging = false;
                //GetComponent<KarolShader>().ChangeMaterial(0);
                GetComponent<Animator>().speed = 1.0f;
                ArriveCount = 0;
            }
        }

        if (On_Teleport == true) {
            //transform.position = Vector3.Lerp(transform.position,Teleport_Target, Teleport_Dis);
            //GetComponent<KarolShader>().LerpMaterial(12, 9);
            if (Time.time > Teleport_Moment + Teleport_Time && On_Aqua == false) {
                On_Aqua = true;
                AquaMoment = Time.time;
                GetComponent<Animator>().speed = 1.0f;
                Instantiate(Ps_Arrive, transform.position + new Vector3(0.0f,1.1f,-0.2f), Quaternion.identity);
                StartCoroutine(StopTrail());
            }

            if (Time.time > AquaMoment + 2.0f && On_Aqua == true) {
                On_Aqua = false;
                On_Teleport = false;
                GetComponent<KarolShader>().ChangeMaterial(0);
            }

        }
    }

    public void DissolveEnd() {
        OnDissolve = false;
        VFX_Teleport.SetInt("Number_of_Particles", 0);
        VFX_Teleport.SetFloat("AttractDrag", _Drag);
    }

    void PhantomDistance(){
        Vector3 Distance = Dodge_Target - Dodge_Current;
        Dis_BetweenPhantom = new Vector3(Distance.x / Max_PhantomCount, Distance.y / Max_PhantomCount, Distance.z / Max_PhantomCount);
        for (int i = 0; i < Max_PhantomCount; i++){
            Phantom_Spacing[i] = new Vector3(Dis_BetweenPhantom.x * i, Dis_BetweenPhantom.y * i, Dis_BetweenPhantom.z * i);
        }
    }

    IEnumerator StopTrail() {
        yield return new WaitForSeconds(0.2f);
        Ps_Trail.Stop();
        Ps_Trail.Stop();
    }
}
