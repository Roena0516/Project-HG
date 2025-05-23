using UnityEngine;
using System.IO;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

[System.Serializable]
public class SongInfoClass
{
    public string title;
    public string artist;
    public float bpm;
    public string songFile;
    public string eventName;
    public float level;
    public string difficulty;
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

    public bool isSyncRoom;

    public GameObject noteObject;
}

public class LoadManager : MonoBehaviour
{
    public SongInfoClass info;
    public List<NoteClass> notes;

    private SettingsManager settings;
    private LevelEditer levelEditer;
    public GameManager gameManager;

    public static LoadManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        settings = SettingsManager.Instance;
        levelEditer = LevelEditer.Instance;

        if (!SceneManager.GetSceneByName("LevelEditor").isLoaded)
        {
            LoadFromJson(Path.Combine(settings.fileName));
        }
        else
        {
            info = levelEditer.saveManager.info;
            notes = levelEditer.saveManager.notes;

            settings.eventName = info.eventName;

            Debug.Log($"1{levelEditer.eventName}");

            Debug.Log("Chart loaded successfully!");
            Debug.Log($"{info.artist}");
        }
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
