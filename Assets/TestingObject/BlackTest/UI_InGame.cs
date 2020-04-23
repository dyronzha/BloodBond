using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_InGame : MonoBehaviour{

    //左上HP相關
    bool Modify = false;
    float CurrentHP = 100.0f;
    float AfterATKHP = 0.0f;
    float fillAmount = 100.0f;
    float Lerping = 0.8f;
    public Image HPBar;

    //左下血瓶Potion相關
    int CurrentPotionCount = 0;
    public Text txt_potion;

    void Update(){
        if (Input.GetKeyDown(KeyCode.V)) ReceiveAttack(5);
        if (Input.GetKeyDown(KeyCode.B)) Recovery(15);
        if (Input.GetKeyDown(KeyCode.K)) PotionPlus();
        if (Input.GetKeyDown(KeyCode.L)) PotionDecrease();
        HPBar.fillAmount = Mathf.Lerp(HPBar.fillAmount, fillAmount,Lerping*Time.deltaTime);
    }

    //受到傷害用
    public void ReceiveAttack(int ATK) {
        fillAmount = (CurrentHP - ATK) / 100.0f;
        if (fillAmount <= 0.0f) {
            fillAmount = 0.0f;
            //呼叫死亡相關程式
        }
        Modify = true;
        CurrentHP = CurrentHP - ATK;
    }

    void Recovery(int _rec) {
        fillAmount = (CurrentHP + _rec) / 100.0f;
        if (fillAmount > 1.0f) {
            fillAmount = 1.0f;
        }
        Modify = true;
        CurrentHP = CurrentHP + _rec;
    }

    //左下藥水增減用
    public void PotionPlus(){
        if(CurrentPotionCount<5)CurrentPotionCount++;
        txt_potion.text = CurrentPotionCount.ToString() + " / 5";
    }

    public void PotionDecrease(){
        if (CurrentPotionCount > 0) {
            Recovery(20);
            CurrentPotionCount--;
        }
        txt_potion.text = CurrentPotionCount.ToString() + " / 5";
    }

}
