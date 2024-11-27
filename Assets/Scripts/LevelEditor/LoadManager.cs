using UnityEngine;
using System.IO;
using System.Collections.Generic;

[System.Serializable]
public class NoteClass
{
    public float beat;
    public float ms;
    public int position;
    public string type;
    public GameObject noteObject;
}

public class LoadManager : MonoBehaviour
{
    public List<NoteClass> notes; // 불러온 노트 데이터를 저장할 리스트

    private void Start()
    {
        LoadFromJson(Path.Combine(Application.streamingAssetsPath, "TestLevel.json"));
    }

    public void LoadFromJson(string filePath)
    {
        // 파일이 존재하는지 확인
        if (File.Exists(filePath))
        {
            // JSON 파일의 내용을 문자열로 읽기
            string json = File.ReadAllText(filePath);

            // JSON 문자열을 NoteClass 리스트로 변환
            NotesContainer container = JsonUtility.FromJson<NotesContainer>(json);
            notes = container.notes;

            Debug.Log("Chart loaded successfully!");
        }
        else
        {
            Debug.LogError("File not found at: " + filePath);
        }
    }

    [System.Serializable]
    private class NotesContainer
    {
        public List<NoteClass> notes; // JSON의 "notes" 필드와 일치해야 함
    }
}
