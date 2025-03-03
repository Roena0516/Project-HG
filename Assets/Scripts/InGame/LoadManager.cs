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
    public string eventName;
}

[System.Serializable]
public class NoteClass
{
    public float beat;
    public float ms;
    public int position;
    public string type;

    public bool isInputed = false;
    public bool isEndNote = false;
    public GameObject noteObject;
}

public class LoadManager : MonoBehaviour
{
    public SongInfoClass info;
    public List<NoteClass> notes;

    private SettingsManager settings;

    [System.Obsolete]
    private void Start()
    {
        settings = FindObjectOfType<SettingsManager>();
        LoadFromJson(Path.Combine(Application.streamingAssetsPath, settings.fileName + ".roena"));
    }

    public void LoadFromJson(string filePath)
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);

            string decrypted = EncryptionHelper.Decrypt(json);

            NotesContainer container = JsonUtility.FromJson<NotesContainer>(decrypted);

            info = container.info;
            notes = container.notes;

            settings.eventName = info.eventName;

            Debug.Log($"1{settings.eventName}");

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
