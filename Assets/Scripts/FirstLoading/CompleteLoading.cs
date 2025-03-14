using UnityEngine;
using UnityEngine.SceneManagement;

public class CompleteLoading : MonoBehaviour
{
    private SettingsManager settingsManager;

    private void Start()
    {
        settingsManager = SettingsManager.Instance;
        settingsManager.sync = 0f;
        settingsManager.speed = 4.0f;

        DontDestroyOnLoad(settingsManager.gameObject);

        SceneManager.LoadScene("Menu");
    }
}
