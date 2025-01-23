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
    public TextMeshProUGUI PerfectCount;
    public TextMeshProUGUI GreatCount;
    public TextMeshProUGUI GoodCount;
    public TextMeshProUGUI BadCount;
    public TextMeshProUGUI MissCount;
    public TextMeshProUGUI Rate;
    public TextMeshProUGUI FCAP;

    [System.Obsolete]
    private void Start()
    {
        judgementManager = FindObjectOfType<JudgementManager>();
        loadManager = FindObjectOfType<LoadManager>();

        SetResult();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Destroy(judgementManager.gameObject);
            SceneManager.LoadScene("FreePlay");
        }
    }

    private void SetResult()
    {
        Artist.text = loadManager.info.artist;
        Title.text= loadManager.info.title;

        PerfectCount.text = $"{judgementManager.judgeCount["Perfect"]}";
        GreatCount.text = $"{judgementManager.judgeCount["Great"]}";
        GoodCount.text = $"{judgementManager.judgeCount["Good"]}";
        BadCount.text = $"{judgementManager.judgeCount["Bad"]}";
        MissCount.text = $"{judgementManager.judgeCount["Miss"]}";

        Rate.text = $"{judgementManager.rate:F2}%";

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
