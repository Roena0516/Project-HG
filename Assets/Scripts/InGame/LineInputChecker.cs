using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
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
    private InputThreadDivider divider;

    public MainInputAction action;
    private List<InputAction> LineActions;
    private InputAction speedUp;
    private InputAction speedDown;

    public List<bool> isHolding;

    public List<GameObject> buttons;

    private bool isSpeedHold;
    public bool isAutoPlay;

    private Coroutine repeatCoroutine;

    public List<float> originX;
    public float originY;

    public List<Coroutine> currentDownButtonRoutines;
    public List<Coroutine> currentUpButtonRoutines;

    private bool isEnd = false;

    public static LineInputChecker Instance { get; private set; }

    public Thread chartPlayThread;

    private readonly Queue<Action> mainThreadQueue = new Queue<Action>();
    private readonly object queueLock = new object();

    public UnityEvent OnPlay = new UnityEvent();

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
#if UNITY_STANDALONE_OSX
        for (int i = 0; i < 4; i++)
        {
            LineActions[i].Enable();
            LineActions[i].started += Started;
            LineActions[i].performed += Performed;
            LineActions[i].canceled += Canceled;
        }
#endif

        speedUp.Enable();
        speedUp.started += Started;
        speedUp.canceled += Canceled;

        speedDown.Enable();
        speedDown.started += Started;
        speedDown.canceled += Canceled;
    }

    private void OnDisable()
    {
#if UNITY_STANDALONE_OSX
        for (int i = 0; i < 4; i++)
        {
            LineActions[i].Disable();
            LineActions[i].started -= Started;
            LineActions[i].performed -= Performed;
            LineActions[i].canceled -= Canceled;
        }
#endif

        speedUp.Disable();
        speedUp.started -= Started;
        speedUp.canceled -= Canceled;

        speedDown.Disable();
        speedDown.started -= Started;
        speedDown.canceled -= Canceled;
    }

    private void OnDestroy()
    {
#if UNITY_STANDALONE_WIN
        isEnd = true;

        if (chartPlayThread != null && chartPlayThread.IsAlive)
        {
            chartPlayThread.Join(100);
        }
#endif
    }

    public void SetSpeed(float duration)
    {
        if (settings.settings.speed + duration >= 1.0 && settings.settings.speed + duration <= 10.0)
        {
            settings.SetSpeed($"{settings.settings.speed += duration}");
            noteGenerator.speed = 4.5f * settings.settings.speed;
            noteGenerator.fallTime = noteGenerator.distance / noteGenerator.speed * 1000f;
            judgementManager.speedText.text = $"{settings.settings.speed:F1}";
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
            case "SpeedUp":
                isSpeedHold = true;
                SetSpeed(0.1f);
                repeatCoroutine = StartCoroutine(RepeatKeyPress(actionName));
                break;
            case "SpeedDown":
                isSpeedHold = true;
                SetSpeed(-0.1f);
                repeatCoroutine = StartCoroutine(RepeatKeyPress(actionName));
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

        isSpeedHold = false;

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

        if (repeatCoroutine != null)
        {
            StopCoroutine(repeatCoroutine);
            repeatCoroutine = null;
        }
    }

    private void Start()
    {
        isHolding = new List<bool>();
        currentDownButtonRoutines = new List<Coroutine>();
        currentUpButtonRoutines = new List<Coroutine>();
        originX = new List<float>();
        originY = buttons[0].transform.position.y;

        for (int i = 0; i < 4; i++)
        {
            isHolding.Add(false);
            currentDownButtonRoutines.Add(null);
            currentUpButtonRoutines.Add(null);
            originX.Add(0);
            originX[i] = buttons[i].transform.position.x;
        }

        Play();
    }

    public void Play()
    {
        currentTimeMs = 0d;
        startTime = Time.time;
        isAutoPlay = settings.isAutoPlay;
        Debug.Log($"Start Time : {startTime}");

        OnPlay.Invoke();

#if UNITY_STANDALONE_WIN
        chartPlayThread = new Thread(ChartPlayWorker);
        chartPlayThread.IsBackground = true;
        chartPlayThread.Start(8000L);
        if (divider == null)
        {
            divider = InputThreadDivider.Instance;
            if (divider == null)
            {
                Debug.LogError("InputThreadDivider.Instance is null");
                return;
            }
        }
#endif
    }

#if UNITY_STANDALONE_WIN
    private void ChartPlayWorker(object param)
    {
        long frequency = (long)param;

        long interval = 10000000 / frequency;
        var stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();
        long prevTick = stopwatch.ElapsedTicks;
        long correction = 0;

        while (!isEnd)
        {
            long now = stopwatch.ElapsedTicks;
            long timeDiff = now - prevTick;

            if (timeDiff >= interval - correction)
            {
                double progress = now / 10000000d;
                currentTime = progress;

                divider.OnChartProgressAsync(progress);
                divider.OnChartProgress(progress);

                correction = timeDiff - interval;
                if (correction > interval)
                {
                    correction = interval;
                }

                prevTick = now;
            }
        }

        Debug.Log("ChartPlayWorker ?????? ????");
    }
#endif

    void Update()
    {
#if UNITY_STANDALONE_OSX
        currentTime = Time.time - startTime;
#endif
        isEnd = gameManager.isLevelEnd;

        lock (queueLock)
        {
            while (mainThreadQueue.Count > 0)
            {
                var action = mainThreadQueue.Dequeue();
                action?.Invoke();
            }
        }

        for (int i = 0; i < 4; i++)
        {
            if (isHolding[i])
                CheckHold(i);
        }
    }

    public void EnqueueMainThreadAction(Action action)
    {
        lock (queueLock)
        {
            mainThreadQueue.Enqueue(action);
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

    public void DownInput(int raneNumber, double inputTime = -1)
    {
        if (inputTime < 0)
        {
            inputTime = currentTime;
        }

        currentTimeMs = inputTime * 1000f;

        isHolding[raneNumber] = true;

        judgementManager.Judge(raneNumber, currentTimeMs);

        StartCoroutine(DownLines(raneNumber));

        if (currentDownButtonRoutines[raneNumber] != null)
        {
            StopCoroutine(currentDownButtonRoutines[raneNumber]);
        }
        currentDownButtonRoutines[raneNumber] = StartCoroutine(DownButton(raneNumber));
    }

    public void UpInput(int raneNumber, double inputTime = -1)
    {
        if (inputTime < 0)
        {
            inputTime = currentTime;
        }

        currentTimeMs = inputTime * 1000f;

        isHolding[raneNumber] = false;

        judgementManager.UpJudge(raneNumber, currentTimeMs);

        StartCoroutine(UpLines(raneNumber));

        if (currentUpButtonRoutines[raneNumber] != null)
        {
            StopCoroutine(currentUpButtonRoutines[raneNumber]);
        }
        currentUpButtonRoutines[raneNumber] = StartCoroutine(UpButton(raneNumber));
    }

    private IEnumerator DownButton(int raneNumber)
    {
        Transform T = buttons[raneNumber].transform;

        float elapsedTime = 0f;
        Vector3 startPos = new Vector3(originX[raneNumber], T.position.y, 0f);
        float duration = 0.05f;
        Vector3 targetPos = new Vector3(originX[raneNumber], originY - 0.325f, 0f);
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            float easedT = Mathf.Sin(t * Mathf.PI * 0.5f);

            T.position = Vector3.Lerp(startPos, targetPos, easedT);

            yield return null;
        }

        T.position = targetPos;

        currentDownButtonRoutines[raneNumber] = null;

        yield break;
    }

    private IEnumerator DownLines(int lineNumber)
    {
        SpriteRenderer renderer = Lines[lineNumber].GetComponent<SpriteRenderer>();

        float elapsedTime = 0f;
        float startAlpha = 0f;
        float duration = 0.01f;
        float targetAlpha = 0.05f;

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

    private IEnumerator UpButton(int raneNumber)
    {
        Transform T = buttons[raneNumber].transform;

        float elapsedTime = 0f;
        Vector3 startPos = new Vector3(originX[raneNumber], T.position.y, 0f);
        float duration = 0.05f;
        Vector3 targetPos = new Vector3(originX[raneNumber], originY, 0f);
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            float easedT = Mathf.Sin(t * Mathf.PI * 0.5f);

            T.position = Vector3.Lerp(startPos, targetPos, easedT);

            yield return null;
        }

        T.position = targetPos;

        currentUpButtonRoutines[raneNumber] = null;

        yield break;
    }

    private IEnumerator UpLines(int lineNumber)
    {
        SpriteRenderer renderer = Lines[lineNumber].GetComponent<SpriteRenderer>();

        float elapsedTime = 0f;
        float startAlpha = 0.07f;
        float duration = 0.01f;
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

    private IEnumerator RepeatKeyPress(string actionName)
    {
        yield return new WaitForSeconds(0.3f);

        while (isSpeedHold)
        {
            switch (actionName)
            {
                case "SpeedUp":
                    SetSpeed(0.1f);
                    break;
                case "SpeedDown":
                    SetSpeed(-0.1f);
                    break;
            }
            yield return new WaitForSeconds(0.05f);
        }
    }
}
