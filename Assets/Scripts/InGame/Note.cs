using UnityEngine;

public class Note : MonoBehaviour
{
    private float speed;

    private bool isSet;
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
        line = LineInputChecker.Instance;
        judgement = JudgementManager.Instance;
        noteGenerator = NoteGenerator.Instance;
        music = MusicPlayer.Instance;
    }

    void Update()
    {
        if (ShouldSetNote() && !isSet)
        {
            SetNote();
        }

        if (isSet)
        {
            double elapsedTime = line.currentTime - dropStartTime;
            float progress = (float)(elapsedTime * noteGenerator.speed / (startY - endY));
            progress = Mathf.Clamp01(progress);  // 0 ~ 1 사이로 제한
            float currentY = Mathf.Lerp(startY, endY, progress);
            transform.position = new Vector2(transform.position.x, currentY);

            if ((line.currentTime * 1000f) - ms >= 200f)
            {
                judgement.PerformAction(noteClass, "Miss", ms);
                judgement.ClearCombo();
                isSet = false;
            }
        }

        if (noteClass.type == "hold" && noteClass.isInputed && (noteClass.ms - (line.currentTime * 1000f) <= 0 && noteClass.ms - (line.currentTime * 1000f) >= -160))
        {
            line.judgementManager.PerformAction(noteClass, "Perfect", noteClass.ms);
            line.judgementManager.AddCombo(1);
        }
    }

    private bool ShouldSetNote()
    {
        double targetTime = (ms - noteGenerator.fallTime * 1d) / 1000d;
        return line.currentTime >= targetTime;
    }

    public void SetNote()
    {
        dropStartTime = line.currentTime;
        isSet = true;
    }

    //public void SetSpeed(float spd)
    //{
    //    speed = spd;
    //}
}
