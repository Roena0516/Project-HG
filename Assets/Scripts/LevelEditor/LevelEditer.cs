using UnityEngine;
using System.IO;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using System.Runtime.InteropServices;
using System;
using FMODUnity;
using FMOD.Studio;
using System.Collections;
using UnityEngine.SceneManagement;
//using SFB;

public class LevelEditer : MonoBehaviour
{
    public int currentMusicTime = 0;

    public EventInstance eventInstance;

    public SaveManager saveManager;

    public GameObject normalPrefab;
    public GameObject holdPrefab;
    public GameObject upPrefab;

    public GameObject notesFolder;
    public GameObject gridFolder;

    public GameObject addIndicator;
    public GameObject removeIndicator;

    public GameObject settingsPanel;

    public RectTransform rectTransform;
    public Canvas canvasComponent;

    public string selectedBeat;

    private Coroutine currentMoveSliderer;

    public int madi;
    public int madi2;
    public int madi3;

    private bool isRemoving;
    public bool isMusicPlaying;

    private float scrollSpeed;

    public float BPM;
    public string artist;
    public string title;
    public string fileName;
    public string eventName;
    public float level;
    public string difficulty;

    //private string[] paths;

    public string noteType;

    private GameObject beat13;
    private GameObject beat14;
    private GameObject beat16;
    private GameObject beat18;
    private GameObject beat112;
    private GameObject beat116;
    private GameObject beat124;
    private GameObject beat132;
    private GameObject beat148;
    private GameObject beat164;

    public GameObject canvas;

    public TMP_Dropdown dropdown;

    public List<GameObject> beats13;
    public List<GameObject> beats14;
    public List<GameObject> beats16;
    public List<GameObject> beats18;
    public List<GameObject> beats112;
    public List<GameObject> beats116;
    public List<GameObject> beats124;
    public List<GameObject> beats132;

    public GameObject gridPrefab;
    public Transform gridContainer;
    public Camera uiCamera;
    public int maxVisibleRows = 40;
    public float cellHeight = 160f; // 각 beat prefab의 높이

    private List<GameObject> gridPool = new();
    private float totalHeight;
    public float scrollY;
    private int currentTopIndex = -1;

    private Dictionary<string, GameObject> beatPrefabMap;


    public static LevelEditer Instance { get; private set; }

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

        beat13 = Resources.Load<GameObject>("Prefabs/LevelEditor/Beats/Beat13");
        beat14 = Resources.Load<GameObject>("Prefabs/LevelEditor/Beats/Beat14");
        beat16 = Resources.Load<GameObject>("Prefabs/LevelEditor/Beats/Beat16");
        beat18 = Resources.Load<GameObject>("Prefabs/LevelEditor/Beats/Beat18");
        beat112 = Resources.Load<GameObject>("Prefabs/LevelEditor/Beats/Beat112");
        beat116 = Resources.Load<GameObject>("Prefabs/LevelEditor/Beats/Beat116");
        beat124 = Resources.Load<GameObject>("Prefabs/LevelEditor/Beats/Beat124");
        beat132 = Resources.Load<GameObject>("Prefabs/LevelEditor/Beats/Beat132");
        beat148 = Resources.Load<GameObject>("Prefabs/LevelEditor/Beats/Beat148");
        beat164 = Resources.Load<GameObject>("Prefabs/LevelEditor/Beats/Beat164");

        beatPrefabMap = new Dictionary<string, GameObject>
        {
            { "1_3", beat13 },
            { "1_4", beat14 },
            { "1_6", beat16 },
            { "1_8", beat18 },
            { "1_12", beat112 },
            { "1_16", beat116 },
            { "1_24", beat124 },
            { "1_32", beat132 },
            { "1_48", beat148 },
            { "1_64", beat164 }
        };
    }

    private void Start()
    {
        selectedBeat = "1_4";
        cellHeight = 160f;

        CreateGridPool(4);
        totalHeight = 9217 * cellHeight;

        dropdown.onValueChanged.AddListener(OnDropdownValueChanged);

        noteType = "Normal";

        isRemoving = false;
        isMusicPlaying = false;

        scrollSpeed = 10f;

        BPM = 120f;

        madi = 192;
        madi2 = 288;
        madi3 = madi + madi2;
    }

    private void ButtonClickHandler(int position, int beat, Transform buttonT)
    {
        canvas.transform.localScale = Vector3.one;

        float realBeat = 0f;

        int index = currentTopIndex + gridPool.IndexOf(buttonT.gameObject);

        if (index != -1)
        {
            realBeat = (2f / (float)beat) * (index);
        }

        Debug.Log($"Position : {position}, Beat : {realBeat} 1/{beat}, Type : {noteType}");

        if (isRemoving)
        {
            saveManager.notes.Remove(saveManager.notes.Find(note => note.beat == realBeat && note.position == position));
            foreach (Transform child in notesFolder.transform)
            {
                LevelEditerNoteManager noteManager = child.GetComponent<LevelEditerNoteManager>();
                if (noteManager.noteClass.beat == realBeat && noteManager.noteClass.position == position)
                {
                    Destroy(child.gameObject);
                }
            }
            return;
        }

        float positionX = 0f;
        if (position == 1)
        {
            positionX = -158f;
        }
        if (position == 2)
        {
            positionX = 158f / 3f * -1f;
        }
        if (position == 3f)
        {
            positionX = 158f / 3f;
        }
        if (position == 4f)
        {
            positionX = 158f;
        }

        float positionY = buttonT.position.y;

        //Debug.Log(positionY);

        GameObject prefab = noteType switch
        {
            "Normal" => normalPrefab,
            "Hold" => holdPrefab,
            "Up" => upPrefab,
            _ => null
        };

        GameObject instantiateObject = Instantiate(prefab, new Vector3(positionX, positionY, 0f), Quaternion.identity, notesFolder.transform);
        LevelEditerNoteManager levelEditerNoteManager = instantiateObject.GetComponent<LevelEditerNoteManager>();
        levelEditerNoteManager.noteClass.position = position;
        levelEditerNoteManager.noteClass.beat = realBeat;
        levelEditerNoteManager.noteClass.type = noteType.ToLower();
        saveManager.notes.Add(levelEditerNoteManager.noteClass);
    }

    private void OnDropdownValueChanged(int index)
    {
        selectedBeat = dropdown.options[index].text;
        int beatNum = 4;
        switch (dropdown.options[index].text)
        {
            case "1/3":
                selectedBeat = "1_3";
                beatNum = 3;
                break;
            case "1/4":
                selectedBeat = "1_4";
                beatNum = 4;
                break;
            case "1/6":
                selectedBeat = "1_6";
                beatNum = 6;
                break;
            case "1/8":
                selectedBeat = "1_8";
                beatNum = 8;
                break;
            case "1/12":
                selectedBeat = "1_12";
                beatNum = 12;
                break;
            case "1/16":
                selectedBeat = "1_16";
                beatNum = 16;
                break;
            case "1/24":
                selectedBeat = "1_24";
                beatNum = 24;
                break;
            case "1/32":
                selectedBeat = "1_32";
                beatNum = 32;
                break;
            case "1/48":
                selectedBeat = "1_48";
                beatNum = 48;
                break;
            case "1/64":
                selectedBeat = "1_64";
                beatNum = 64;
                break;
        }

        GameObject beatPrefab = selectedBeat switch
        {
            "1_3" => beat13,
            "1_4" => beat14,
            "1_6" => beat16,
            "1_8" => beat18,
            "1_12" => beat112,
            "1_16" => beat116,
            "1_24" => beat124,
            "1_32" => beat132,
            _ => beat14
        };

        cellHeight = beatPrefab.transform.Find("Btn 1").gameObject.GetComponent<RectTransform>().rect.height;
        totalHeight = 9217 * cellHeight;

        CreateGridPool(beatNum);

        canvas.transform.localScale = Vector3.one;

        scrollY = Mathf.Clamp(scrollY, 0, totalHeight - cellHeight * maxVisibleRows);
        gridContainer.localPosition = new Vector3(0, -scrollY - 500, 0);

        int newTopIndex = Mathf.FloorToInt(scrollY / cellHeight);
        if (newTopIndex != currentTopIndex)
        {
            currentTopIndex = newTopIndex;
            UpdateVisibleGrids(newTopIndex);
        }
    }

    private void InputHandler()
    {
        canvas.transform.localScale = Vector3.one;

        if (Input.GetKey(KeyCode.W))
        {
            //notesFolder.transform.Translate(1000f * Time.deltaTime * Vector2.down);
            scrollY += 1000f * Time.deltaTime;
            CalcCurrentMusicTime();
        }
        if (Input.GetKey(KeyCode.S))
        {
            //notesFolder.transform.Translate(1000f * Time.deltaTime * Vector2.up);
            scrollY -= 1000f * Time.deltaTime;
            CalcCurrentMusicTime();
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            scrollY += 1000f * scroll;
            CalcCurrentMusicTime();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetIsRemoving("Add");
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetIsRemoving("Remove");
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            isMusicPlaying = !isMusicPlaying;
            eventInstance.setPaused(!isMusicPlaying);

            if (currentMoveSliderer == null)
            {
                currentMoveSliderer = StartCoroutine(MoveSlider());
            }
            else
            {
                StopCoroutine(currentMoveSliderer);
                currentMoveSliderer = null;
            }
        }
    }

    private void ScrollHandler()
    {
        scrollY = Mathf.Clamp(scrollY, 0, totalHeight - cellHeight * maxVisibleRows);
        gridContainer.localPosition = new Vector3(0, -scrollY - 500f, 0);

        notesFolder.transform.position = new Vector3(0, -scrollY + 280f, 0);

        int newTopIndex = Mathf.FloorToInt(scrollY / cellHeight);
        if (newTopIndex != currentTopIndex)
        {
            currentTopIndex = newTopIndex;
            UpdateVisibleGrids(newTopIndex);
        }
    }

    private void Update()
    {
        InputHandler();

        ScrollHandler();

        UpdateMusicTime();
        if (settingsPanel.activeSelf)
        {
            UpdateInputFieldValue();
        }
    }

    private void CreateGridPool(int beatNum)
    {
        gridPool.Clear(); // 기존 풀 제거

        // 기존에 생성된 오브젝트 제거
        foreach (Transform child in gridContainer)
        {
            Destroy(child.gameObject);
        }

        if (!beatPrefabMap.TryGetValue(selectedBeat, out GameObject prefab))
        {
            Debug.LogError($"선택된 Beat 프리셋({selectedBeat})에 해당하는 프리팹이 없습니다.");
            return;
        }

        for (int i = 0; i < maxVisibleRows + 2; i++)
        {
            GameObject go = Instantiate(prefab, gridContainer);
            go.SetActive(false);

            // 각 버튼(1~4)을 찾아서 리스너 바인딩
            for (int j = 1; j <= 4; j++)
            {
                Transform buttonTransform = go.transform.Find($"Btn {j}");
                if (buttonTransform != null)
                {
                    int pos = j; // 클로저 캡처 방지
                    Button btn = buttonTransform.GetComponent<Button>();
                    if (btn != null)
                    {
                        btn.onClick.RemoveAllListeners(); // 혹시 모르니 리스너 초기화
                        btn.onClick.AddListener(() => ButtonClickHandler(pos, beatNum, go.transform));
                    }
                    else
                    {
                        Debug.LogWarning($"Button component not found in Btn {j}");
                    }
                }
                else
                {
                    Debug.LogWarning($"Btn {j} not found in prefab.");
                }
            }

            gridPool.Add(go);
        }
    }

    private void UpdateVisibleGrids(int topIndex)
    {
        for (int i = 0; i < gridPool.Count; i++)
        {
            int beatIndex = topIndex + i;
            if (beatIndex >= 9217)
            {
                gridPool[i].SetActive(false);
                continue;
            }
            gridPool[i].SetActive(true);
            gridPool[i].transform.localPosition = new Vector3(0, beatIndex * cellHeight, 0);
        }
    }

    public TMP_InputField BPMInput;
    public TMP_InputField artistInput;
    public TMP_InputField titleInput;
    public TMP_InputField eventNameInput;
    public TMP_InputField difficultyInput;
    public TMP_InputField levelInput;

    private void UpdateInputFieldValue()
    {
        BPMInput.text = $"{BPM}";
        artistInput.text = $"{artist}";
        titleInput.text = $"{title}";
        eventNameInput.text = $"{eventName}";
        difficultyInput.text = $"{difficulty}";
        levelInput.text = $"{level}";
    }

    private void UpdateMusicTime()
    {
        if (eventInstance.isValid())
        {
            eventInstance.getTimelinePosition(out currentMusicTime);
        }
    }

    private void SetMusicTime(int musicTime)
    {
        currentMusicTime = musicTime;
        //Debug.Log(currentMusicTime);
        eventInstance.setTimelinePosition(currentMusicTime);
    }

    private void CalcCurrentMusicTime()
    {
        canvas.transform.localScale = Vector3.one;
        float musicTime = scrollY / scrollSpeed / 2f;
        SetMusicTime((int)musicTime);
    }

    IEnumerator MoveSlider()
    {
        while (isMusicPlaying)
        {
            canvas.transform.localScale = Vector3.one;
            UpdateMusicTime();
            //gridFolder.transform.Translate(scrollSpeed * Time.deltaTime * Vector2.down);
            scrollY = (scrollSpeed * currentMusicTime) * 2f;
            scrollY = Mathf.Clamp(scrollY, 0, totalHeight - cellHeight * maxVisibleRows);
            gridContainer.localPosition = new Vector3(0, -scrollY - 500f, 0);

            notesFolder.transform.position = new Vector3(0, -scrollY + 280f, 0);

            int newTopIndex = Mathf.FloorToInt(scrollY / cellHeight);
            if (newTopIndex != currentTopIndex)
            {
                currentTopIndex = newTopIndex;
                UpdateVisibleGrids(newTopIndex);
            }

            yield return null;
        }

        yield break;
    }

    public void SaveLevel()
    {
        saveManager.SaveToJson(Path.Combine(Application.streamingAssetsPath, $"{artist}-{title}.roena"), BPM, artist, title, eventName, level, difficulty);
    }

    public void LoadLevel()
    {
        LoadFromJson(Path.Combine(Application.streamingAssetsPath, $"{fileName}.roena"));
    }

    private void LoadFromJson(string filePath)
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);

            string decrypted = EncryptionHelper.Decrypt(json);

            NotesContainer container = JsonUtility.FromJson<NotesContainer>(decrypted);
            BPM = container.info.bpm;
            artist = container.info.artist;
            title = container.info.title;
            difficulty = container.info.difficulty;
            level = container.info.level;
            eventName = container.info.eventName;

            SetEventInstance(eventName);
            SetScrollSpeed();

            saveManager.notes = container.notes;

            Debug.Log("Chart loaded successfully!");

            scrollY = 0;
            gridContainer.localPosition = new Vector3(0, -scrollY - 500f, 0);

            int newTopIndex = Mathf.FloorToInt(scrollY / cellHeight);
            if (newTopIndex != currentTopIndex)
            {
                currentTopIndex = newTopIndex;
                UpdateVisibleGrids(newTopIndex);
            }

            PlaceNotesFromLoadedFile();
        }
        else
        {
            Debug.LogError("File not found at: " + filePath);
        }
    }

    private void PlaceNotesFromLoadedFile()
    {
        foreach (NoteClass note in saveManager.notes)
        {
            canvas.transform.localScale = Vector3.one;

            float positionX = 0f;
            if (note.position == 1)
            {
                positionX = -158f;
            }
            if (note.position == 2)
            {
                positionX = 158f / 3f * -1f;
            }
            if (note.position == 3f)
            {
                positionX = 158f / 3f;
            }
            if (note.position == 4f)
            {
                positionX = 158f;
            }

            float positionY = -211f + (320f * note.beat);
            notesFolder.transform.position = new Vector3(0, 280f, 0);

            GameObject prefab = note.type switch
            {
                "normal" => normalPrefab,
                "hold" => holdPrefab,
                "up" => upPrefab,
                _ => null
            };

            GameObject instantiateObject = Instantiate(prefab, new Vector3(positionX, positionY, 0f), Quaternion.identity, notesFolder.transform);
            LevelEditerNoteManager levelEditerNoteManager = instantiateObject.GetComponent<LevelEditerNoteManager>();
            levelEditerNoteManager.noteClass.position = note.position;
            levelEditerNoteManager.noteClass.beat = note.beat;
            levelEditerNoteManager.noteClass.type = note.type;
        }
    }

    [System.Serializable]
    private class NotesContainer
    {
        public SongInfoClass info;
        public List<NoteClass> notes;
    }

    public void SetLevel(string inputed)
    {
        float.TryParse(inputed, out level);
    }

    public void SetDifficulty(string inputed)
    {
        difficulty = inputed;
    }

    public void SetFileName(string inputed)
    {
        fileName = inputed;
    }

    public void SetBPM(string inputed)
    {
        float.TryParse(inputed, out BPM);
        SetScrollSpeed();
    }

    private void SetScrollSpeed()
    {
        scrollSpeed = 160f / (1000f * 60f / BPM);
    }

    public void SetNoteType(string type)
    {
        noteType = type;
    }

    public void SetArtist(string inputed)
    {
        artist = inputed;
    }

    public void SetTitle(string inputed)
    {
        title = inputed;
    }

    public void SetEventName(string inputed)
    {
        eventName = inputed;
        SetEventInstance(eventName);
    }

    private void SetEventInstance(string name)
    {
        eventInstance = RuntimeManager.CreateInstance($"event:/{name}");

        eventInstance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));

        eventInstance.setVolume(0.5f);
        eventInstance.start();

        eventInstance.setTimelinePosition(0);
        eventInstance.setPaused(true);
    }

    //public void SetFilePath()
    //{
    //    paths = StandaloneFileBrowser.OpenFolderPanel("Select Folder", "", false);
    //    if (paths.Length > 0)
    //    {
    //        Debug.Log(paths[0]);
    //    }
    //}

    public void SetIsRemoving(string inputed)
    {
        if (inputed == "Remove")
        {
            isRemoving = true;
            removeIndicator.SetActive(true);
            addIndicator.SetActive(false);
        }

        if (inputed == "Add")
        {
            isRemoving = false;
            removeIndicator.SetActive(false);
            addIndicator.SetActive(true);
        }
    }

    public void ChangeToTestScene()
    {
        canvas.SetActive(false);
        
        SceneManager.LoadSceneAsync("InGame", LoadSceneMode.Additive);
        Scene testScene = SceneManager.GetSceneByName("InGame");
        if (testScene.IsValid() && testScene.isLoaded)
        {
            saveManager.notes.Sort((note1, note2) => note1.beat.CompareTo(note2.beat));
            SceneManager.SetActiveScene(testScene);
        }
    }

    public void OpenSettingsPanel()
    {
        settingsPanel.SetActive(!settingsPanel.activeSelf);
        if (settingsPanel.activeSelf)
        {

        }
    }
}
