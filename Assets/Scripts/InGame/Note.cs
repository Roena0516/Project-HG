using FMOD.Studio;
using FMODUnity;
using System.Collections;
using UnityEngine;

public class Note : MonoBehaviour
{
    private float speed;

    public bool isSet;
    public double ms;

    public bool isEndNote;
    public bool isInputed;

    private LineInputChecker line;
    private JudgementManager judgement;
    private NoteGenerator noteGenerator;

    public NoteClass noteClass;

    private const float startY = 6f;
    private const float endY = -6f;

    private double dropStartTime;

    private Coroutine moveNoteRoutine;

    // 히트 사운드
    public EventInstance eventInstance;
    public EventInstance hitSoundInstance;

    private void SetHitSoundInstance()
    {
        hitSoundInstance = RuntimeManager.CreateInstance($"event:/tamb");

        hitSoundInstance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));

        hitSoundInstance.setVolume(1f);
        //hitSoundInstance.start();
    }


    void Start()
    {
        isSet = false;
        isEndNote = false;
        isInputed = false;
        
        line = LineInputChecker.Instance;
        judgement = JudgementManager.Instance;
        noteGenerator = NoteGenerator.Instance;

        speed = noteGenerator.speed;
        dropStartTime = (ms - noteGenerator.fallTime) / 1000f;

        moveNoteRoutine = StartCoroutine(MoveNote());

        SetHitSoundInstance();
    }

    public void SetNote()
    {
        dropStartTime = line.currentTime;
        speed = noteGenerator.speed;
        isSet = true;
    }

    private void OnDestroy()
    {
        StopCoroutine(moveNoteRoutine);
    }

    public IEnumerator MoveNote()
    {
        while(true)
        {
            dropStartTime = (ms - noteGenerator.fallTime) / 1000f;
            double elapsedTime = line.currentTime - dropStartTime;
            float progress = (float)(elapsedTime * speed / (startY - endY));
            progress = Mathf.Clamp01(progress);  // 0 ~ 1 사이로 제한
            float currentY = Mathf.Lerp(startY, endY, progress);
            transform.position = new Vector2(transform.position.x, currentY);

            yield return null;
        }
    }

    private void Misser()
    {
        if ((line.currentTime * 1000f) - ms >= 200f)
        {
            judgement.PerformAction(noteClass, "Miss", ms);
            judgement.ClearCombo();
            isSet = false;
        }
    }

    private void HoldPerformer()
    {
        if (noteClass.type == "hold" && noteClass.isInputed && (noteClass.ms - (line.currentTime * 1000f) <= 0 && noteClass.ms - (line.currentTime * 1000f) >= -160))
        {
            line.judgementManager.PerformAction(noteClass, "PerfectP", noteClass.ms);
            line.judgementManager.AddCombo(1);
        }
    }

    private void AutoPlayPerformer()
    {
        if (line.isAutoPlay && !noteClass.isInputed && (noteClass.ms - (line.currentTime * 1000f) <= 0))
        {
            line.judgementManager.PerformAction(noteClass, "PerfectP", noteClass.ms);
            line.judgementManager.AddCombo(1);
            
            Debug.Log($"AutoPlay note.ms: {noteClass.ms}, currentTime: {line.currentTime * 1000f}");
            //hitSoundInstance.start();
        }
    }

    void Update()
    {
        speed = noteGenerator.speed;

        Misser();

        HoldPerformer();

        AutoPlayPerformer();
    }
}
