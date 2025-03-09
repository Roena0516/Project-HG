using UnityEngine;
using TMPro;

public class SyncRoomManager : MonoBehaviour
{
    public int inputConut;
    public int msCount;

    private int avg;

    public TextMeshProUGUI avgText;

    private void Start()
    {
        inputConut = 0;
        msCount = 0;
        avg = 0;

        avgText.text = $"평균 입력 : {avg}ms";
    }

    public void CalcAvg()
    {
        avg = msCount / inputConut;
        avgText.text = $"평균 입력 : {avg}ms";
    }
}
