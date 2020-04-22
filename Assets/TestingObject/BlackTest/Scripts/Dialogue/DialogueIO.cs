using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class DialogueIO : MonoBehaviour{

    string ReadPath_CSV;
    int RowCount = 0;

    public List<string> ReadNameList = new List<string>();
    public List<string> ReadEmotionList = new List<string>();
    public List<string> ReadSideList = new List<string>();
    public List<string> ReadDialogueList = new List<string>();

    public DialogueManager _dialoguemanager;

    void Awake(){
        ReadPath_CSV = Application.streamingAssetsPath + "/1_Dialogue.csv";//之後用switch case選擇章節載入
        ReadCSVFile(ReadPath_CSV);
        SetChapterDialogue();
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
                ReadNameList.Add(data_values[0].ToString());
                ReadEmotionList.Add(data_values[1].ToString());
                ReadSideList.Add(data_values[2].ToString());
                ReadDialogueList.Add(data_values[3].ToString());
            }
            RowCount++;
        }
    }

    public void SetChapterDialogue(){
        _dialoguemanager.EnqueueDialogue(ReadNameList, ReadEmotionList, ReadSideList, ReadDialogueList);
    }
}
