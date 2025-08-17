using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CompleteLoading : MonoBehaviour
{
    private SettingsManager settingsManager;

    private void Start()
    {
        settingsManager = SettingsManager.Instance;
        settingsManager.LoadSettings(); // 설정 파일 로드

        DontDestroyOnLoad(settingsManager.gameObject);

        if (settingsManager.settings.isFirstStart) // 첫 입장일 때 실행되는 코드
        {
            settingsManager.SetFirstStart(false); // 이제 첫 입장이 아님

            StartTutorial(); // 튜토리얼 실행
        }
        else // 평소에 실행되는 코드
        {
            SceneManager.LoadSceneAsync("Menu"); // 메인 메뉴로 이동
        }
    }

    private void StartTutorial()
    {
        string[] directory = Directory.GetDirectories(Application.streamingAssetsPath, "system"); // system 폴더 불러오기
        if (directory.Length == 0)
        {
            Debug.LogWarning($"system 폴더를 찾지 못했습니다.");
        }

        string[] jsonFiles = Directory.GetFiles(directory[0], "tutorial.roena"); // 튜토리얼 채보 파일 불러오기
        if (jsonFiles.Length == 0)
        {
            Debug.LogWarning($"튜토리얼 파일이 존재하지 않습니다.");
        }

        settingsManager.SetFileName(jsonFiles[0]); // LoadManager에서 사용할 파일 경로
        SceneManager.LoadSceneAsync("Tutorial"); // 튜토리얼 씬으로 이동
    }
}
