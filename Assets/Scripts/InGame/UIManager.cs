using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    // UI Components
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI comboText;
    [SerializeField] private TextMeshProUGUI comboTitleText;
    [SerializeField] private TextMeshProUGUI rateText;
    [SerializeField] private TextMeshProUGUI judgeText;
    [SerializeField] private TextMeshProUGUI plusJudgeText;
    [SerializeField] private TextMeshProUGUI fastSlow;
    [SerializeField] private TextMeshProUGUI perfectpCountText;
    [SerializeField] private TextMeshProUGUI perfectCountText;
    [SerializeField] private TextMeshProUGUI greatCountText;
    [SerializeField] private TextMeshProUGUI goodCountText;
    [SerializeField] private TextMeshProUGUI missCountText;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI artistText;
    [SerializeField] private TextMeshProUGUI FCAPText;
    [SerializeField] private TextMeshProUGUI speedText;
    [SerializeField] private List<SpriteRenderer> fastIndicators;
    [SerializeField] private List<SpriteRenderer> slowIndicators;

    // Classes
    [SerializeField] private LoadManager loadManager;
    private SettingsManager settings;

    // Coroutines
    private Coroutine comboPopInRoutine;
    private Coroutine currentJudgementRoutine;
    private Coroutine popInRoutine;

    private void Start()
    {
        if (!loadManager)
        {
            Debug.LogError("loadManager is not defined");
            return;
        }
        settings = SettingsManager.Instance;
        if (!settings)
        {
            Debug.LogError("Settings is not defined");
            return;
        }

        SetUIs();
    }

    private void SetUIs()
    {
        float level = loadManager.info.level != 0 ? loadManager.info.level : 0;

        SetInitialJudgementText();
        SetLevelText(level);
        SetInitialSongInfoText();
        SetSpeedText();
    }

    private void SetInitialJudgementText()
    {
        Color tempColor = judgeText.color;
        tempColor.a = 0f;
        judgeText.color = tempColor;
        plusJudgeText.color = plusJudgeText.color.SetAlpha(0f);
        fastSlow.color = tempColor;
    }

    private void SetInitialSongInfoText()
    {
        titleText.text = settings.songTitle;
        artistText.text = settings.songArtist;
    }

    public void SetSpeedText()
    {
        speedText.text = $"{settings.settings.speed:F1}";
    }

    public void SetLevelText(float level)
    {
        levelText.text = $"{level}";
    }

    public void SetCombo(int combo)
    {
        comboText.text = $"{combo}";
        comboTitleText.color = comboTitleText.color.SetAlpha(1f);

        if (comboPopInRoutine != null)
        {
            StopCoroutine(comboPopInRoutine);
        }
        comboPopInRoutine = StartCoroutine(PopIn(comboText.rectTransform));
    }

    public void ClearCombo()
    {
        comboText.text = $"";
        comboTitleText.color = comboTitleText.color.SetAlpha(0f);
    }

    public void UpdateJudgeCountText(Dictionary<string, int> judgeCount)
    {
        perfectpCountText.text = $"{judgeCount["PerfectP"]}";
        perfectCountText.text = $"{judgeCount["Perfect"]}";
        greatCountText.text = $"{judgeCount["Great"]}";
        goodCountText.text = $"{judgeCount["Good"]}";
        missCountText.text = $"{judgeCount["Miss"] + judgeCount["Bad"]}";
    }

    public void ChangeRate(float rate)
    {
        rateText.text = $"{rate:F2}%";
    }

    public void SetFCAPText(string FCAP)
    {
        FCAPText.text = FCAP;
    }

    public IEnumerator JudgementTextShower(string judgement, double Ms, int position)
    {
        if (currentJudgementRoutine != null)
        {
            StopCoroutine(currentJudgementRoutine);
        }
        currentJudgementRoutine = StartCoroutine(ShowJudgementTextRoutine(judgement, Ms, position));
        yield break;
    }

    public IEnumerator ShowJudgementTextRoutine(string judgement, double Ms, int position)
    {
        Color tempColor = judgeText.color;
        int index = position - 1;
        tempColor.a = 1f;
        judgeText.color = tempColor;

        // 텍스트 알파값 세팅
        if (judgement == "PerfectP")
        {
            judgeText.text = "PERFECT";
            plusJudgeText.color = plusJudgeText.color.SetAlpha(1f);
        }
        else if (judgement == "Bad")
        {
            judgeText.text = "MISS";
            plusJudgeText.color = plusJudgeText.color.SetAlpha(0f);
        }
        else
        {
            judgeText.text = $"{judgement.ToUpper()}";
            plusJudgeText.color = plusJudgeText.color.SetAlpha(0f);
        }

        // 판정 텍스트 팝인
        if (popInRoutine != null)
        {
            StopCoroutine(popInRoutine);
        }
        popInRoutine = StartCoroutine(PopIn(judgeText.rectTransform));

        // FAST / SLOW 처리
        if (Ms > 0)
        {
            tempColor = fastSlow.color; tempColor.a = 1f;
            fastSlow.color = tempColor;
            fastSlow.text = $"+{(int)Ms}";

            //if (judgement != "Perfect")
            //    StartCoroutine(IndicatorShower(index, true));
        }
        else if (Ms == 0)
        {
            tempColor = fastSlow.color; tempColor.a = 1f;
            fastSlow.color = tempColor;
            fastSlow.text = string.Empty;
        }
        else // Ms < 0
        {
            tempColor = fastSlow.color; tempColor.a = 1f;
            fastSlow.color = tempColor;
            fastSlow.text = $"{(int)Ms}";

            //if (judgement != "Perfect")
            //    StartCoroutine(IndicatorShower(index, false));
        }

        // Perfect+
        //if (plusJudgeText.color.a > 0.99f)
        //    StartCoroutine(PopIn(plusJudgeText.rectTransform));

        // 2초 유지
        yield return new WaitForSeconds(2f);

        // 숨기기
        judgeText.color = judgeText.color.SetAlpha(0f);
        plusJudgeText.color = plusJudgeText.color.SetAlpha(0f);
        fastSlow.color = fastSlow.color.SetAlpha(0f);

        currentJudgementRoutine = null;
    }

    public IEnumerator IndicatorSetter()
    {
        //for (int i = 0; i < 4; i++)
        //{
        //    fastIndicators[i].color = fastIndicators[i].color.SetAlpha(0f);
        //    slowIndicators[i].color = slowIndicators[i].color.SetAlpha(0f);
        //}

        yield break;
    }

    public IEnumerator IndicatorShower(int index, bool isFast)
    {
        Color tempColor;

        if (isFast)
        {
            tempColor = fastIndicators[index].color;
            tempColor.a = 1f;
            fastIndicators[index].color = tempColor;

            yield return new WaitForSeconds(1f);

            tempColor.a = 0f;
            fastIndicators[index].color = tempColor;
        }
        else
        {
            tempColor = slowIndicators[index].color;
            tempColor.a = 1f;
            slowIndicators[index].color = tempColor;

            yield return new WaitForSeconds(1f);

            tempColor.a = 0f;
            slowIndicators[index].color = tempColor;
        }
    }

    private IEnumerator PopIn(RectTransform target, float fromScale = 0.3f, float toScale = 1f, float duration = 0.07f)
    {
        if (target == null) yield break;

        Vector3 start = Vector3.one * fromScale;
        Vector3 end = Vector3.one * toScale;

        float t = 0f;
        target.localScale = start;

        // EaseOutSine Easing
        float EaseOutSine(float t)
        {
            return Mathf.Sin((t * Mathf.PI) / 2f);
        }

        while (t < duration)
        {
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / duration);
            float e = EaseOutSine(p);
            target.localScale = Vector3.LerpUnclamped(start, end, e);
            yield return null;
        }

        target.localScale = end;
    }
}
