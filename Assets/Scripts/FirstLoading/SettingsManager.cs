using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Networking;

public class GameSettings
{
    public bool isFirstStart = true;

    public float speed = 2.0f;
    public string effectOption = "None";

    // UI 설정 값들 (타이틀: 선택된 인덱스)
    public int displayMode = 0;
    public int displayResolution = 1;
    public int frameLimit = 4;
    public int defaultLanguage = 0;
    public int songInfoLanguage = 0;
    public int judgementLineHeight = 0;
    public int sync = 0;
    public int fastSlowExp = 3;

    public List<string> KeyBinds = new()
    {
#if UNITY_STANDALONE_OSX || UNITY_WEBGL
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

    private Player playerData;

    public string eventName;
    public string songTitle;
    public string songArtist;
    public string fileName;
    private SongInfoClass info;

    public SongInfoClass Info
    {
        get => info;          // 값 가져오기
        set => info = value;  // 값 설정하기
    }

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

    public async Task LoadSettingsInWebGL()
    {
        using (UnityWebRequest req = UnityWebRequest.Get(filePath))
        {
            var op = req.SendWebRequest();
            while (!op.isDone)
                await Task.Yield();

            if (req.result == UnityWebRequest.Result.ConnectionError ||
                req.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"[WebGL] Failed to load settings.json: {req.error}");
                return;
            }

            string json = req.downloadHandler.text;
            if (!string.IsNullOrEmpty(json))
            {
                settings = JsonUtility.FromJson<GameSettings>(json);
                Debug.Log("[WebGL] Settings loaded successfully.");
            }
            else
            {
                Debug.LogError("[WebGL] settings.json is empty.");
            }
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
        int.TryParse(inputed, out settings.sync);
    }

    public void SetSpeed(string inputed)
    {
        float.TryParse(inputed, out settings.speed);
    }

    public void SetKeyBinds(List<string> keys)
    {
        settings.KeyBinds = keys;
    }

    public void SetFirstStart(bool setted)
    {
        settings.isFirstStart = setted;
    }

    public void SetPlayerData(Player setted)
    {
        playerData = setted;
    }

    public Player GetPlayerData()
    {
        return playerData;
    }

    public void UpdateSettingValue(string settingTitle, int newIndex)
    {
        UpdateSettingValueWithoutSave(settingTitle, newIndex);

        // 자동 저장
        SaveSettings();
    }

    public void UpdateSettingValueWithoutSave(string settingTitle, int newIndex)
    {
        // Localization된 타이틀을 키로 사용하기 때문에 매핑 필요
        LocalizationManager locManager = LocalizationManager.Instance;

        if (settingTitle == locManager.GetText("display_mode"))
        {
            settings.displayMode = newIndex;
        }
        else if (settingTitle == locManager.GetText("display_resolution"))
        {
            settings.displayResolution = newIndex;
        }
        else if (settingTitle == locManager.GetText("frame_limit"))
        {
            settings.frameLimit = newIndex;
        }
        else if (settingTitle == locManager.GetText("default_language"))
        {
            settings.defaultLanguage = newIndex;
        }
        else if (settingTitle == locManager.GetText("song_info_language"))
        {
            settings.songInfoLanguage = newIndex;
        }
        else if (settingTitle == locManager.GetText("judgement_line_height"))
        {
            settings.judgementLineHeight = newIndex;
        }
        else if (settingTitle == locManager.GetText("song_output_delay"))
        {
            settings.sync = newIndex;
        }
        else if (settingTitle == locManager.GetText("fast_slow_exp"))
        {
            settings.fastSlowExp = newIndex;
        }
    }

    public int GetSettingValue(string settingTitle)
    {
        LocalizationManager locManager = LocalizationManager.Instance;

        if (settingTitle == locManager.GetText("display_mode"))
        {
            return settings.displayMode;
        }
        else if (settingTitle == locManager.GetText("display_resolution"))
        {
            return settings.displayResolution;
        }
        else if (settingTitle == locManager.GetText("frame_limit"))
        {
            return settings.frameLimit;
        }
        else if (settingTitle == locManager.GetText("default_language"))
        {
            return settings.defaultLanguage;
        }
        else if (settingTitle == locManager.GetText("song_info_language"))
        {
            return settings.songInfoLanguage;
        }
        else if (settingTitle == locManager.GetText("judgement_line_height"))
        {
            return settings.judgementLineHeight;
        }
        else if (settingTitle == locManager.GetText("song_output_delay"))
        {
            return settings.sync;
        }
        else if (settingTitle == locManager.GetText("fast_slow_exp"))
        {
            return settings.fastSlowExp;
        }

        return 0;
    }
}
