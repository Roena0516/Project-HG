using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance { get; private set; }

    private Dictionary<string, string> localizedTexts = new();
    private string currentLanguage = "kr";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadLanguage(string languageCode)
    {
        currentLanguage = languageCode;
        string path = Path.Combine(Application.streamingAssetsPath, "Localization", $"{languageCode}.json");

#if UNITY_WEBGL && !UNITY_EDITOR
        StartCoroutine(LoadInWebGL(languageCode));
#else
        string json = File.ReadAllText(path);
        localizedTexts = JsonUtility.FromJson<LocalizationDictionary>(json).ToDictionary();
        Debug.Log($"[LocalizationManager] Language '{languageCode}' loaded successfully");
#endif
    }

    public IEnumerator LoadInWebGL(string languageCode)
    {
        currentLanguage = languageCode;
        string path = Path.Combine(Application.streamingAssetsPath, "Localization", $"{languageCode}.json");

        using (var req = UnityWebRequest.Get(path))
        {
            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
            {
                localizedTexts = JsonUtility.FromJson<LocalizationDictionary>(req.downloadHandler.text).ToDictionary();
                Debug.Log($"[LocalizationManager] Language '{languageCode}' loaded successfully (WebGL)");
            }
            else
            {
                Debug.LogError($"[LocalizationManager] Failed to load language '{languageCode}': {req.error}");
            }
        }
    }

    public string GetText(string key)
    {
        if (localizedTexts.TryGetValue(key, out var value))
            return value;
        return $"<Missing Text: {key}>";
    }

    [System.Serializable]
    private class LocalizationDictionary
    {
        public List<LocalizationEntry> entries;
        public Dictionary<string, string> ToDictionary()
        {
            var dict = new Dictionary<string, string>();
            foreach (var e in entries)
                dict[e.key] = e.value;
            return dict;
        }
    }

    [System.Serializable]
    private class LocalizationEntry
    {
        public string key;
        public string value;
    }
}