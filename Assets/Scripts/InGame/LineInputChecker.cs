using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class LineInputChecker : MonoBehaviour
{
    public double currentTimeMs;
    public double startTime;
    public double currentTime;
    public List<GameObject> Lines;

    private JudgementManager judgementManager;
    private NoteGenerator noteGenerator;
    private GameManager gameManager;
    private SettingsManager settings;

    public MainInputAction action;

    private List<InputAction> LineActions;

    public List<bool> isHolding;

    [System.Obsolete]
    private void Awake()
    {
        action = new MainInputAction();
        settings = FindObjectOfType<SettingsManager>();

        LineActions = settings.LineActions.ToList();
    }

    private void OnEnable()
    {
        for (int i = 0; i < 4; i++)
        {
            LineActions[i].Enable();
            LineActions[i].started += Started;
            LineActions[i].performed += Performed;
            LineActions[i].canceled += Canceled;
        }
    }

    private void OnDisable()
    {
        for (int i = 0; i < 4; i++)
        {
            LineActions[i].Disable();
            LineActions[i].started -= Started;
            LineActions[i].performed -= Performed;
            LineActions[i].canceled -= Canceled;
        }
    }

    void Started(InputAction.CallbackContext context)
    {
        string pressed = context.control.name;
        string actionName = context.action.name;

        Debug.Log($"Start {pressed} {actionName}");

        switch (actionName)
        {
            case "Line1Action":
                DownInput(0);
                break;
            case "Line2Action":
                DownInput(1);
                break;
            case "Line3Action":
                DownInput(2);
                break;
            case "Line4Action":
                DownInput(3);
                break;
        }
    }

    void Performed(InputAction.CallbackContext context)
    {
        string pressed = context.control.name;
        string actionName = context.action.name;

        Debug.Log($"Perform {pressed} {actionName}");
    }

    void Canceled(InputAction.CallbackContext context)
    {
        string pressed = context.control.name;
        string actionName = context.action.name;

        Debug.Log($"Cancel {pressed} {actionName}");

        switch (actionName)
        {
            case "Line1Action":
                UpInput(0);
                break;
            case "Line2Action":
                UpInput(1);
                break;
            case "Line3Action":
                UpInput(2);
                break;
            case "Line4Action":
                UpInput(3);
                break;
        }
    }

    [System.Obsolete]
    private void Start()
    {
        currentTimeMs = 0d;
        startTime = Time.time;
        Debug.Log($"Start Time : {startTime}");
        judgementManager = GetComponent<JudgementManager>();
        noteGenerator = FindObjectOfType<NoteGenerator>();
        gameManager = FindObjectOfType<GameManager>();

        for (int i = 0; i < 4; i++)
        {
            isHolding.Add(false);
        }
    }

    void Update()
    {
        currentTime = Time.time - startTime;

        if (isHolding[0])
        {
            CheckHold(0);
        }
        if (isHolding[1])
        {
            CheckHold(1);
        }
        if (isHolding[2])
        {
            CheckHold(2);
        }
        if (isHolding[3])
        {
            CheckHold(3);
        }
    }

    private void CheckHold(int raneNumber)
    {
        var filteredNotes = noteGenerator.notes
        .Where(note => Mathf.Abs((float)(note.ms - (currentTime * 1000))) <= 40)
        .ToList();

        foreach (NoteClass note in filteredNotes)
        {
            if (note.type == "hold" && raneNumber + 1 == note.position && !note.isInputed && (note.ms - (currentTime * 1000f) <= 0 && note.ms - (currentTime * 1000f) >= -120))
            {
                judgementManager.PerformAction(note, "Perfect", note.ms);
                judgementManager.AddCombo(1);
                break;
            }
        }
    }

    private void DownInput(int raneNumber)
    {
        currentTimeMs = currentTime * 1000f;

        isHolding[raneNumber] = true;

        judgementManager.Judge(raneNumber, currentTimeMs);

        StartCoroutine(DownLines(raneNumber));
    }
    private void UpInput(int raneNumber)
    {
        currentTimeMs = currentTime * 1000f;

        isHolding[raneNumber] = false;

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
