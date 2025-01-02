using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class JudgementManager : MonoBehaviour
{
    private float perfect = 40f;
    private float great = 60f;
    private float good = 110f;
    private float bad = 160f;

    public int combo;
    public float rate;

    private NoteGenerator noteGenerator;

    public TextMeshProUGUI judgeText;
    public TextMeshProUGUI fastSlow;
    public TextMeshProUGUI comboText;
    public TextMeshProUGUI rateText;
    public TextMeshProUGUI judgeCountText;

    private Coroutine currentJudgementRoutine;

    public Dictionary<string, float> noteTypeRate = new Dictionary<string, float>();
    public Dictionary<string, int> judgeCount = new Dictionary<string, int>();

    [System.Obsolete]
    private void Start()
    {
        Color tempColor = judgeText.color;
        tempColor.a = 0f;
        judgeText.color = tempColor;
        fastSlow.color = tempColor;
        noteGenerator = FindObjectOfType<NoteGenerator>();
        rate = 100f;

        noteTypeRate["normal"] = 0f;
        noteTypeRate["hold"] = 0f;
        noteTypeRate["up"] = 0f;

        judgeCount["Perfect"] = 0;
        judgeCount["Great"] = 0;
        judgeCount["Good"] = 0;
        judgeCount["Bad"] = 0;
        judgeCount["Miss"] = 0;

        ClearCombo();
        UpdateJudgeCountText();
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

    public void Judge(int raneNumber, float currentTimeMs)
    {
        var filteredNotes = noteGenerator.notes
        .Where(note => Mathf.Abs(note.ms - currentTimeMs) <= 1000)
        .ToList();

        foreach (NoteClass note in filteredNotes)
        {
            float timeDifference = Mathf.Abs(note.ms - currentTimeMs);

            if (timeDifference <= bad && note.type == "hold" && raneNumber + 1 == note.position && !note.isInputed)
            {
                PerformAction(note, "Perfect", note.ms);
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

    public void UpJudge(int raneNumber, float currentTimeMs)
    {
        var filteredNotes = noteGenerator.notes
        .Where(note => Mathf.Abs(note.ms - currentTimeMs) <= 1000)
        .ToList();

        foreach (NoteClass note in filteredNotes)
        {
            float timeDifference = Mathf.Abs(note.ms - currentTimeMs);
            float notAbsDiff = note.ms - currentTimeMs;

            if (timeDifference <= perfect && note.type == "up" && raneNumber + 1 == note.position && !note.isInputed)
            {
                PerformAction(note, "Perfect", currentTimeMs);
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
    }

    public void ClearCombo()
    {
        combo = 0;
        comboText.text = $"";
    }

    public void UpdateJudgeCountText()
    {
        judgeCountText.text = $"{judgeCount["Miss"]}/{judgeCount["Bad"]}/{judgeCount["Good"]}/{judgeCount["Great"]}/{judgeCount["Perfect"]}";
    }

    public void PerformAction(NoteClass note, string judgement, float currentTimeMs)
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
            ChangeRate(noteTypeRate[note.type], 0.75f);
        }
        if (judgement == "Miss")
        {
            ChangeRate(noteTypeRate[note.type], 1f);
        }
        float Ms = note.ms - currentTimeMs;
        judgeCount[judgement]++;
        UpdateJudgeCountText();
        StartCoroutine(JudegementTextShower(judgement, Ms));
    }

    private void ChangeRate(float typeRate, float ratio)
    {
        rate -= typeRate * ratio;
        rateText.text = $"{rate:F2}%";
    }

    IEnumerator JudegementTextShower(string judgement, float Ms)
    {
        if (currentJudgementRoutine != null)
        {
            StopCoroutine(currentJudgementRoutine);
        }
        currentJudgementRoutine = StartCoroutine(ShowJudgementTextRoutine(judgement, Ms));
        yield break;
    }

    private IEnumerator ShowJudgementTextRoutine(string judgement, float Ms)
    {
        Color tempColor = judgeText.color;
        tempColor.a = 1f;
        judgeText.color = tempColor;
        judgeText.text = $"{judgement}";
        if (Ms > 0)
        {
            fastSlow.color = tempColor;
            fastSlow.text = $"+{(int)Ms}";
        }
        if (Ms == 0)
        {
            fastSlow.color = tempColor;
            fastSlow.text = $"";
        }
        if (Ms < 0)
        {
            fastSlow.color = tempColor;
            fastSlow.text = $"{(int)Ms}";
        }

        yield return new WaitForSeconds(2f);

        tempColor = judgeText.color;
        tempColor.a = 0;
        judgeText.color = tempColor;
        fastSlow.color = tempColor;
        currentJudgementRoutine = null;
    }

}
