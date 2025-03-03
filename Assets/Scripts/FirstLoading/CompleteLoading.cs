using UnityEngine;
using UnityEngine.SceneManagement;

public class CompleteLoading : MonoBehaviour
{
    private SettingsManager settingsManager;

    [System.Obsolete]
    private void Start()
    {
        settingsManager = FindObjectOfType<SettingsManager>();
        settingsManager.sync = 0f;
        settingsManager.speed = 4.0f;

        DontDestroyOnLoad(settingsManager.gameObject);

        SceneManager.LoadScene("Menu");
    }
}
