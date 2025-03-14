using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class SyncRoomResultManager : MonoBehaviour
{
    private GameManager gameManager;
    private JudgementManager judgementManager;
    private SyncRoomManager syncRoomManager;
    private SettingsManager settings;

    public TextMeshProUGUI avgText;
    public TextMeshProUGUI infoText;

    public TextMeshProUGUI PerfectCount;
    public TextMeshProUGUI GreatCount;
    public TextMeshProUGUI GoodCount;
    public TextMeshProUGUI BadCount;
    public TextMeshProUGUI MissCount;
    public TextMeshProUGUI Rate;
    public TextMeshProUGUI FCAP;

    private void Start()
    {
        gameManager = GameManager.Instance;
        judgementManager = JudgementManager.Instance;
        syncRoomManager = SyncRoomManager.Instance;
        settings = SettingsManager.Instance;

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

        avgText.text = $"평균 입력 : {syncRoomManager.avg}ms";
        infoText.text = $"{syncRoomManager.inputConut}번의 입력, 입력 시간의 합 : {syncRoomManager.msCount}ms";
    }

    public void SetSync()
    {
        settings.sync = syncRoomManager.avg;
    }
}
