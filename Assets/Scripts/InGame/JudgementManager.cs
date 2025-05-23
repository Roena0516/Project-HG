using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class JudgementManager : MonoBehaviour
{
    private float perfectp = 25f;
    private float perfect = 40f;
    private float great = 60f;
    private float good = 110f;
    private float bad = 160f;

    public int combo;
    public float rate;

    public NoteGenerator noteGenerator;
    public GameManager gameManager;
    public SyncRoomManager syncRoomManager;
    private SettingsManager settings;
    public ParticleManager particle;

    public bool isAP;
    public bool isFC;

    public TextMeshProUGUI judgeText;
    public TextMeshProUGUI plusJudgeText;
    public TextMeshProUGUI fastSlow;
    public TextMeshProUGUI comboText;
    public TextMeshProUGUI comboTitleText;
    public TextMeshPro rateText;
    public TextMeshPro perfectpCountText;
    public TextMeshPro perfectCountText;
    public TextMeshPro greatCountText;
    public TextMeshPro goodCountText;
    public TextMeshPro missCountText;
    public TextMeshPro titleText;
    public TextMeshPro artistText;
    public TextMeshProUGUI FCAPText;
    public TextMeshProUGUI speedText;

    public List<SpriteRenderer> fastIndicators;
    public List<SpriteRenderer> slowIndicators;

    private Coroutine currentJudgementRoutine;

    public Dictionary<string, float> noteTypeRate = new Dictionary<string, float>();
    public Dictionary<string, int> judgeCount = new Dictionary<string, int>();

    public static JudgementManager Instance { get; private set; }

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
        settings = SettingsManager.Instance;

        Color tempColor = judgeText.color;
        tempColor.a = 0f;
        judgeText.color = tempColor;
        plusJudgeText.color = plusJudgeText.color.SetAlpha(0f);
        fastSlow.color = tempColor;
        rate = 100f;

        isAP = false;
        isFC = false;

        noteTypeRate["normal"] = 0f;
        noteTypeRate["hold"] = 0f;
        noteTypeRate["up"] = 0f;

        judgeCount["PerfectP"] = 0;
        judgeCount["Perfect"] = 0;
        judgeCount["Great"] = 0;
        judgeCount["Good"] = 0;
        judgeCount["Bad"] = 0;
        judgeCount["Miss"] = 0;

        speedText.text = $"{settings.settings.speed:F1}";
        titleText.text = settings.songTitle;
        artistText.text = settings.songArtist;

        StartCoroutine(IndicatorSetter());
        ClearCombo();
        UpdateJudgeCountText();
    }

    private IEnumerator IndicatorSetter()
    {
        for (int i = 0; i < 4; i++)
        {
            fastIndicators[i].color = fastIndicators[i].color.SetAlpha(0f);
            slowIndicators[i].color = slowIndicators[i].color.SetAlpha(0f);
        }

        yield break;
    }

    public void CalcRate()
    {
        float rateAllNote = (noteGenerator.noteTypeCounts["normal"] * 1) + (noteGenerator.noteTypeCounts["hold"] * 1) + (noteGenerator.noteTypeCounts["up"] * 2);

        noteTypeRate["normal"] = (noteGenerator.noteTypeCounts["normal"] > 0) ? (noteGenerator.noteTypeCounts["normal"] / rateAllNote * 100 / noteGenerator.noteTypeCounts["normal"]) : 0;
        noteTypeRate["hold"] = (noteGenerator.noteTypeCounts["hold"] > 0) ? (noteGenerator.noteTypeCounts["hold"] / rateAllNote * 100) / noteGenerator.noteTypeCounts["hold"] : 0;
        noteTypeRate["up"] = (noteGenerator.noteTypeCounts["up"] > 0) ? (noteGenerator.noteTypeCounts["up"] * 2 / rateAllNote * 100) / noteGenerator.noteTypeCounts["up"] : 0;
        //foreach (var pair in noteGenerator.noteTypeCounts)
        //{
        //    Debug.Log($"{pair.Key}: {pair.Value}");
        //}
        //foreach (var pair in noteTypeRate)
        //{
        //    Debug.Log($"{pair.Key}: {pair.Value}");
        //}
    }

    public void Judge(int raneNumber, double currentTimeMs)
    {
        var filteredNotes = noteGenerator.notes
        .Where(note => Mathf.Abs((float)(note.ms - currentTimeMs)) <= 161)
        .ToList();

        foreach (NoteClass note in filteredNotes)
        {
            if (note.type == "up" && raneNumber + 1 == note.position && !note.isInputed)
            {
                break;
            }

            float timeDifference = Mathf.Abs((float)(note.ms - currentTimeMs));

            if (timeDifference <= bad && note.type == "hold" && raneNumber + 1 == note.position && !note.isInputed)
            {
                SetHoldInputed(note);
                break;
            }
            if (timeDifference <= perfectp && note.type == "normal" && raneNumber + 1 == note.position && !note.isInputed)
            {
                PerformAction(note, "PerfectP", currentTimeMs);
                AddCombo(1);
                break;
            }
            if (timeDifference <= perfect && note.type == "normal" && raneNumber + 1 == note.position && !note.isInputed)
            {
                PerformAction(note, "Perfect", currentTimeMs);
                AddCombo(1);
                break;
            }
            if (timeDifference <= great && note.type == "normal" && raneNumber + 1 == note.position && !note.isInputed)
            {
                PerformAction(note, "Great", currentTimeMs);
                AddCombo(1);
                break;
            }
            if (timeDifference <= good && note.type == "normal" && raneNumber + 1 == note.position && !note.isInputed)
            {
                PerformAction(note, "Good", currentTimeMs);
                AddCombo(1);
                break;
            }
            if (timeDifference <= bad && note.type == "normal" && raneNumber + 1 == note.position && !note.isInputed)
            {
                PerformAction(note, "Bad", currentTimeMs);
                ClearCombo();
                break;
            }
        }
    }

    public void SetHoldInputed(NoteClass note)
    {
        note.isInputed = true;
    }

    public void UpJudge(int raneNumber, double currentTimeMs)
    {
        var filteredNotes = noteGenerator.notes
        .Where(note => Mathf.Abs((float)(note.ms - currentTimeMs)) <= 1000)
        .ToList();

        foreach (NoteClass note in filteredNotes)
        {
            float timeDifference = Mathf.Abs((float)(note.ms - currentTimeMs));
            double notAbsDiff = note.ms - currentTimeMs;

            if ((note.ms - (currentTimeMs) <= 40 && note.ms - (currentTimeMs) > 0) && note.type == "hold" && raneNumber + 1 == note.position && !note.isInputed)
            {
                note.isInputed = true;
                break;
            }

            if (timeDifference <= perfect && note.type == "up" && raneNumber + 1 == note.position && !note.isInputed)
            {
                PerformAction(note, "PerfectP", currentTimeMs);
                AddCombo(1);
                break;
            }
            if (notAbsDiff >= -80 && notAbsDiff < -40 && note.type == "up" && raneNumber + 1 == note.position && !note.isInputed)
            {
                PerformAction(note, "Perfect", currentTimeMs);
                AddCombo(1);
                break;
            }
            if (notAbsDiff >= -120 && notAbsDiff < -80 && note.type == "up" && raneNumber + 1 == note.position && !note.isInputed)
            {
                PerformAction(note, "Great", currentTimeMs);
                AddCombo(1);
                break;
            }
            if (notAbsDiff >= -200 && notAbsDiff < -120 && note.type == "up" && raneNumber + 1 == note.position && !note.isInputed)
            {
                PerformAction(note, "Good", currentTimeMs);
                AddCombo(1);
                break;
            }
            if (notAbsDiff > 40 && notAbsDiff <= 100  && note.type == "up" && raneNumber + 1 == note.position && !note.isInputed)
            {
                PerformAction(note, "Great", currentTimeMs);
                AddCombo(1);
                break;
            }
            if (notAbsDiff > 100 && notAbsDiff <= 140 && note.type == "up" && raneNumber + 1 == note.position && !note.isInputed)
            {
                PerformAction(note, "Good", currentTimeMs);
                AddCombo(1);
                break;
            }
        }
    }

    public void AddCombo(int amount)
    {
        combo += amount;
        comboText.text = $"{combo}";
        comboTitleText.color = comboTitleText.color.SetAlpha(1f);
    }

    public void ClearCombo()
    {
        combo = 0;
        comboText.text = $"";
        comboTitleText.color = comboTitleText.color.SetAlpha(0f);
    }

    public void UpdateJudgeCountText()
    {
        perfectpCountText.text = $"{judgeCount["PerfectP"]}";
        perfectCountText.text = $"{judgeCount["Perfect"]}";
        greatCountText.text = $"{judgeCount["Great"]}";
        goodCountText.text = $"{judgeCount["Good"]}";
        missCountText.text = $"{judgeCount["Miss"] + judgeCount["Bad"]}";
    }

    public void PerformAction(NoteClass note, string judgement, double currentTimeMs)
    {
        Debug.Log($"{judgement}: {note.ms}, input: {currentTimeMs}");
        note.isInputed = true;
        Destroy(note.noteObject);
        Debug.Log(noteTypeRate[note.type]);
        if (judgement == "Great")
        {
            ChangeRate(noteTypeRate[note.type], 0.25f);
        }
        if (judgement == "Good")
        {
            ChangeRate(noteTypeRate[note.type], 0.5f);
        }
        if (judgement == "Bad")
        {
            ChangeRate(noteTypeRate[note.type], 1f);
        }
        if (judgement == "Miss")
        {
            ChangeRate(noteTypeRate[note.type], 1f);
        }
        double Ms = note.ms - currentTimeMs;
        judgeCount[judgement]++;
        UpdateJudgeCountText();

        if (note.isEndNote == true)
        {
            if (judgeCount["Miss"] == 0 && judgeCount["Bad"] == 0)
            {
                FCAPText.text = "FULL COMBO";
                isFC = true;
            }
            if (judgeCount["Miss"] == 0 && judgeCount["Bad"] == 0 && judgeCount["Good"] == 0 && judgeCount["Great"] == 0)
            {
                FCAPText.text = "ALL PERFECT";
                isAP = true;
            }

            Debug.Log($"{note.ms}, {note.type}, {note.position}, {note.isEndNote}, {note.beat}");

            gameManager.isLevelEnd = true;
        }
        StartCoroutine(JudegementTextShower(judgement, Ms, note.position));

        if (judgement != "Miss")
        {
            //particle.EmitParticle(note.position - 1);
        }

        if (gameManager.isSyncRoom && judgement != "Miss")
        {
            syncRoomManager.inputConut++;
            syncRoomManager.msCount += (int)Ms;
            syncRoomManager.CalcAvg();
        }
    }

    private void ChangeRate(float typeRate, float ratio)
    {
        rate -= typeRate * ratio;
        rateText.text = $"{rate:F2}%";
    }

    IEnumerator JudegementTextShower(string judgement, double Ms, int position)
    {
        if (currentJudgementRoutine != null)
        {
            StopCoroutine(currentJudgementRoutine);
        }
        currentJudgementRoutine = StartCoroutine(ShowJudgementTextRoutine(judgement, Ms, position));
        yield break;
    }

    private IEnumerator ShowJudgementTextRoutine(string judgement, double Ms, int position)
    {
        Color tempColor = judgeText.color;
        int index = position - 1;
        tempColor.a = 1f;
        judgeText.color = tempColor;
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
        if (Ms > 0)
        {
            tempColor = fastSlow.color;
            tempColor.a = 1f;
            fastSlow.color = tempColor;
            fastSlow.text = $"+{(int)Ms}";

            if (judgement != "Perfect")
            {
                StartCoroutine(IndicatorShower(index, true));
            }           
        }
        if (Ms == 0)
        {
            tempColor = fastSlow.color;
            tempColor.a = 1f;
            fastSlow.color = tempColor;
            fastSlow.text = $"";
        }
        if (Ms < 0)
        {
            tempColor = fastSlow.color;
            tempColor.a = 1f;
            fastSlow.color = tempColor;
            fastSlow.text = $"{(int)Ms}";

            if (judgement != "Perfect")
            {
                StartCoroutine(IndicatorShower(index, false));
            }
        }

        yield return new WaitForSeconds(2f);

        tempColor = judgeText.color;
        tempColor.a = 0;
        judgeText.color = tempColor;

        plusJudgeText.color = plusJudgeText.color.SetAlpha(0f);

        tempColor = fastSlow.color;
        tempColor.a = 0;
        fastSlow.color = tempColor;
        currentJudgementRoutine = null;
    }

    private IEnumerator IndicatorShower(int index, bool isFast)
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

}
