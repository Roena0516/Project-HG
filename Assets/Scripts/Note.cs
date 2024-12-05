using UnityEngine;

public class Note : MonoBehaviour
{
    private float speed;

    private bool isSet;
    public float ms;

    private LineInputChecker line;
    private JudgementManager judgement;
    public NoteClass noteClass;

    [System.Obsolete]
    void Start()
    {
        isSet = false;
        line = FindObjectOfType<LineInputChecker>();
        judgement = FindObjectOfType<JudgementManager>();
    }

    void FixedUpdate()
    {
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

    public void SetNote()
    {
        isSet = !isSet;
    }

    public void SetSpeed(float spd)
    {
        speed = spd;
    }
}
