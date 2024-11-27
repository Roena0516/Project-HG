using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class NoteDataWrapper
{
    public List<NoteClass> notes; // 노트 데이터를 감싸는 클래스
}

public class SaveManager : MonoBehaviour
{
    public List<NoteClass> notes;

    public void SaveToJson(string filePath)
    {
        // NoteDataWrapper의 인스턴스를 생성하고 데이터 할당
        NoteDataWrapper wrapper = new NoteDataWrapper();
        wrapper.notes = notes;

        // JSON 문자열로 변환
        string json = JsonUtility.ToJson(wrapper, true); // prettyPrint를 true로 설정

        // 파일로 저장
        File.WriteAllText(filePath, json);
        Debug.Log("Chart saved to: " + filePath);
    }

}
