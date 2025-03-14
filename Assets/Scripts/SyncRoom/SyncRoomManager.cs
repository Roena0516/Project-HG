using UnityEngine;
using TMPro;

public class SyncRoomManager : MonoBehaviour
{
    public int inputConut;
    public int msCount;

    public int avg;

    public TextMeshProUGUI avgText;
    public TextMeshProUGUI infoText;

    public static SyncRoomManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        inputConut = 0;
        msCount = 0;
        avg = 0;

        avgText.text = $"평균 입력 : {avg}ms";
        infoText.text = $"{inputConut}번의 입력, 입력 시간의 합 : {msCount}ms";
    }

    public void CalcAvg()
    {
        avg = msCount / inputConut;
        avgText.text = $"평균 입력 : {avg}ms";
        infoText.text = $"{inputConut}번의 입력, 입력 시간의 합 : {msCount}ms";
    }
}
