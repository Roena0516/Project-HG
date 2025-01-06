using UnityEngine;

public class Note : MonoBehaviour
{
    private float speed;

    private bool isSet;
    public float ms;

    private LineInputChecker line;
    private JudgementManager judgement;
    private NoteGenerator noteGenerator;

    public NoteClass noteClass;

    [System.Obsolete]
    void Start()
    {
        isSet = false;
        line = FindObjectOfType<LineInputChecker>();
        judgement = FindObjectOfType<JudgementManager>();
        noteGenerator = FindObjectOfType<NoteGenerator>();
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
        float targetTime = (ms - noteGenerator.fallTime * 1) / 1000f;
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
