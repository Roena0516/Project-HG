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
    public float speed = 12f;

    public List<GameObject> Lines;

    private Vector3 spawnPosition1;
    private Vector3 spawnPosition2;
    private Vector3 spawnPosition3;
    private Vector3 spawnPosition4;

    Quaternion spawnRotation;

    private LoadManager loadManager;

    public List<NoteClass> notes;
    public SongInfoClass info;

    [System.Obsolete]
    void Start()
    {
        distance = 9f;
        spawnPosition1 = new Vector3(Lines[0].transform.position.x, transform.position.y, 0);
        spawnPosition2 = new Vector3(Lines[1].transform.position.x, transform.position.y, 0);
        spawnPosition3 = new Vector3(Lines[2].transform.position.x, transform.position.y, 0);
        spawnPosition4 = new Vector3(Lines[3].transform.position.x, transform.position.y, 0);
        spawnRotation = Quaternion.Euler(0f, 0f, 0f);

        fallTime = distance / speed * 1000f;

        loadManager = FindObjectOfType<LoadManager>();

        StartCoroutine(NoteGenerate());
    }

    IEnumerator NoteGenerate()
    {
        yield return new WaitForSeconds(1f);

        notes = loadManager.notes;
        info = loadManager.info;
        BPM = info.bpm;

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
        noteClass.noteObject = note;
        noteClass.noteObject.GetComponent<Note>().SetSpeed(speed);
        oneBeatDuration = 60f / BPM * 1000f;
        beatDuration = oneBeatDuration * beat;
        StartCoroutine(NoteSetter(noteClass, note, beatDuration));
    }

    IEnumerator NoteSetter(NoteClass noteClass, GameObject note, float beatDuration)
    {
        Note noteScript = note.GetComponent<Note>();

        float ms = beatDuration + fallTime + 1000f;

        noteClass.ms = ms;

        yield return new WaitForSeconds(beatDuration / 1000f);

        noteScript.SetNote();
        noteScript.noteClass = noteClass;
        noteScript.ms = ms;
    }
}
