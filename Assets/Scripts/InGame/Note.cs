using UnityEngine;

public class Note : MonoBehaviour
{
    private float speed;

    private bool isSet;
    public double ms;

    public bool isEndNote;

    private LineInputChecker line;
    private JudgementManager judgement;
    private NoteGenerator noteGenerator;
    private MusicPlayer music; 

    public NoteClass noteClass;

    void Start()
    {
        isSet = false;
        isEndNote = false;
        line = LineInputChecker.Instance;
        judgement = JudgementManager.Instance;
        noteGenerator = NoteGenerator.Instance;
        music = MusicPlayer.Instance;
    }

    void FixedUpdate()
    {
        if (ShouldSetNote())
        {
            SetNote();
        }

        if (isSet)
        {
            transform.Translate(Vector2.down * speed * Time.deltaTime);
            if ((line.currentTime * 1000f) - ms >= 200f)
            {
                judgement.PerformAction(noteClass, "Miss", ms);
                judgement.ClearCombo();
            }
        }
    }

    private bool ShouldSetNote()
    {
        double targetTime = (ms - noteGenerator.fallTime * 1d) / 1000d;
        return line.currentTime >= targetTime;
    }

    public void SetNote()
    {
        isSet = true;
    }

    public void SetSpeed(float spd)
    {
        speed = spd;
    }
}
