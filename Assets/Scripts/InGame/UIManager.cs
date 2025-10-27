using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    // UI Components
    [SerializeField] private TextMeshProUGUI _levelText;
    [SerializeField] private TextMeshProUGUI _comboText;
    [SerializeField] private TextMeshProUGUI _comboTitleText;
    [SerializeField] private TextMeshProUGUI _rateText;
    [SerializeField] private TextMeshProUGUI _judgeText;
    [SerializeField] private TextMeshProUGUI _plusJudgeText;
    [SerializeField] private TextMeshProUGUI _fastSlow;
    [SerializeField] private TextMeshProUGUI _perfectpCountText;
    [SerializeField] private TextMeshProUGUI _perfectCountText;
    [SerializeField] private TextMeshProUGUI _greatCountText;
    [SerializeField] private TextMeshProUGUI _goodCountText;
    [SerializeField] private TextMeshProUGUI _missCountText;
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private TextMeshProUGUI _artistText;
    [SerializeField] private TextMeshProUGUI _FCAPText;
    [SerializeField] private TextMeshProUGUI _speedText;
    [SerializeField] private List<TextMeshProUGUI> _fastIndicators;
    [SerializeField] private List<TextMeshProUGUI> _slowIndicators;

    // Classes
    [SerializeField] private LoadManager _loadManager;
    private SettingsManager _settings;

    // Coroutines
    private Coroutine _comboPopInRoutine;
    private Coroutine _currentJudgementRoutine;
    private Coroutine _currentLeftIndicatorRoutine;
    private Coroutine _currentRightIndicatorRoutine;
    private Coroutine _popInRoutine;

    private void Start()
    {
        if (!_loadManager)
        {
            Debug.LogError("loadManager is not defined");
            return;
        }
        _settings = SettingsManager.Instance;
        if (!_settings)
        {
            Debug.LogError("Settings is not defined");
            return;
        }

        SetUIs();
    }

    private void SetUIs()
    {
        float level = _loadManager.info.level != 0 ? _loadManager.info.level : 0;

        SetInitialJudgementText();
        SetLevelText(level);
        SetInitialSongInfoText();
        SetSpeedText();
    }

    private void SetInitialJudgementText()
    {
        _judgeText.color = _judgeText.color.SetAlpha(0f);
        _plusJudgeText.color = _plusJudgeText.color.SetAlpha(0f);
        _fastSlow.color = _fastSlow.color.SetAlpha(0f);
        _fastIndicators[0].color = _fastIndicators[0].color.SetAlpha(0f);
        _slowIndicators[0].color = _slowIndicators[0].color.SetAlpha(0f);
        _fastIndicators[1].color = _fastIndicators[1].color.SetAlpha(0f);
        _slowIndicators[1].color = _slowIndicators[1].color.SetAlpha(0f);
    }

    private void SetInitialSongInfoText()
    {
        _titleText.text = _settings.songTitle;
        _artistText.text = _settings.songArtist;
    }

    public void SetSpeedText()
    {
        _speedText.text = $"{_settings.settings.speed:F1}";
    }

    public void SetLevelText(float level)
    {
        _levelText.text = $"{level}";
    }

    public void SetCombo(int combo)
    {
        _comboText.text = $"{combo}";
        _comboTitleText.color = _comboTitleText.color.SetAlpha(1f);

        if (_comboPopInRoutine != null)
        {
            StopCoroutine(_comboPopInRoutine);
        }
        _comboPopInRoutine = StartCoroutine(PopIn(_comboText.rectTransform));
    }

    public void ClearCombo()
    {
        _comboText.text = $"";
        _comboTitleText.color = _comboTitleText.color.SetAlpha(0f);
    }

    public void UpdateJudgeCountText(Dictionary<string, int> judgeCount)
    {
        _perfectpCountText.text = $"{judgeCount["PerfectP"]}";
        _perfectCountText.text = $"{judgeCount["Perfect"]}";
        _greatCountText.text = $"{judgeCount["Great"]}";
        _goodCountText.text = $"{judgeCount["Good"]}";
        _missCountText.text = $"{judgeCount["Miss"] + judgeCount["Bad"]}";
    }

    public void ChangeRate(float rate)
    {
        _rateText.text = $"{rate:F2}%";
    }

    public void SetFCAPText(string FCAP)
    {
        _FCAPText.text = FCAP;
    }

    public IEnumerator JudgementTextShower(string judgement, double Ms, int position)
    {
        if (_currentJudgementRoutine != null)
        {
            StopCoroutine(_currentJudgementRoutine);
        }
        _currentJudgementRoutine = StartCoroutine(ShowJudgementTextRoutine(judgement, Ms, position));
        yield break;
    }

    public IEnumerator ShowJudgementTextRoutine(string judgement, double Ms, int position)
    {
        Color tempColor = _judgeText.color;
        int index = position - 1;
        tempColor.a = 1f;
        _judgeText.color = tempColor;

        // 텍스트 알파값 세팅
        if (judgement == "PerfectP")
        {
            _judgeText.text = "PERFECT";
            _plusJudgeText.color = _plusJudgeText.color.SetAlpha(1f);
        }
        else if (judgement == "Bad")
        {
            _judgeText.text = "MISS";
            _plusJudgeText.color = _plusJudgeText.color.SetAlpha(0f);
        }
        else
        {
            _judgeText.text = $"{judgement.ToUpper()}";
            _plusJudgeText.color = _plusJudgeText.color.SetAlpha(0f);
        }

        // 판정 텍스트 팝인
        if (_popInRoutine != null)
        {
            StopCoroutine(_popInRoutine);
        }
        _popInRoutine = StartCoroutine(PopIn(_judgeText.rectTransform));

        bool isIndicatorLeft = index <= 1;

        _fastSlow.color = _fastSlow.color.SetAlpha(1f);
        _fastSlow.text = string.Empty;

        // FAST / SLOW 처리
        if (Ms > 0)
        {
            if (_settings.settings.fastSlowExp != 0 && _settings.settings.fastSlowExp >= 1)
            {
                if (judgement == "Good" || judgement == "Miss")
                {
                    _fastSlow.color = _fastSlow.color.SetAlpha(1f);
                    _fastSlow.text = $"+{(int)Ms}";

                    if (isIndicatorLeft)
                    {
                        if (_currentLeftIndicatorRoutine != null)
                        {
                            StopCoroutine(_currentLeftIndicatorRoutine);
                        }
                        _currentLeftIndicatorRoutine = StartCoroutine(IndicatorShower(isIndicatorLeft, true));
                    }
                    else
                    {
                        if (_currentRightIndicatorRoutine != null)
                        {
                            StopCoroutine(_currentRightIndicatorRoutine);
                        }
                        _currentRightIndicatorRoutine = StartCoroutine(IndicatorShower(isIndicatorLeft, true));
                    }
                }
            }
            if (_settings.settings.fastSlowExp != 0 && _settings.settings.fastSlowExp >= 2)
            {
                if (judgement == "Great")
                {
                    _fastSlow.color = _fastSlow.color.SetAlpha(1f);
                    _fastSlow.text = $"+{(int)Ms}";

                    if (isIndicatorLeft)
                    {
                        if (_currentLeftIndicatorRoutine != null)
                        {
                            StopCoroutine(_currentLeftIndicatorRoutine);
                        }
                        _currentLeftIndicatorRoutine = StartCoroutine(IndicatorShower(isIndicatorLeft, true));
                    }
                    else
                    {
                        if (_currentRightIndicatorRoutine != null)
                        {
                            StopCoroutine(_currentRightIndicatorRoutine);
                        }
                        _currentRightIndicatorRoutine = StartCoroutine(IndicatorShower(isIndicatorLeft, true));
                    }
                }
            }
            if (_settings.settings.fastSlowExp != 0 && _settings.settings.fastSlowExp >= 3)
            {
                if (judgement == "Perfect")
                {
                    _fastSlow.color = _fastSlow.color.SetAlpha(1f);
                    _fastSlow.text = $"+{(int)Ms}";

                    if (isIndicatorLeft)
                    {
                        if (_currentLeftIndicatorRoutine != null)
                        {
                            StopCoroutine(_currentLeftIndicatorRoutine);
                        }
                        _currentLeftIndicatorRoutine = StartCoroutine(IndicatorShower(isIndicatorLeft, true));
                    }
                    else
                    {
                        if (_currentRightIndicatorRoutine != null)
                        {
                            StopCoroutine(_currentRightIndicatorRoutine);
                        }
                        _currentRightIndicatorRoutine = StartCoroutine(IndicatorShower(isIndicatorLeft, true));
                    }
                }
            }
        }
        else if (Ms == 0)
        {
            _fastSlow.color = _fastSlow.color.SetAlpha(1f);
            _fastSlow.text = string.Empty;
        }
        else // Ms < 0
        {
            if (_settings.settings.fastSlowExp != 0 && _settings.settings.fastSlowExp >= 1)
            {
                if (judgement == "Good" || judgement == "Miss")
                {
                    _fastSlow.color = _fastSlow.color.SetAlpha(1f);
                    _fastSlow.text = $"-{(int)Ms}";

                    if (isIndicatorLeft)
                    {
                        if (_currentLeftIndicatorRoutine != null)
                        {
                            StopCoroutine(_currentLeftIndicatorRoutine);
                        }
                        _currentLeftIndicatorRoutine = StartCoroutine(IndicatorShower(isIndicatorLeft, false));
                    }
                    else
                    {
                        if (_currentRightIndicatorRoutine != null)
                        {
                            StopCoroutine(_currentRightIndicatorRoutine);
                        }
                        _currentRightIndicatorRoutine = StartCoroutine(IndicatorShower(isIndicatorLeft, false));
                    }
                }
            }
            if (_settings.settings.fastSlowExp != 0 && _settings.settings.fastSlowExp >= 2)
            {
                if (judgement == "Great")
                {
                    _fastSlow.color = _fastSlow.color.SetAlpha(1f);
                    _fastSlow.text = $"-{(int)Ms}";

                    if (isIndicatorLeft)
                    {
                        if (_currentLeftIndicatorRoutine != null)
                        {
                            StopCoroutine(_currentLeftIndicatorRoutine);
                        }
                        _currentLeftIndicatorRoutine = StartCoroutine(IndicatorShower(isIndicatorLeft, false));
                    }
                    else
                    {
                        if (_currentRightIndicatorRoutine != null)
                        {
                            StopCoroutine(_currentRightIndicatorRoutine);
                        }
                        _currentRightIndicatorRoutine = StartCoroutine(IndicatorShower(isIndicatorLeft, false));
                    }
                }
            }
            if (_settings.settings.fastSlowExp != 0 && _settings.settings.fastSlowExp >= 3)
            {
                if (judgement == "Perfect")
                {
                    _fastSlow.color = _fastSlow.color.SetAlpha(1f);
                    _fastSlow.text = $"-{(int)Ms}";

                    if (isIndicatorLeft)
                    {
                        if (_currentLeftIndicatorRoutine != null)
                        {
                            StopCoroutine(_currentLeftIndicatorRoutine);
                        }
                        _currentLeftIndicatorRoutine = StartCoroutine(IndicatorShower(isIndicatorLeft, false));
                    }
                    else
                    {
                        if (_currentRightIndicatorRoutine != null)
                        {
                            StopCoroutine(_currentRightIndicatorRoutine);
                        }
                        _currentRightIndicatorRoutine = StartCoroutine(IndicatorShower(isIndicatorLeft, false));
                    }
                }
            }
        }

        // Perfect+
        //if (plusJudgeText.color.a > 0.99f)
        //    StartCoroutine(PopIn(plusJudgeText.rectTransform));

        // 2초 유지
        yield return new WaitForSeconds(2f);

        // 숨기기
        _judgeText.color = _judgeText.color.SetAlpha(0f);
        _plusJudgeText.color = _plusJudgeText.color.SetAlpha(0f);
        _fastSlow.color = _fastSlow.color.SetAlpha(0f);

        _currentJudgementRoutine = null;
    }

    public IEnumerator IndicatorShower(bool isIndicatorLeft, bool isFast)
    {
        int index = 0;
        if (!isIndicatorLeft)
        {
            index = 1;
        }

        if (isFast)
        {
            _fastIndicators[index].color = _fastIndicators[index].color.SetAlpha(0f);
            _slowIndicators[index].color = _slowIndicators[index].color.SetAlpha(0f);

            _fastIndicators[index].color = _fastIndicators[index].color.SetAlpha(1f);
            yield return new WaitForSeconds(1f);
            _fastIndicators[index].color = _fastIndicators[index].color.SetAlpha(0f);
        }
        else
        {
            _fastIndicators[index].color = _fastIndicators[index].color.SetAlpha(0f);
            _slowIndicators[index].color = _slowIndicators[index].color.SetAlpha(0f);

            _slowIndicators[index].color = _slowIndicators[index].color.SetAlpha(1f);
            yield return new WaitForSeconds(1f);
            _slowIndicators[index].color = _slowIndicators[index].color.SetAlpha(0f);
        }

        if (isIndicatorLeft)
        {
            _currentLeftIndicatorRoutine = null;
        }
        else
        {
            _currentRightIndicatorRoutine = null;
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
