using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//之後可考慮將角色立繪身處位置也編入CSV表格以便讀取與傳送

public class DialogueManager : MonoBehaviour{
    public TestPlayer _testplayer;
    public EmotionPack _emotionpack;
    public bool DuringDialogue = false;
    public Text nameText;
    public Text DialogueText;
    public Animator animator;
    public Queue<string> AllActorNames = new Queue<string>();
    public Queue<string> AllEmotions = new Queue<string>();
    public Queue<string> AllActorSides = new Queue<string>();
    public Queue<string> AllDialogueSentences = new Queue<string>();
    public float TypingSpeed = 0.05f;
    string CurrentSentence = "";

    //確認內文用
    int CheckChar = 0;
    //public Image IconX;
    bool DuringTyping = false;

    void Update(){
        if (DuringDialogue == true) {
            if (Input.GetMouseButtonDown(0) || Input.GetButtonDown("Fire3")) {
                if (DuringTyping == true)  SkipTypeEffect(); 
                else DisplayNextSentence();
            }
        }
    }

    public void EnqueueDialogue(List<string> NameList, List<string> EmotionList, List<string> SideList, List<string> DialogueList) {
        //預先清空
        AllActorNames.Clear();
        AllEmotions.Clear();
        AllActorSides.Clear();
        AllDialogueSentences.Clear();

        //推入本章節對話角色名與內容
        foreach (string name in NameList) { AllActorNames.Enqueue(name); }
        foreach (string emotion in EmotionList) { AllEmotions.Enqueue(emotion); }
        foreach (string side in SideList) { AllActorSides.Enqueue(side); }
        foreach (string sentence in DialogueList) { AllDialogueSentences.Enqueue(sentence); }
    }

    public void StartDialogue() {
        //_testplayer.SwitchDialogue(true);
        //animator.SetBool("IsOpen", true);
        animator.Play("DialogueComeIn");
        DialogueText.text = "";
    }

    public void DisplayNextSentence() {
        //IconX.enabled = false;
        if (AllDialogueSentences.Count == 0) {
            //IconX.enabled = false;
            DuringDialogue = false;
            //animator.SetBool("IsOpen", false);
            animator.Play("DialogueComeOut");
            BloodBond.Player.canControl = true;
            _emotionpack.CharacterLeave("All");
            return;
        }

        if (AllActorNames.Peek() == "") {
            DuringDialogue = false;
            //animator.SetBool("IsOpen", false);
            animator.Play("DialogueComeOut");
            AllActorNames.Dequeue();
            AllActorSides.Dequeue();
            AllEmotions.Dequeue();
            AllDialogueSentences.Dequeue();
            _emotionpack.CharacterLeave("All");
            BloodBond.Player.canControl = true;
            return;
        }

        CheckChar = 0;
        DuringDialogue = true;
        DuringTyping = true;

        _emotionpack.FindEmotion(AllEmotions.Dequeue(), AllActorSides.Dequeue());
        nameText.text = AllActorNames.Dequeue();
        CurrentSentence = AllDialogueSentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(CurrentSentence));
    }

    public void CheckActorEmotion() {
        if (AllEmotions.Count != 0 && AllEmotions.Peek() == "") {
            _emotionpack.FindEmotion(AllEmotions.Dequeue(), AllActorSides.Dequeue());
        } 
        else {
            //_emotionpack.CharacterLeave("All");
        }

        //IconX.enabled = false;
    }

    IEnumerator TypeSentence(string CurrentSentence) {
        DialogueText.text = "";
        foreach (char letter in CurrentSentence.ToCharArray()) {
            DialogueText.text += letter;
            if (CheckChar == CurrentSentence.ToCharArray().Length-1) {
                DuringTyping = false;
                //IconX.enabled = true; //叫出Continue圖案
            }
            else CheckChar++;
            yield return new WaitForSeconds(TypingSpeed);
        }
    }

    public void SkipTypeEffect() {
        StopAllCoroutines();
        DialogueText.text = CurrentSentence;
        DuringTyping = false;
        //IconX.enabled = true; //叫出Continue圖案
    }

}
