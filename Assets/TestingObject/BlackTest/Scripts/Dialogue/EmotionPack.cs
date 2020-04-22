using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class EmotionPack : MonoBehaviour{

    //立繪對話---Start
    string url;
    Sprite CurrentSprite;
    Dictionary<string, Sprite> EmotionDic = new Dictionary<string, Sprite>();

    public Animator anim_L;
    public Animator anim_R;
    //public Animator anim_R2;
    public Image Character_L;
    public Image Character_R;
    //public Image Character_R2;
    //public Canvas Sort_L;
    //public Canvas Sort_R;
    //public Canvas Sort_R2;

    //當角色一多，改用bool陣列+foreach去控管是否上灰階
    bool L_AlreadyIn = false;
    bool R_AlreadyIn = false;
    //bool R2_AlreadyIn = false;
    //立繪對話---End

    //事件對話---Start
    string event_url;
    Sprite CurrentEventSprite;
    Dictionary<string, Sprite> EventDic = new Dictionary<string, Sprite>();
    public Animator anim_Event;
    public Image Character_Event;
    //事件對話---End


    void Start(){
        CreateDictonary(1);
        CreateEventDictionary(1);
    }

    //立繪對話相關函式---Start
    public void FindEmotion(string EmotionName, string Side) {
        if (EmotionDic.ContainsKey(EmotionName) == true) {
            if (Side == "L"){
                Character_L.sprite = EmotionDic[EmotionName];
                Character_L.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                if (L_AlreadyIn == false){
                    anim_L.Play("Actor_L_In");
                    anim_L.SetBool("ComeIn", true);
                    L_AlreadyIn = true;
                }
                if (R_AlreadyIn == true) Character_R.color = new Color(0.5f, 0.5f, 0.5f, 1.0f);
                //if(R2_AlreadyIn == true) Character_R2.color = new Color(0.5f, 0.5f, 0.5f, 1.0f);
            }

            else if (Side == "R"){
                Character_R.sprite = EmotionDic[EmotionName];
                Character_R.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                if (R_AlreadyIn == false){
                    anim_R.Play("Actor_R_In");
                    anim_R.SetBool("ComeIn", true);
                    R_AlreadyIn = true;
                }
                if (L_AlreadyIn == true) Character_L.color = new Color(0.5f, 0.5f, 0.5f, 1.0f);
                //if (R2_AlreadyIn == true) {
                //    Character_R2.color = new Color(0.5f, 0.5f, 0.5f, 1.0f);
                //    Sort_R.sortingOrder = 1;
                //    Sort_R2.sortingOrder = 0;
                //}
            }

            else if (Side == "R2") {
                //Character_R2.sprite = EmotionDic[EmotionName];
                //Character_R2.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                //if (R2_AlreadyIn == false){
                //    anim_R2.SetBool("ComeIn", true);
                //    R2_AlreadyIn = true;
                //}
                if (L_AlreadyIn == true) Character_L.color = new Color(0.5f, 0.5f, 0.5f, 1.0f);
                if (R_AlreadyIn == true) {
                    Character_R.color = new Color(0.5f, 0.5f, 0.5f, 1.0f);
                    //Sort_R.sortingOrder = 0;
                    //Sort_R2.sortingOrder = 1;
                }
            }

        }


    }

    public void CharacterLeave(string Side){
        switch (Side){
            case "L":
                anim_L.Play("Actor_L_Out");
                anim_L.SetBool("ComeIn", false);
                L_AlreadyIn = false;
                break;

            case "R":
                anim_R.Play("Actor_R_Out");
                anim_R.SetBool("ComeIn", false);
                R_AlreadyIn = false;
                break;

            //case "R2":
            //    anim_R2.SetBool("ComeIn", false);
            //    R2_AlreadyIn = false;
            //    break;

            case "All":
                anim_L.Play("Actor_L_Out");
                anim_L.SetBool("ComeIn", false);
                L_AlreadyIn = false;
                anim_R.Play("Actor_R_Out");
                anim_R.SetBool("ComeIn", false);
                R_AlreadyIn = false;
                //anim_R2.SetBool("ComeIn", false);
                //R2_AlreadyIn = false;
                break;
        }
    }

    void GetFolder(string FolderName) {
        DirectoryInfo dir = new DirectoryInfo(Application.streamingAssetsPath + "/" + FolderName);
        FileInfo[] info = dir.GetFiles("*.png");
        foreach (FileInfo f in info) {
            string FileName = f.Name.Remove(f.Name.Length-4);
            EmotionDic.Add(FileName, LoadSprite(FolderName, FileName));
        }
    }

    Sprite LoadSprite(string FolderName, string FileName){
        url = Path.Combine(Application.streamingAssetsPath, FolderName + "/" + FileName + ".png");
        byte[] imgData;
        Texture2D tex = new Texture2D(2, 2);

        imgData = File.ReadAllBytes(url);
        tex.LoadImage(imgData);

        Vector2 pivot = new Vector2(0.5f, 0.5f);
        CurrentSprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), pivot, 100.0f);
        CurrentSprite.name = FileName;
        return CurrentSprite;
    }

    public void CreateDictonary(int Chapter){
        switch (Chapter){
            case 1:
                GetFolder("Karol");
                GetFolder("Iterra");
                GetFolder("Blood");
                GetFolder("Earth");
                break;
        }
    }
    //立繪對話相關函式---End

    //事件對話相關函式---Start
    public void FindEventEmotion(string EmotionName) {
        Character_Event.sprite = EventDic[EmotionName];
        anim_Event.SetBool("IsOpen", true);
        anim_Event.Play("EventComeIn");
    }

    public void EventLeave() {
        anim_Event.SetBool("IsOpen", false);
        anim_Event.Play("EventComeOut");
    }

    void GetEventFolder(string FolderName) {
        DirectoryInfo dir = new DirectoryInfo(Application.streamingAssetsPath + "/Events/" + FolderName);
        FileInfo[] info = dir.GetFiles("*.png");
        foreach (FileInfo f in info) {
            string FileName = f.Name.Remove(f.Name.Length - 4);
            EventDic.Add(FileName, LoadEventSprite(FolderName, FileName));
        }
    }

    Sprite LoadEventSprite(string FolderName, string FileName) {
        event_url = Application.streamingAssetsPath + "/Events/" + FolderName + "/" + FileName + ".png";
        byte[] imgData;
        Texture2D tex = new Texture2D(2, 2);

        imgData = File.ReadAllBytes(event_url);
        tex.LoadImage(imgData);

        Vector2 pivot = new Vector2(0.5f, 0.5f);
        CurrentEventSprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), pivot, 100.0f);
        CurrentEventSprite.name = FileName;
        return CurrentEventSprite;
    }

    public void CreateEventDictionary(int Chapter) {
        switch (Chapter) {
            case 1:
                GetEventFolder("Karol");
                GetEventFolder("Iterra");
                GetEventFolder("Hunter");
                GetEventFolder("Blood");
                break;
        }
    }
    //事件對話相關函式---End

}
