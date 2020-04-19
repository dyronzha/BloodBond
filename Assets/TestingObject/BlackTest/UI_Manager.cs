using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Manager : MonoBehaviour{
    public Image[] Menu;
    public Sprite[] BtnState;
    bool AlreadyPush = false;
    bool BtnWorked = false;
    int Currentbtn = 0;
    public Animator BlackPanel;

    bool SelectNewGame = false;
    float Trigger_Moment;

    void Start(){
        BtnSelect(Currentbtn);
    }

    void Update(){
        if (SelectNewGame == true && Time.time > Trigger_Moment + 3.0f) {
            Debug.Log("SceneTrasction");
            SelectNewGame = false;
        }

        if (AlreadyPush == false && BtnWorked == false){
            if (Input.GetAxis("Vertical") > 0.0f || Input.GetAxis("Joy1Axis7") > 0.0f){
                if (Currentbtn > 0) Currentbtn--;
                AlreadyPush = true;
                BtnSelect(Currentbtn);
            }
            else if (Input.GetAxis("Vertical") < 0.0f || Input.GetAxis("Joy1Axis7") < 0.0f){
                if (Currentbtn < 3) Currentbtn++;
                AlreadyPush = true;
                BtnSelect(Currentbtn);
            }

            else if (Input.GetButtonDown("Fire1")) {
                BtnWorked = true;
                BtnFunction(Currentbtn);
            }

        }
        else if (AlreadyPush == true && Input.GetAxis("Vertical") == 0.0f && Input.GetAxis("Joy1Axis7") == 0.0f) {AlreadyPush = false;}
    }

    void BtnSelect(int _btn) {
        for (int i = 0; i < 4; i++) {
            if (i == _btn) Menu[i].sprite = BtnState[i + 4];
            else Menu[i].sprite = BtnState[i];
        }
    }

    void BtnFunction(int _btn) {
        Menu[_btn].sprite = BtnState[_btn+8];
        switch (_btn) {
            case 0:
                Trigger_Moment = Time.time;
                BlackPanel.Play("FadeIn");
                SelectNewGame = true;
                Debug.Log("New Game");
                break;
            case 1:
                Debug.Log("Continue");
                break;
            case 2:
                Debug.Log("Gallery");
                break;
            case 3:
                Debug.Log("Exit");
                break;
        }
    }
}
