using UnityEngine;
using System.IO;
using System.Collections.Generic;

[System.Serializable]
public class SongInfoClass
{
    public string title;
    public string artist;
    public float bpm;
    public string songFile;
}

[System.Serializable]
public class NoteClass
{
    public float beat;
    public float ms;
    public int position;
    public string type;

    public bool isInputed = false;
    public GameObject noteObject;
}

public class LoadManager : MonoBehaviour
{
    public SongInfoClass info;
    public List<NoteClass> notes;

    private MenuManager menu;

    [System.Obsolete]
    private void Start()
    {
        menu = FindObjectOfType<MenuManager>();
        LoadFromJson(Path.Combine(Application.streamingAssetsPath, "test-test", menu.fileName + ".json"));
    }

    public void LoadFromJson(string filePath)
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);

            NotesContainer container = JsonUtility.FromJson<NotesContainer>(json);

            info = container.info;
            notes = container.notes;

            Debug.Log("Chart loaded successfully!");
            Debug.Log($"{info.artist}");
        }
        else
        {
            Debug.LogError("File not found at: " + filePath);
        }
    }

    [System.Serializable]
    private class NotesContainer
    {
        public SongInfoClass info;
        public List<NoteClass> notes;
    }
}
