using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

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
