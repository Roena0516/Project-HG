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
    private MusicPlayer music; 

    public NoteClass noteClass;

    private const float startY = 6f;
    private const float endY = -6f;

    private double dropStartTime;

    void Start()
    {
        isSet = false;
        isEndNote = false;
        isInputed = false;
        speed = 0;
        line = LineInputChecker.Instance;
        judgement = JudgementManager.Instance;
        noteGenerator = NoteGenerator.Instance;
        music = MusicPlayer.Instance;
    }

    public void SetNote()
    {
        dropStartTime = line.currentTime;
        speed = noteGenerator.speed;
        isSet = true;
    }

    public void MoveNote()
    {
        if (isSet)
        {
            double elapsedTime = line.currentTime - dropStartTime;
            float progress = (float)(elapsedTime * speed / (startY - endY));
            //float progress = (float)(elapsedTime * (12f / noteGenerator.speed));
            progress = Mathf.Clamp01(progress);  // 0 ~ 1 사이로 제한
            float currentY = Mathf.Lerp(startY, endY, progress);
            transform.position = new Vector2(transform.position.x, currentY);

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
            line.judgementManager.PerformAction(noteClass, "Perfect", noteClass.ms);
            line.judgementManager.AddCombo(1);
        }
    }

    void Update()
    {
        if (ShouldSetNote() && !isSet)
        {
            SetNote();
        }

        MoveNote();

        Misser();

        HoldPerformer();
    }

    private bool ShouldSetNote()
    {
        double targetTime = (ms - noteGenerator.fallTime * 1d) / 1000d;
        return line.currentTime >= targetTime;
    }
    //public void SetSpeed(float spd)
    //{
    //    speed = spd;
    //}
}
