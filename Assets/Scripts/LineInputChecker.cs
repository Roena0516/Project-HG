using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineInputChecker : MonoBehaviour
{
    public float currentTimeMs;
    public List<GameObject> Lines;

    private JudgementManager judgementManager;

    [System.Obsolete]
    private void Start()
    {
        judgementManager = GetComponent<JudgementManager>();
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
        if (Input.GetKeyUp(KeyCode.L))
        {
            currentTimeMs = Time.time * 1000f;
            StartCoroutine(UpLines(2));
        }
        if (Input.GetKeyUp(KeyCode.Semicolon))
        {
            currentTimeMs = Time.time * 1000f;
            StartCoroutine(UpLines(3));
        }
    }

    private void DownInput(int raneNumber)
    {
        currentTimeMs = Time.time * 1000f;
        judgementManager.Judge(raneNumber, currentTimeMs);

        StartCoroutine(DownLines(raneNumber));
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
