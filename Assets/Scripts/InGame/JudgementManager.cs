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
    [SerializeField] private UIManager UIManager;

    public bool isAP;
    public bool isFC;

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

        StartCoroutine(UIManager.IndicatorSetter());
        ClearCombo();
        UIManager.UpdateJudgeCountText(judgeCount);
    }

    public void CalcRate()
    {
        float rateAllNote = (noteGenerator.noteTypeCounts["normal"] * 1) + (noteGenerator.noteTypeCounts["hold"] * 1) + (noteGenerator.noteTypeCounts["up"] * 2);

        noteTypeRate["normal"] = (noteGenerator.noteTypeCounts["normal"] > 0) ? (noteGenerator.noteTypeCounts["normal"] / rateAllNote * 100 / noteGenerator.noteTypeCounts["normal"]) : 0;
        noteTypeRate["hold"] = (noteGenerator.noteTypeCounts["hold"] > 0) ? (noteGenerator.noteTypeCounts["hold"] / rateAllNote * 100) / noteGenerator.noteTypeCounts["hold"] : 0;
        noteTypeRate["up"] = (noteGenerator.noteTypeCounts["up"] > 0) ? (noteGenerator.noteTypeCounts["up"] * 2 / rateAllNote * 100) / noteGenerator.noteTypeCounts["up"] : 0;
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
        UIManager.SetCombo(combo);
    }

    public void ClearCombo()
    {
        combo = 0;
        UIManager.ClearCombo();
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
        UIManager.UpdateJudgeCountText(judgeCount);

        if (note.isEndNote == true)
        {
            if (judgeCount["Miss"] == 0 && judgeCount["Bad"] == 0)
            {
                UIManager.SetFCAPText("FULL COMBO");
                isFC = true;
            }
            if (judgeCount["Miss"] == 0 && judgeCount["Bad"] == 0 && judgeCount["Good"] == 0 && judgeCount["Great"] == 0)
            {
                UIManager.SetFCAPText("ALL PERFECT");
                isAP = true;
            }

            Debug.Log($"{note.ms}, {note.type}, {note.position}, {note.isEndNote}, {note.beat}");

            gameManager.isLevelEnd = true;
        }
        StartCoroutine(UIManager.JudgementTextShower(judgement, Ms, note.position));

        //if (judgement != "Miss")
        //{
        //    particle.EmitParticle(note.position - 1);
        //}

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
        UIManager.ChangeRate(rate);
    }

}
