using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class NoteDataWrapper
{
    public List<NoteClass> notes; // 노트 데이터를 감싸는 클래스
    public SongInfoClass info;
}

public class SaveManager : MonoBehaviour
{
    public List<NoteClass> notes;

    public SongInfoClass info;

    public void SaveToJson(string filePath, float BPM, string artist, string title, string eventName)
    {
        notes.Sort((note1, note2) => note1.beat.CompareTo(note2.beat));

        // NoteDataWrapper의 인스턴스를 생성하고 데이터 할당
        NoteDataWrapper wrapper = new NoteDataWrapper();
        wrapper.notes = notes;

        info.artist = artist;
        info.bpm = BPM;
        info.songFile = "asdf";
        info.title = title;
        info.eventName = eventName;
        wrapper.info = info;

        // JSON 문자열로 변환
        string json = JsonUtility.ToJson(wrapper, true); // prettyPrint를 true로 설정

        string encrypted = EncryptionHelper.Encrypt(json);

        // 파일로 저장
        File.WriteAllText(filePath, encrypted);
        Debug.Log("Chart saved to: " + filePath);
    }

}
