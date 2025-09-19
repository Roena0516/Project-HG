using TMPro;
using UnityEngine;

public class ResultUIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI level;
    [SerializeField] private TextMeshProUGUI Artist;
    [SerializeField] private TextMeshProUGUI Title;
    [SerializeField] private TextMeshProUGUI PerfectPlusCount;
    [SerializeField] private TextMeshProUGUI PerfectCount;
    [SerializeField] private TextMeshProUGUI GreatCount;
    [SerializeField] private TextMeshProUGUI GoodCount;
    [SerializeField] private TextMeshProUGUI MissCount;
    [SerializeField] private TextMeshProUGUI Rate;
    [SerializeField] private TextMeshProUGUI FCAP;
    [SerializeField] private TextMeshProUGUI Rank;

    private LoadManager loadManager;
    private JudgementManager judgementManager;

    public void SetResultUIs(LoadManager loadManager_arg, JudgementManager judgementManager_arg)
    {
        loadManager = loadManager_arg;
        judgementManager = judgementManager_arg;

        SetBoardText();
        SetRightText();
    }

    private void SetBoardText()
    {
        level.text = $"{loadManager.info.level}";
        Artist.text = loadManager.info.artist;
        Title.text = loadManager.info.title;
        PerfectPlusCount.text = $"{judgementManager.judgeCount["PerfectP"]}";
        PerfectCount.text = $"{judgementManager.judgeCount["Perfect"]}";
        GreatCount.text = $"{judgementManager.judgeCount["Great"]}";
        GoodCount.text = $"{judgementManager.judgeCount["Good"]}";
        MissCount.text = $"{judgementManager.judgeCount["Miss"] + judgementManager.judgeCount["Bad"]}";
        Rate.text = $"{judgementManager.rate:F2}%";
    }

    private void SetRightText()
    {
        SetFCAPText();
        SetRankText(judgementManager.rate);
    }

    private void SetRankText(float rate)
    {
        if (rate >= 99.75f)
        {
            Rank.text = "SSS+";
        }
        else if (rate >= 99.5f)
        {
            Rank.text = "SSS";
        }
        else if (rate >= 99.25f)
        {
            Rank.text = "SS+";
        }
        else if (rate >= 99.0f)
        {
            Rank.text = "SS";
        }
        else if (rate >= 98.5f)
        {
            Rank.text = "S+";
        }
        else if (rate >= 98.0f)
        {
            Rank.text = "S";
        }
        else if (rate >= 97.0f)
        {
            Rank.text = "AAA";
        }
        else if (rate >= 96.0f)
        {
            Rank.text = "AA";
        }
        else if (rate >= 95.0f)
        {
            Rank.text = "A";
        }
        else if (rate >= 94.0f)
        {
            Rank.text = "BBB";
        }
        else if (rate >= 93.0f)
        {
            Rank.text = "BB";
        }
        else if (rate >= 92.0f)
        {
            Rank.text = "B";
        }
        else if (rate >= 90.0f)
        {
            Rank.text = "C";
        }
        else
        {
            Rank.text = "D";
        }
    }

    private void SetFCAPText()
    {
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
