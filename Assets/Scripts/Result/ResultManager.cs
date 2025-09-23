using FMOD.Studio;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultManager : MonoBehaviour
{
    private JudgementManager judgementManager;
    private LoadManager loadManager;
    private SettingsManager settings;
    [SerializeField] private ResultUIManager UIManager;
    [SerializeField] private ResultAPI resultAPI;

    private string baseUrl = "https://prod.windeath44.wiki/api";
    private string accessToken;

    private void Start()
    {
        judgementManager = JudgementManager.Instance;
        loadManager = LoadManager.Instance;
        settings = SettingsManager.Instance;

        SaveResult();
        UIManager.SetResultUIs(loadManager, judgementManager);
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
        accessToken = $"{settings.GetPlayerData().accessToken}";

        string rank = SetRank(judgementManager.rate);
        string state = SetFCAP();

        ResultRequest newResult = new()
        {
            musicId = loadManager.info.id,
            completionRate = judgementManager.rate,
            combo = judgementManager.combo,
            perfectPlus = judgementManager.judgeCount["PerfectP"],
            perfect = judgementManager.judgeCount["Perfect"],
            great = judgementManager.judgeCount["Great"],
            good = judgementManager.judgeCount["Good"],
            miss = judgementManager.judgeCount["Miss"] + judgementManager.judgeCount["Bad"],
            state = state,
        };

        StartCoroutine(resultAPI.PostResultAPI(baseUrl, newResult, accessToken, onSuccess: (res) =>
        {
            Debug.Log($"Saved! id={res.gamePlayHistoryId}, rank={res.rank}, state={res.state}");
        }, onError: (err) =>
        {
            Debug.LogWarning("Save failed: " + err);
        }));
    }

    private string SetFCAP()
    {
        string FCAP = "CLEAR";

        if (judgementManager.rate < 80)
        {
            FCAP = "FAILED";
        }
        if (judgementManager.isFC)
        {
            FCAP = "FULL COMBO";
        }
        if (judgementManager.isAP)
        {
            FCAP = "ALL PERFECT";
        }

        return FCAP;
    }

    private string SetRank(float rate)
    {
        string rank;

        if (rate >= 99.75f)
        {
            rank = "SSS+";
        }
        else if (rate >= 99.5f)
        {
            rank = "SSS";
        }
        else if (rate >= 99.25f)
        {
            rank = "SS+";
        }
        else if (rate >= 99.0f)
        {
            rank = "SS";
        }
        else if (rate >= 98.5f)
        {
            rank = "S+";
        }
        else if (rate >= 98.0f)
        {
            rank = "S";
        }
        else if (rate >= 97.0f)
        {
            rank = "AAA";
        }
        else if (rate >= 96.0f)
        {
            rank = "AA";
        }
        else if (rate >= 95.0f)
        {
            rank = "A";
        }
        else if (rate >= 94.0f)
        {
            rank = "BBB";
        }
        else if (rate >= 93.0f)
        {
            rank = "BB";
        }
        else if (rate >= 92.0f)
        {
            rank = "B";
        }
        else if (rate >= 90.0f)
        {
            rank = "C";
        }
        else
        {
            rank = "D";
        }

        return rank;
    }
}
