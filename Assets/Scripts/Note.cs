using UnityEngine;

public class Note : MonoBehaviour
{
    public float speed = 15f;

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
            if ((Time.time * 1000f) - ms >= 200f)
            {
                judgement.PerformAction(noteClass, "Miss", ms);
            }
        }
    }

    public void SetNote()
    {
        isSet = !isSet;
    }
}
