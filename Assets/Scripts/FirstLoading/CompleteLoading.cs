using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CompleteLoading : MonoBehaviour
{
    private SettingsManager settingsManager;

    private void Start()
    {
        settingsManager = SettingsManager.Instance;
        settingsManager.LoadSettings();

        DontDestroyOnLoad(settingsManager.gameObject);

        SceneManager.LoadSceneAsync("Menu");
    }
}
