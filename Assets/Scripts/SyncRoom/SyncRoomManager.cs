using UnityEngine;
using TMPro;

public class SyncRoomManager : MonoBehaviour
{
    public int inputConut;
    public int msCount;

    public int avg;

    public TextMeshProUGUI avgText;
    public TextMeshProUGUI infoText;

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
