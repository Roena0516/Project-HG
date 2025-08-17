using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameSettings
{
    public bool isFirstStart = true;

    public float sync = 0f;
    public float speed = 2.0f;
    public string effectOption = "None";
    public bool isKR = false;


    public List<string> KeyBinds = new()
    {
#if UNITY_STANDALONE_OSX
        "keyboard/D",
        "keyboard/F",
        "keyboard/J",
        "keyboard/K"
#elif UNITY_STANDALONE_WIN
        "D",
        "F",
        "J",
        "K"
#endif
    };
}

public class SettingsManager : MonoBehaviour
{
    public GameSettings settings;

    private static readonly string filePath = Application.streamingAssetsPath + "/settings.json";

    public string eventName;
    public string songTitle;
    public string songArtist;
    public string fileName;

    public MainInputAction action;

    public List<InputAction> LineActions;

    public bool isAutoPlay;

    public bool isKR;

    public static SettingsManager Instance { get; private set; }

    private void Awake()
    {
        action = new MainInputAction();
        LineActions.Add(action.Player.Line1Action);
        LineActions.Add(action.Player.Line2Action);
        LineActions.Add(action.Player.Line3Action);
        LineActions.Add(action.Player.Line4Action);

        settings = new();

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadSettings()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            settings = JsonUtility.FromJson<GameSettings>(json);

            for (int i = 0; i < 4; i++)
            {
                LineActions[i].ApplyBindingOverride(settings.KeyBinds[i]);
            }

            Debug.Log("settings.json loaded successfully");
        }
        else
        {
            Debug.LogError("settings.json not found at: " + filePath);
            SaveSettings();
        }
    }

    public void SaveSettings()
    {
        string json = JsonUtility.ToJson(settings, true);

        File.WriteAllText(filePath, json);
        Debug.Log("settings.json saved to: " + filePath);
    }

    [System.Obsolete]
    private void OnEnable()
    {
        for (int i = 0; i < 4; i++)
        {
            LineActions[i].Enable();
        }
    }

    [System.Obsolete]
    private void OnDisable()
    {
        for (int i = 0; i < 4; i++)
        {
            LineActions[i].Disable();
        }
    }

    public void SetFileName(string inputed)
    {
        fileName = inputed;
    }

    public void SetSongTitle(string inputed)
    {
        songTitle = inputed;
    }

    public void SetSongArtist(string inputed)
    {
        songArtist = inputed;
    }

    public void SetEventName(string inputed)
    {
        eventName = inputed;
    }

    public void SetSync(string inputed)
    {
        float.TryParse(inputed, out settings.sync);
    }

    public void SetSpeed(string inputed)
    {
        float.TryParse(inputed, out settings.speed);
    }

    public void SetKeyBinds(List<string> keys)
    {
        settings.KeyBinds = keys;
    }

    public void SetToKR(bool setted)
    {
        settings.isKR = setted;
    }

    public void SetFirstStart(bool setted)
    {
        settings.isFirstStart = setted;
    }
}
