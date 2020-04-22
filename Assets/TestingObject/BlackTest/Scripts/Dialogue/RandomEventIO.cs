using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class EventInfo{
    public string EventType;
    public string ActorName;
    public string Emotions;
    public string Sentences;
}

public class RandomEventIO : MonoBehaviour{

    Dictionary<string, EventInfo> RandomDic = new Dictionary<string, EventInfo>();
    string ReadPath_CSV;
    int RowCount = 0;
    public EventManager _eventmanager;

    void Awake(){
        ReadPath_CSV = Application.streamingAssetsPath + "/1_RandomEvent.csv";//之後用switch case選擇章節載入
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

                //分欄位存入Dictionary
                if (data_values[0] != ""){
                    EventInfo _eventinfo = new EventInfo();
                    _eventinfo.EventType = data_values[0];
                    _eventinfo.ActorName = data_values[1];
                    _eventinfo.Emotions = data_values[2];
                    _eventinfo.Sentences = data_values[3];

                    RandomDic.Add(_eventinfo.EventType, _eventinfo);
                }
            }
            RowCount++;
        }
    }

    public void TriggerEventType(string Type) {
        EventInfo Current = RandomDic[Type];
        _eventmanager.PushEventToQueue(Current.ActorName, Current.Emotions, Current.Sentences);
    }



}
