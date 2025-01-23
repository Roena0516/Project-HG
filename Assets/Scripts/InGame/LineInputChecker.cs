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

    public MainInputAction action;
    private InputAction Line1Action;
    private InputAction Line2Action;
    private InputAction Line3Action;
    private InputAction Line4Action;

    private void Awake()
    {
        action = new MainInputAction();
        Line1Action = action.Player.Line1Action;
        Line2Action = action.Player.Line2Action;
        Line3Action = action.Player.Line3Action;
        Line4Action = action.Player.Line4Action;
    }

    private void OnEnable()
    {
        Line1Action.Enable();
        Line1Action.started += Started;
        Line1Action.performed += Performed;
        Line1Action.canceled += Canceled;

        Line2Action.Enable();
        Line2Action.started += Started;
        Line2Action.performed += Performed;
        Line2Action.canceled += Canceled;

        Line3Action.Enable();
        Line3Action.started += Started;
        Line3Action.performed += Performed;
        Line3Action.canceled += Canceled;

        Line4Action.Enable();
        Line4Action.started += Started;
        Line4Action.performed += Performed;
        Line4Action.canceled += Canceled;
    }

    private void OnDisable()
    {
        Line1Action.Disable();
        Line1Action.started -= Started;
        Line1Action.performed -= Performed;
        Line1Action.canceled -= Canceled;

        Line2Action.Disable();
        Line2Action.started -= Started;
        Line2Action.performed -= Performed;
        Line2Action.canceled -= Canceled;

        Line3Action.Disable();
        Line3Action.started -= Started;
        Line3Action.performed -= Performed;
        Line3Action.canceled -= Canceled;

        Line4Action.Disable();
        Line4Action.started -= Started;
        Line4Action.performed -= Performed;
        Line4Action.canceled -= Canceled;
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
    }

    void Update()
    {
        currentTime = Time.time - startTime;

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
        judgementManager.Judge(raneNumber, currentTimeMs);

        StartCoroutine(DownLines(raneNumber));
    }
    private void UpInput(int raneNumber)
    {
        currentTimeMs = currentTime * 1000f;
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
