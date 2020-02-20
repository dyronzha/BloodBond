using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.VFX;

public class Dodge : MonoBehaviour{

    public float Phantom_Time = 0.3f;
    float Trigger_Moment = 0.0f;

    bool On_Dodging = false;
    Vector3 Dodge_Target;
    Vector3 Dodge_Current;
    public float Dodge_Dis = 0.5f;

    public Material GlowMaterial;
    public GameObject PhantomCreate;
    public int Max_PhantomCount = 5;
    int Current_Max;
    public Vector3[] Phantom_Spacing;
    Vector3 Dis_BetweenPhantom;
    int ArriveCount = 0;

    public VisualEffect[] PhantomManage;
    float[] PhantomMoment;

    void Start(){
        Phantom_Spacing = new Vector3[Max_PhantomCount];
        PhantomManage = new VisualEffect[Max_PhantomCount];
        PhantomMoment = new float[Max_PhantomCount];
        Current_Max = Max_PhantomCount;
    }

    void Update(){

        if (Max_PhantomCount != Current_Max) {
            Phantom_Spacing = new Vector3[Max_PhantomCount];
            PhantomManage = new VisualEffect[Max_PhantomCount];
            PhantomMoment = new float[Max_PhantomCount];
            Current_Max = Max_PhantomCount;
        }

        if (Input.GetKeyDown(KeyCode.E)) {
            GetComponent<Animator>().Play("Dodge");
            GetComponent<Animator>().speed = 0.0f;
            Trigger_Moment = Time.time;
            On_Dodging = true;
            Dodge_Current = transform.position;
            Dodge_Target = transform.position + new Vector3(0.0f, 0.0f, 3.0f);
            PhantomDistance();
        }

        if (On_Dodging == true) {
            transform.position = Vector3.Lerp(transform.position, Dodge_Target, Dodge_Dis);
            //產生殘影
            if (ArriveCount < Max_PhantomCount && Mathf.Abs(transform.position.x - Dodge_Current.x) >= Phantom_Spacing[ArriveCount].x) {
                Instantiate(PhantomCreate,Dodge_Current + new Vector3(0.0f, 1.0f, 0.0f) + Phantom_Spacing[ArriveCount], transform.rotation);
                PhantomCreate.GetComponent<Phantom>().CheckOnce = true;
                ArriveCount++;
            }

            if (Time.time > Trigger_Moment + Phantom_Time) {
                On_Dodging = false;
                GetComponent<Animator>().speed = 1.0f;
                ArriveCount = 0;
            }
        }
    }

    void PhantomDistance() {
        Vector3 Distance = Dodge_Target - Dodge_Current;
        Dis_BetweenPhantom = new Vector3(Distance.x / Max_PhantomCount, Distance.y / Max_PhantomCount, Distance.z / Max_PhantomCount);
        for (int i = 0; i < Max_PhantomCount; i++) {
            Phantom_Spacing[i] = new Vector3(Dis_BetweenPhantom.x * i, Dis_BetweenPhantom.y * i, Dis_BetweenPhantom.z * i);
        }
    }


}
