using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NoteGenerator : MonoBehaviour
{
    public GameObject notePrefab;
    public float BPM = 120f;

    public float distance;
    public float fallTime;

    public List<GameObject> Lines;

    private Vector3 spawnPosition1;
    private Vector3 spawnPosition2;
    private Vector3 spawnPosition3;
    private Vector3 spawnPosition4;

    Quaternion spawnRotation;

    private LoadManager loadManager;

    public List<NoteClass> notes;

    [System.Obsolete]
    void Start()
    {
        distance = 9f;
        spawnPosition1 = new Vector3(-3f, transform.position.y, 0);
        spawnPosition2 = new Vector3(-1.5f, transform.position.y, 0);
        spawnPosition3 = new Vector3(0f, transform.position.y, 0);
        spawnPosition4 = new Vector3(1.5f, transform.position.y, 0);
        spawnRotation = Quaternion.Euler(0f, 0f, 0f);

        fallTime = distance / notePrefab.GetComponent<Note>().speed * 1000f;

        loadManager = FindObjectOfType<LoadManager>();

        StartCoroutine(NoteGenerate());
    }

    IEnumerator NoteGenerate()
    {
        yield return new WaitForSeconds(1f);

        notes = loadManager.notes;

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
            GameObject note = Instantiate(notePrefab, ranePosition, R);
            noteClass.noteObject = note;
            oneBeatDuration = 60f / BPM * 1000f;
            beatDuration = oneBeatDuration * beat;
            StartCoroutine(NoteSetter(noteClass, note, beatDuration));
        }
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
