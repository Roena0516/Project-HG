using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

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
    public GameManager gameManager;
    private SettingsManager settings;
    private LevelEditer levelEditor;

    public List<NoteClass> notes;
    public SongInfoClass info;

    public List<int> randomRane;

    public GameObject notesFolder;

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

        randomRane.Add(1);
        randomRane.Add(2);
        randomRane.Add(3);
        randomRane.Add(4);
    }

    private void Start()
    {
        speed = 4.5f;
        distance = 6f - (-2.63f);
        spawnPosition1 = new Vector3(Lines[0].transform.position.x, transform.position.y, 0);
        spawnPosition2 = new Vector3(Lines[1].transform.position.x, transform.position.y, 0);
        spawnPosition3 = new Vector3(Lines[2].transform.position.x, transform.position.y, 0);
        spawnPosition4 = new Vector3(Lines[3].transform.position.x, transform.position.y, 0);
        spawnRotation = Quaternion.Euler(0f, 0f, 0f);

        settings = SettingsManager.Instance;

        speed *= settings.settings.speed;

        fallTime = distance / speed * 1000f;

        noteTypeCounts["normal"] = 0;
        noteTypeCounts["hold"] = 0;
        noteTypeCounts["up"] = 0;

        if (settings.settings.effectOption == "Random")
        {
            randomRane.Shuffle();
        }
        if (settings.settings.effectOption == "Half Random")
        {
            randomRane.ShuffleBySplit(2);
        }
        if (settings.settings.effectOption == "L. Quater Random")
        {
            randomRane.ShuffleBySplit(1);
        }
        if (settings.settings.effectOption == "R. Quater Random")
        {
            randomRane.ShuffleBySplit(3);
        }

        StartCoroutine(NoteGenerate());
    }

    private IEnumerator NoteGenerate()
    {
        yield return new WaitForSeconds(1f);

        notes = loadManager.notes;
        info = loadManager.info;
        if (!gameManager.isTest)
        {
            BPM = info.bpm;
        }
        else
        {
            levelEditor = LevelEditer.Instance;
            BPM = levelEditor.BPM;
        }

        noteCount = notes.Count;
        Debug.Log($"Count : {noteCount}");
        

        if (!gameManager.isTest)
        {
            StartCoroutine(NoteSpawnerSpawner());
        }
        else
        {
            StartCoroutine(TestNoteSpawnerSpawner());
        }



        foreach (NoteClass note in notes)
        {
            noteTypeCounts[note.type]++;
            note.isEndNote = false;
        }

        notes[noteCount - 1].isEndNote = true;
        Debug.Log($"isEndNote is {notes[noteCount - 1].beat}");

        judgement.CalcRate();

        yield break;
    }

    private IEnumerator NoteSpawnerSpawner()
    {
        foreach (NoteClass note in notes)
        {
            //Debug.Log(randomRane[note.position - 1]);
            note.position = randomRane[note.position - 1];
            NoteSpawner(note, note.position, note.type, note.beat, spawnRotation);
            yield return new WaitForSeconds(0.03625f);
        }
        yield break;
    }

    private IEnumerator TestNoteSpawnerSpawner()
    {
        foreach (NoteClass note in notes)
        {
            NoteSpawner(note, note.position, note.type, note.beat, spawnRotation);
        }
        yield break;
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
            note = Instantiate(notePrefab, ranePosition, R, notesFolder.transform);
        }
        if (type == "hold")
        {
            note = Instantiate(holdPrefab, ranePosition, R, notesFolder.transform);
        }
        if (type == "up")
        {
            note = Instantiate(upPrefab, ranePosition, R, notesFolder.transform);
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

        float ms = beatDuration;

        if (gameManager.isTest)
        {
            levelEditor = LevelEditer.Instance;
            ms -= levelEditor.currentMusicTime;
        }

        noteClass.ms = ms;

        noteScript.noteClass = noteClass;
        noteScript.ms = ms;

        yield break;
    }
}
