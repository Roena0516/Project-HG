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

    public JudgementManager judgementManager;
    public NoteGenerator noteGenerator;
    public GameManager gameManager;
    private SettingsManager settings;

    public MainInputAction action;
    private List<InputAction> LineActions;
    private InputAction speedUp;
    private InputAction speedDown;

    public List<bool> isHolding;

    public static LineInputChecker Instance { get; private set; }

    private void Awake()
    {
        action = new MainInputAction();
        speedUp = action.Player.SpeedUp;
        speedDown = action.Player.SpeedDown;

        settings = SettingsManager.Instance;

        LineActions = settings.LineActions.ToList();

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
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

        speedUp.Enable();
        speedUp.started += Started;

        speedDown.Enable();
        speedDown.started += Started;
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

        speedUp.Disable();
        speedUp.started -= Started;

        speedDown.Disable();
        speedDown.started -= Started;
    }

    public void SetSpeed(float duration)
    {
        settings.speed += duration;
        noteGenerator.speed = 4.5f * settings.speed;
        noteGenerator.fallTime = noteGenerator.distance / noteGenerator.speed * 1000f;
        Debug.Log("Speed is setted to " + settings.speed);
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
            case "SpeedUp":
                SetSpeed(0.1f);
                break;
            case "SpeedDown":
                SetSpeed(-0.1f);
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

    private void Start()
    {
        currentTimeMs = 0d;
        startTime = Time.time;
        Debug.Log($"Start Time : {startTime}");

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
            if (note.type == "hold" && raneNumber + 1 == note.position && !note.isInputed && (note.ms - (currentTime * 1000f) <= 0 && note.ms - (currentTime * 1000f) >= -160))
            {
                note.isInputed = true;
                break;
            }
        }
    }

    //private void PerformHold()
    //{
    //    var filteredNotes = noteGenerator.notes
    //    .Where(note => Mathf.Abs((float)(note.ms - (currentTime * 1000))) <= 160)
    //    .ToList();

    //    foreach (NoteClass note in filteredNotes)
    //    {
    //        if (note.type == "hold" && note.isInputed && (note.ms - (currentTime * 1000f) <= 0 && note.ms - (currentTime * 1000f) >= -160))
    //        {
    //            judgementManager.PerformAction(note, "Perfect", note.ms);
    //            judgementManager.AddCombo(1);
    //            break;
    //        }
    //    }
    //}

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
