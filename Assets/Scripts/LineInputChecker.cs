using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineInputChecker : MonoBehaviour
{
    public float currentTimeMs;
    public List<GameObject> Lines;

    private float perfect = 40f;

    private NoteGenerator noteGenerator;

    [System.Obsolete]
    private void Start()
    {
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
        if (Input.GetKeyDown(KeyCode.Semicolon))
        {
            DownInput(2);
        }
        if (Input.GetKeyDown(KeyCode.Quote))
        {
            DownInput(3);
        }

        if (Input.GetKeyUp(KeyCode.S))
        {
            currentTimeMs = Time.time * 1000f;
            StartCoroutine(UpLines(0));
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            currentTimeMs = Time.time * 1000f;
            StartCoroutine(UpLines(1));
        }
        if (Input.GetKeyUp(KeyCode.Semicolon))
        {
            currentTimeMs = Time.time * 1000f;
            StartCoroutine(UpLines(2));
        }
        if (Input.GetKeyUp(KeyCode.Quote))
        {
            currentTimeMs = Time.time * 1000f;
            StartCoroutine(UpLines(3));
        }
    }

    private void DownInput(int raneNumber)
    {
        currentTimeMs = Time.time * 1000f;
        foreach (NoteClass note in noteGenerator.notes)
        {
            float timeDifference = Mathf.Abs(note.ms - currentTimeMs);

            // 차이가 timeThreshold 이하일 때 작동
            if (timeDifference <= perfect && raneNumber + 1 == note.position)
            {
                // 노트와 입력이 일치한 것으로 처리 (예: 노트 판정)
                PerformAction(note);
                break;
            }
        }

        StartCoroutine(DownLines(raneNumber));
    }

    void PerformAction(NoteClass note)
    {
        Debug.Log($"Perfect: {note.ms}, input: {currentTimeMs}");

        Destroy(note.noteObject);
    }

    private IEnumerator DownLines(int lineNumber)
    {
        SpriteRenderer renderer = Lines[lineNumber].GetComponent<SpriteRenderer>();

        float elapsedTime = 0f;
        float startAlpha = 0f;
        float duration = 0.0625f;
        float targetAlpha = 0.3f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / duration);

            // 변경된 투명도 적용
            renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, alpha);

            yield return null;
        }

        // 마지막 상태 보정
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

            // 변경된 투명도 적용
            renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, alpha);

            yield return null;
        }

        // 마지막 상태 보정
        renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, targetAlpha);

        yield break;
    }
}
