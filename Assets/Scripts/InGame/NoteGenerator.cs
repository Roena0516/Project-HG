using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NoteGenerator : MonoBehaviour
{
    public GameObject notePrefab;
    public GameObject holdPrefab;
    public GameObject upPrefab;
    public float BPM;

    public float distance;
    public float fallTime;
    public float speed;

    private int noteCount;

    public List<GameObject> Lines;

    public Dictionary<string, int> noteTypeCounts = new Dictionary<string, int>();

    private Vector3 spawnPosition1;
    private Vector3 spawnPosition2;
    private Vector3 spawnPosition3;
    private Vector3 spawnPosition4;

    Quaternion spawnRotation;

    public LoadManager loadManager;
    public LineInputChecker checker;
    public JudgementManager judgement;
    public MusicPlayer musicPlayer;
    private SettingsManager settings;

    public List<NoteClass> notes;
    public SongInfoClass info;

    public static NoteGenerator Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        speed = 4.5f;
        distance = 9f;
        spawnPosition1 = new Vector3(Lines[0].transform.position.x, transform.position.y, 0);
        spawnPosition2 = new Vector3(Lines[1].transform.position.x, transform.position.y, 0);
        spawnPosition3 = new Vector3(Lines[2].transform.position.x, transform.position.y, 0);
        spawnPosition4 = new Vector3(Lines[3].transform.position.x, transform.position.y, 0);
        spawnRotation = Quaternion.Euler(0f, 0f, 0f);

        settings = SettingsManager.Instance;

        speed *= settings.speed;

        fallTime = distance / speed * 1000f;

        noteTypeCounts["normal"] = 0;
        noteTypeCounts["hold"] = 0;
        noteTypeCounts["up"] = 0;

        StartCoroutine(NoteGenerate());
    }

    IEnumerator NoteGenerate()
    {
        yield return new WaitForSeconds(1f);

        notes = loadManager.notes;
        info = loadManager.info;
        BPM = info.bpm;

        noteCount = notes.Count;
        Debug.Log(noteCount);
        notes[noteCount - 1].isEndNote = true;

        StartCoroutine(NoteSpawnerSpawner());

        foreach (NoteClass note in notes)
        {
            noteTypeCounts[note.type]++;
        }

        judgement.CalcRate();

        yield break;
    }

    IEnumerator NoteSpawnerSpawner()
    {
        foreach (NoteClass note in notes)
        {
            NoteSpawner(note, note.position, note.type, note.beat, spawnRotation);
            yield return new WaitForSeconds(0.03625f);
        }
    }

    public void NoteSpawner(NoteClass noteClass, int position, string type, float beat, Quaternion R)
    {
        Vector3 ranePosition = spawnPosition1;
        float oneBeatDuration;
        float beatDuration;
        GameObject note = null;
        if (position == 1)
        {
            ranePosition = spawnPosition1;
        }
        if (position == 2)
        {
            ranePosition = spawnPosition2;
        }
        if (position == 3)
        {
            ranePosition = spawnPosition3;
        }
        if (position == 4)
        {
            ranePosition = spawnPosition4;
        }
        if (type == "normal")
        {
            note = Instantiate(notePrefab, ranePosition, R);
        }
        if (type == "hold")
        {
            note = Instantiate(holdPrefab, ranePosition, R);
        }
        if (type == "up")
        {
            note = Instantiate(upPrefab, ranePosition, R);
        }

        if (noteClass.isEndNote == true)
        {
            note.GetComponent<Note>().isEndNote = true;
        }
        noteClass.noteObject = note;
        //noteClass.noteObject.GetComponent<Note>().SetSpeed(speed);
        oneBeatDuration = 60f / BPM * 1000f;
        beatDuration = oneBeatDuration * beat;

        StartCoroutine(NoteSetter(noteClass, note, beatDuration));
    }

    IEnumerator NoteSetter(NoteClass noteClass, GameObject note, float beatDuration)
    {
        Note noteScript = note.GetComponent<Note>();

        float ms = beatDuration + 3000f;

        noteClass.ms = ms;

        noteScript.noteClass = noteClass;
        noteScript.ms = ms;

        yield break;
    }
}
