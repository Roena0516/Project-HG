using System.Collections.Generic;
using UnityEngine;

public class JudgementManager : MonoBehaviour
{
    private float perfect = 40f;
    private float great = 60f;
    private float good = 120f;

    private NoteGenerator noteGenerator;

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

        Destroy(note.noteObject);
    }
}
