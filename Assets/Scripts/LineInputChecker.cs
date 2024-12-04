using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LineInputChecker : MonoBehaviour
{
    public float currentTimeMs;
    public List<GameObject> Lines;

    private JudgementManager judgementManager;
    private NoteGenerator noteGenerator;

    [System.Obsolete]
    private void Start()
    {
        currentTimeMs = 0f;
        judgementManager = GetComponent<JudgementManager>();
        noteGenerator = FindObjectOfType<NoteGenerator>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            DownInput(0);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            DownInput(1);
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            DownInput(2);
        }
        if (Input.GetKeyDown(KeyCode.Semicolon))
        {
            DownInput(3);
        }


        if (Input.GetKey(KeyCode.S))
        {
            CheckHold(0);
        }
        if (Input.GetKey(KeyCode.D))
        {
            CheckHold(1);
        }
        if (Input.GetKey(KeyCode.L))
        {
            CheckHold(2);
        }
        if (Input.GetKey(KeyCode.Semicolon))
        {
            CheckHold(3);
        }



        if (Input.GetKeyUp(KeyCode.S))
        {
            UpInput(0);
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            UpInput(1);
        }
        if (Input.GetKeyUp(KeyCode.L))
        {
            UpInput(2);
        }
        if (Input.GetKeyUp(KeyCode.Semicolon))
        {
            UpInput(3);
        }
    }

    private void CheckHold(int raneNumber)
    {
        var filteredNotes = noteGenerator.notes
        .Where(note => Mathf.Abs(note.ms - (Time.time * 1000)) <= 40)
        .ToList();

        foreach (NoteClass note in filteredNotes)
        {
            if (note.type == "hold" && raneNumber + 1 == note.position && !note.isInputed && (note.ms - (Time.time * 1000) <= 3 && note.ms - (Time.time * 1000) >= -120))
            {
                judgementManager.PerformAction(note, "Perfect", note.ms);
                judgementManager.AddCombo(1);
                break;
            }
        }
    }

    private void DownInput(int raneNumber)
    {
        currentTimeMs = Time.time * 1000f;
        judgementManager.Judge(raneNumber, currentTimeMs);

        StartCoroutine(DownLines(raneNumber));
    }
    private void UpInput(int raneNumber)
    {
        currentTimeMs = Time.time * 1000f;
        judgementManager.UpJudge(raneNumber, currentTimeMs);

        StartCoroutine(UpLines(raneNumber));
    }

    private IEnumerator DownLines(int lineNumber)
    {
        SpriteRenderer renderer = Lines[lineNumber].GetComponent<SpriteRenderer>();

        float elapsedTime = 0f;
        float startAlpha = 0f;
        float duration = 0.0625f;
        float targetAlpha = 0.2f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / duration);

            renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, alpha);

            yield return null;
        }

        renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, targetAlpha);

        yield break;
    }

    private IEnumerator UpLines(int lineNumber)
    {
        SpriteRenderer renderer = Lines[lineNumber].GetComponent<SpriteRenderer>();

        float elapsedTime = 0f;
        float startAlpha = 0.5f;
        float duration = 0.0625f;
        float targetAlpha = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / duration);

            renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, alpha);

            yield return null;
        }

        renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, targetAlpha);

        yield break;
    }
}
