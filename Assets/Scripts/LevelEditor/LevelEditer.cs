using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class LevelEditer : MonoBehaviour
{

    private SaveManager saveManager;

    [System.Obsolete]
    private void Start()
    {
        saveManager = FindObjectOfType<SaveManager>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            float beat = GetCurrentMsFromTimeline();
            int position = GetNotePositionFromMouse();

            NoteClass newNote = new NoteClass { beat = beat, position = position, type = "normal" };
            saveManager.notes.Add(newNote);
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            string userPath = "/Users/roena0516/Desktop/Game/LevelEditerTest.json";
            saveManager.SaveToJson(userPath);
        }
    }

    float GetCurrentMsFromTimeline()
    {
        return Time.realtimeSinceStartup * 1000f;
    }

    int GetNotePositionFromMouse()
    {
        // 마우스 위치를 기준으로 노트 위치 결정
        return Mathf.FloorToInt(Input.mousePosition.x / 100);
    }

    //public void LoadFromJson()
    //{
    //    string path = Application.persistentDataPath + "/chartData.json";
    //    if (File.Exists(path))
    //    {
    //        string json = File.ReadAllText(path);
    //        NoteData loadedData = JsonUtility.FromJson<NoteData>(json);
    //        notes = new List<Note>(loadedData.notes);
    //    }
    //}

}
