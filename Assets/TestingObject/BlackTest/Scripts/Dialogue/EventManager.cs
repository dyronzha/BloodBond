using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventManager : MonoBehaviour{

    public Text nameText;
    public Text EventText;

    float TriggerMoment = 0.0f;
    bool Showing_Event = false;
    public float StayTime = 5.0f;

    public EmotionPack _emotionpack;
    public Queue<string> AwaitActorName = new Queue<string>();
    public Queue<string> AwaitEmotion = new Queue<string>();
    public Queue<string> AwaitSentence = new Queue<string>();
    public Animator anim_eventbox;

    void Awake(){
        AwaitActorName.Clear();
        AwaitEmotion.Clear();
        AwaitSentence.Clear();
    }

    void Update(){
        if (Showing_Event == true && Time.time > TriggerMoment + StayTime){
            Showing_Event = false;
            _emotionpack.EventLeave();
        }

        else if (Showing_Event == false && AwaitActorName.Count > 0 && anim_eventbox.GetCurrentAnimatorStateInfo(0).IsName("StandBy")) {
            DisplayNextEvent();
        }

    }

    public void DisplayNextEvent() {
        //應該可以拆掉
        if (AwaitActorName.Count == 0) {
            return;
        }

        nameText.text = "";
        EventText.text = "";
        _emotionpack.FindEventEmotion(AwaitEmotion.Dequeue());
        nameText.text = AwaitActorName.Dequeue();
        EventText.text = AwaitSentence.Dequeue();
        Showing_Event = true;
        TriggerMoment = Time.time;
    }

    public void PushEventToQueue(string ActorName,string Emotion,string Sentence) {
        AwaitActorName.Enqueue(ActorName);
        AwaitEmotion.Enqueue(Emotion);
        AwaitSentence.Enqueue(Sentence);
    }

    //            _randomeventio.TriggerEventType(checkpoint.tag.ToString()); // for隨機事件(寶箱or獵人)

}
