using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class MainEventIO : MonoBehaviour{

    string ReadPath_CSV;
    int RowCount = 0;
    public Queue<string> AllActorNames = new Queue<string>();
    public Queue<string> AllEmotions = new Queue<string>();
    public Queue<string> AllEventSentences = new Queue<string>();
    public EventManager _eventmanager;

    void Awake(){
        AllActorNames.Clear();
        AllEmotions.Clear();
        AllEventSentences.Clear();
        ReadPath_CSV = Application.streamingAssetsPath + "/1_MainEvent.csv";//之後用switch case選擇章節載入
        ReadCSVFile(ReadPath_CSV);
    }

    void ReadCSVFile(string filePath){
        StreamReader sReader = new StreamReader(filePath);
        bool EndofFile = false;

        while (!EndofFile){
            string line = sReader.ReadLine();
            if (line == null){
                EndofFile = true;
                break;
            }
            if (RowCount != 0){
                var data_values = line.Split(',');
                AllActorNames.Enqueue(data_values[0]);
                AllEmotions.Enqueue(data_values[1]);
                AllEventSentences.Enqueue(data_values[2]);
            }
            RowCount++;
        }
    }

    public void TriggerMainEvent() {
        while (AllActorNames.Count != 0 && AllActorNames.Peek() != "") {
            _eventmanager.PushEventToQueue(AllActorNames.Dequeue(), AllEmotions.Dequeue(), AllEventSentences.Dequeue());
        }
        if (AllActorNames.Count !=0 && AllActorNames.Peek() == "") {
            AllActorNames.Dequeue();
            AllEmotions.Dequeue();
            AllEventSentences.Dequeue();
        }
    }
}