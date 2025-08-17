using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CompleteLoading : MonoBehaviour
{
    private SettingsManager settingsManager;

    private void Start()
    {
        settingsManager = SettingsManager.Instance;
        settingsManager.LoadSettings(); // ���� ���� �ε�

        DontDestroyOnLoad(settingsManager.gameObject);

        if (settingsManager.settings.isFirstStart) // ù ������ �� ����Ǵ� �ڵ�
        {
            settingsManager.SetFirstStart(false); // ���� ù ������ �ƴ�

            StartTutorial(); // Ʃ�丮�� ����
        }
        else // ��ҿ� ����Ǵ� �ڵ�
        {
            SceneManager.LoadSceneAsync("Menu"); // ���� �޴��� �̵�
        }
    }

    private void StartTutorial()
    {
        string[] directory = Directory.GetDirectories(Application.streamingAssetsPath, "system"); // system ���� �ҷ�����
        if (directory.Length == 0)
        {
            Debug.LogWarning($"system ������ ã�� ���߽��ϴ�.");
        }

        string[] jsonFiles = Directory.GetFiles(directory[0], "tutorial.roena"); // Ʃ�丮�� ä�� ���� �ҷ�����
        if (jsonFiles.Length == 0)
        {
            Debug.LogWarning($"Ʃ�丮�� ������ �������� �ʽ��ϴ�.");
        }

        settingsManager.SetFileName(jsonFiles[0]); // LoadManager���� ����� ���� ���
        SceneManager.LoadSceneAsync("Tutorial"); // Ʃ�丮�� ������ �̵�
    }
}
