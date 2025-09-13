using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.IO;

public class ResultManager : MonoBehaviour
{
    private JudgementManager judgementManager;
    private LoadManager loadManager;

    public TextMeshProUGUI Artist;
    public TextMeshProUGUI Title;
    public TextMeshProUGUI PerfectPlusCount;
    public TextMeshProUGUI PerfectCount;
    public TextMeshProUGUI GreatCount;
    public TextMeshProUGUI GoodCount;
    public TextMeshProUGUI MissCount;
    public TextMeshProUGUI Rate;
    public TextMeshProUGUI FCAP;

    private void Start()
    {
        judgementManager = JudgementManager.Instance;
        loadManager = LoadManager.Instance;

        SaveResult();
        SetResult();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Destroy(judgementManager.gameObject);
            SceneManager.LoadSceneAsync("FreePlay");
        }
    }

    private void SaveResult()
    {
        Result newResult = new()
        {
            playerId = "test",
            musicId = loadManager.info.id,
            rate = judgementManager.rate,
            combo = judgementManager.combo,
            perfectPlus = judgementManager.judgeCount["PerfectP"],
            perfect = judgementManager.judgeCount["Perfect"],
            great = judgementManager.judgeCount["Great"],
            good = judgementManager.judgeCount["Good"],
            miss = judgementManager.judgeCount["Miss"] + judgementManager.judgeCount["Bad"],
            played_at = ""
        };

        // JSON 파일 경로
        string path = Path.Combine(Application.streamingAssetsPath, "result.json");

        ResultsContainer container;

        // 기존 파일이 있으면 읽어서 역직렬화
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            container = JsonUtility.FromJson<ResultsContainer>(json);

            if (container.results == null)
            {
                container.results = new List<Result>();
            }
        }
        else
        {
            // 없으면 새 컨테이너 생성
            container = new ResultsContainer();

            Debug.Log("result.json이 존재하지 않습니다.");
        }

        // 새로운 결과 추가
        container.results.Add(newResult);

        // 다시 직렬화해서 저장
        string newJson = JsonUtility.ToJson(container, true);
        File.WriteAllText(path, newJson);

        Debug.Log("result.json is successfully saved");
    }

    private void SetResult()
    {
        Artist.text = loadManager.info.artist;
        Title.text= loadManager.info.title;
        PerfectPlusCount.text = $"{judgementManager.judgeCount["PerfectP"]}";
        PerfectCount.text = $"{judgementManager.judgeCount["Perfect"]}";
        GreatCount.text = $"{judgementManager.judgeCount["Great"]}";
        GoodCount.text = $"{judgementManager.judgeCount["Good"]}";
        MissCount.text = $"{judgementManager.judgeCount["Miss"] + judgementManager.judgeCount["Bad"]}";

        Rate.text = $"{judgementManager.rate:F2}%";

        if (judgementManager.rate < 80)
        {
            FCAP.text = "FAILED";
        }

        if (judgementManager.isFC)
        {
            FCAP.text = "FULL COMBO";
        }
        if (judgementManager.isAP)
        {
            FCAP.text = "ALL PERFECT";
        }
    }
}
