using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class JudgementManager : MonoBehaviour
{
    private float perfect = 40f;
    private float great = 60f;
    private float good = 120f;

    private NoteGenerator noteGenerator;

    public TextMeshProUGUI judgeText;
    public GameObject judgeObject;

    private Coroutine currentJudgementRoutine;

    [System.Obsolete]
    private void Start()
    {
        noteGenerator = FindObjectOfType<NoteGenerator>();
    }

    public void Judge(int raneNumber, float currentTimeMs)
    {
        foreach (NoteClass note in noteGenerator.notes)
        {
            float timeDifference = Mathf.Abs(note.ms - currentTimeMs);

            if (timeDifference <= perfect && raneNumber + 1 == note.position)
            {
                PerformAction(note, "Perfect", currentTimeMs);
                break;
            }
            if (timeDifference <= great && raneNumber + 1 == note.position)
            {
                PerformAction(note, "Great", currentTimeMs);
                break;
            }
            if (timeDifference <= good && raneNumber + 1 == note.position)
            {
                PerformAction(note, "Good", currentTimeMs);
                break;
            }
        }
    }
    public void PerformAction(NoteClass note, string judgement, float currentTimeMs)
    {
        Debug.Log($"{judgement}: {note.ms}, input: {currentTimeMs}");
        StartCoroutine(JudegementTextShower(judgement));
        Destroy(note.noteObject);
    }

    IEnumerator JudegementTextShower(string judgement)
    {
        if (currentJudgementRoutine != null)
        {
            StopCoroutine(currentJudgementRoutine);
        }
        currentJudgementRoutine = StartCoroutine(ShowJudgementTextRoutine(judgement));
        yield break;
    }

    private IEnumerator ShowJudgementTextRoutine(string judgement)
    {
        judgeObject.SetActive(true);
        judgeText.text = $"{judgement}";

        yield return new WaitForSeconds(2f);

        judgeObject.SetActive(false);
        currentJudgementRoutine = null;
    }

}
