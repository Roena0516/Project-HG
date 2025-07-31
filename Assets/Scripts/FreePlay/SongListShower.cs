using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections;
using System.IO;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class SongListShower : MonoBehaviour
{
    private SettingsManager settings;

    public LoadAllJSONs loader;

    public GameObject contentFolder;
    public GameObject songListFolder;
    public GameObject songPrefab;
    public GameObject canvas;
    public GameObject difficultyIndicator;

    public TextMeshProUGUI speedText;
    public TextMeshProUGUI bgnText;
    public TextMeshProUGUI midText;
    public TextMeshProUGUI endText;
    public TextMeshProUGUI sndText;

    public GameObject syncInput;
    public GameObject speedInput;
    public TMP_Dropdown dropdown;

    private float originX;
    private float indicatorOriginX;
    private float originY;

    public int listNum;

    public int selectedDifficulty;

    private bool isHold;

    private Coroutine currentSetSongRoutine;
    private Coroutine currentSetDifficultyRoutine;
    private Coroutine repeatCoroutine;

    public SongInfoClass selectedSongInfo;
    //private Coroutine currentSetSongIndexRoutine;

    //public static SongListShower Instance { get; private set; }

    private void Start()
    {
        canvas.transform.localScale = Vector3.one;

        settings = SettingsManager.Instance;

        contentFolder.transform.position = new Vector3(contentFolder.transform.position.x, -1f * (songListFolder.transform.position.y / 2f) + (songPrefab.GetComponent<RectTransform>().sizeDelta.y / 2f), 0f);

        originX = contentFolder.transform.position.x;
        indicatorOriginX = difficultyIndicator.transform.position.x;
        originY = contentFolder.transform.position.y;

        isHold = false;

        listNum = 1;
        selectedDifficulty = 1;

        speedText.text = $"{settings.settings.speed:F1}";

        dropdown.onValueChanged.AddListener(OnDropdownValueChanged);
        Debug.Log(settings.settings.effectOption);
        if (settings.settings.effectOption == "None")
        {
            dropdown.value = 0;
        }
        if (settings.settings.effectOption == "Random")
        {
            dropdown.value = 1;
        }
        if (settings.settings.effectOption == "Half Random")
        {
            dropdown.value = 2;
        }
        if (settings.settings.effectOption == "L. Quater Random")
        {
            dropdown.value = 3;
        }
        if (settings.settings.effectOption == "R. Quater Random")
        {
            dropdown.value = 4;
        }
        Debug.Log($"value : {dropdown.value}");
    }

    public void Shower()
    {
        foreach (var pair in loader.songDictionary)
        {
            string key = pair.Key;

            SongInfoClass info = loader.songDictionary[key][0];

            if (selectedSongInfo.artist == "")
            {
                SetSelectedSongInfo(info);
            }

            Debug.Log($"{info.artist} {info.title} {info.bpm}");

            GameObject song = Instantiate(songPrefab, contentFolder.transform);
            Transform artistTitle = song.transform.Find("Artist-TitlePanel");
            if (settings.settings.isKR)
            {
                artistTitle.Find("TitleText").gameObject.GetComponent<TextMeshProUGUI>().text = info.title;
                artistTitle.Find("ArtistText").gameObject.GetComponent<TextMeshProUGUI>().text = info.artist;
            }
            else
            {
                artistTitle.Find("TitleText").gameObject.GetComponent<TextMeshProUGUI>().text = info.jpTitle;
                artistTitle.Find("ArtistText").gameObject.GetComponent<TextMeshProUGUI>().text = info.jpArtist;
            }
            song.transform.Find("BPMText").gameObject.GetComponent<TextMeshProUGUI>().text = $"{info.bpm}BPM";

            SongListInfoSetter setter = song.GetComponent<SongListInfoSetter>();
            setter.filePath.Add("");
            setter.filePath.Add("");
            setter.filePath.Add("");
            setter.filePath.Add("");

            setter.artist = info.artist;
            setter.title = info.title;

            List<SongInfoClass> songList = loader.songDictionary[key];
            foreach (SongInfoClass infos in songList)
            {
                if (infos.difficulty == "BEGINNING")
                {
                    setter.filePath[0] = infos.fileLocation;
                }
                if (infos.difficulty == "MIDDLE")
                {
                    setter.filePath[1] = infos.fileLocation;
                }
                if (infos.difficulty == "END")
                {
                    setter.filePath[2] = infos.fileLocation;
                }
                if (infos.difficulty == "STARTEND")
                {
                    setter.filePath[3] = infos.fileLocation;
                }
            }
        }
    }

    private void SetSelectedSongInfo(SongInfoClass info)
    {
        selectedSongInfo = info;
        DifficultySetter(info.artist + "-" + info.title);
    }

    public MainInputAction action;
    private InputAction listUp;
    private InputAction listDown;
    private InputAction DifficultyUp;
    private InputAction DifficultyDown;
    private InputAction scrollList;
    private InputAction songSelect;
    private InputAction exitSongList;
    private InputAction speedUp;
    private InputAction speedDown;

    private void Awake()
    {
        action = new MainInputAction();
        listUp = action.FreePlay.ListUp;
        listDown = action.FreePlay.ListDown;
        DifficultyUp = action.FreePlay.DifficultyUp;
        DifficultyDown = action.FreePlay.DifficultyDown;
        scrollList = action.FreePlay.ScrollList;
        songSelect = action.FreePlay.SongSelect;
        exitSongList = action.FreePlay.ExitSongList;
        speedUp = action.FreePlay.SpeedUp;
        speedDown = action.FreePlay.SpeedDown;

        bgnText.color = bgnText.color.SetAlpha(0f);
        midText.color = midText.color.SetAlpha(0f);
        endText.color = endText.color.SetAlpha(0f);
        sndText.color = sndText.color.SetAlpha(0f);

        //if (Instance == null)
        //{
        //    Instance = this;
        //}
        //else
        //{
        //    Destroy(gameObject);
        //}
    }

    [System.Obsolete]
    private void OnEnable()
    {
        listUp.Enable();
        listUp.started += Started;
        listUp.canceled += Canceled;

        listDown.Enable();
        listDown.started += Started;
        listDown.canceled += Canceled;

        DifficultyUp.Enable();
        DifficultyUp.started += Started;
        DifficultyUp.canceled += Canceled;

        DifficultyDown.Enable();
        DifficultyDown.started += Started;
        DifficultyDown.canceled += Canceled;

        scrollList.Enable();
        scrollList.performed += OnScroll;

        songSelect.Enable();
        songSelect.started += Started;
        songSelect.canceled += Canceled;

        exitSongList.Enable();
        exitSongList.started += Started;
        exitSongList.canceled += Canceled;

        speedUp.Enable();
        speedUp.started += Started;
        speedUp.canceled += Canceled;

        speedDown.Enable();
        speedDown.started += Started;
        speedDown.canceled += Canceled;
    }

    [System.Obsolete]
    private void OnDisable()
    {
        listUp.Disable();
        listUp.started -= Started;
        listUp.canceled -= Canceled;

        listDown.Disable();
        listDown.started -= Started;
        listDown.canceled -= Canceled;

        DifficultyUp.Disable();
        DifficultyUp.started -= Started;
        DifficultyUp.canceled -= Canceled;

        DifficultyDown.Disable();
        DifficultyDown.started -= Started;
        DifficultyDown.canceled -= Canceled;

        scrollList.Disable();
        scrollList.performed -= OnScroll;

        songSelect.Disable();
        songSelect.started -= Started;
        songSelect.canceled -= Canceled;

        exitSongList.Disable();
        exitSongList.started -= Started;
        exitSongList.canceled -= Canceled;

        speedUp.Disable();
        speedUp.started -= Started;
        speedUp.canceled -= Canceled;

        speedDown.Disable();
        speedDown.started -= Started;
        speedDown.canceled -= Canceled;
    }

    [System.Obsolete]
    private void OnScroll(InputAction.CallbackContext context)
    {
        Vector2 scrollDelta = context.ReadValue<Vector2>();

        if (scrollDelta.y > 0)
        {
            SetList(listNum - 1);
        }
        if (scrollDelta.y < 0)
        {
            SetList(listNum + 1);
        }
    }

    [System.Obsolete]
    void Started(InputAction.CallbackContext context)
    {
        string actionName = context.action.name;

        if(!isHold)
        {
            isHold = true;

            switch (actionName)
            {
                case "ListUp":
                    SetList(listNum - 1);
                    repeatCoroutine = StartCoroutine(RepeatKeyPress(actionName));
                    break;
                case "ListDown":
                    SetList(listNum + 1);
                    repeatCoroutine = StartCoroutine(RepeatKeyPress(actionName));
                    break;
                case "DifficultyUp":
                    SetDifficulty(selectedDifficulty + 1, 1);
                    repeatCoroutine = StartCoroutine(RepeatKeyPress(actionName));
                    break;
                case "DifficultyDown":
                    SetDifficulty(selectedDifficulty - 1, -1);
                    repeatCoroutine = StartCoroutine(RepeatKeyPress(actionName));
                    break;
                case "SongSelect":
                    SelectSong(listNum - 1);
                    break;
                case "ExitSongList":
                    SceneManager.LoadScene("Menu");
                    break;
                case "SpeedUp":
                    SpeedOneUp();
                    repeatCoroutine = StartCoroutine(RepeatKeyPress(actionName));
                    break;
                case "SpeedDown":
                    SpeedOneDown();
                    repeatCoroutine = StartCoroutine(RepeatKeyPress(actionName));
                    break;
            }
        }
    }

    void Canceled(InputAction.CallbackContext context)
    {
        isHold = false;

        if (repeatCoroutine != null)
        {
            StopCoroutine(repeatCoroutine);
            repeatCoroutine = null;
        }
    }

    public void SpeedOneUp()
    {
        settings.settings.speed += 0.1f;
        speedText.text = $"{settings.settings.speed:F1}";
    }
    public void SpeedOneDown()
    {
        settings.settings.speed -= 0.1f;
        speedText.text = $"{settings.settings.speed:F1}";
    }

    [System.Obsolete]
    private void SetList(int toIndex)
    {
        //contentFolder.transform.position = new Vector3(contentFolder.transform.position.x, contentFolder.transform.position.y + (songPrefab.GetComponent<RectTransform>().sizeDelta.y * targetIndex), 0f);
            //Debug.Log(contentFolder.transform.GetChildCount());
        if (toIndex > 0 && toIndex <= contentFolder.transform.GetChildCount())
        {
            listNum = toIndex;

            Debug.Log(toIndex);
            if (currentSetSongRoutine != null)
            {
                StopCoroutine(currentSetSongRoutine);
            }
            currentSetSongRoutine = StartCoroutine(SetSong(listNum - 1));

            SongListInfoSetter setter = contentFolder.transform.GetChild(listNum - 1).GetComponent<SongListInfoSetter>();
            DifficultySetter(setter.artist + "-" + setter.title);

            SetDifficulty(selectedDifficulty, 1);
        }
    }

    private IEnumerator SetSong(int index)
    {
        canvas.transform.localScale = Vector3.one;

        Transform T = contentFolder.transform;

        float elapsedTime = 0f;
        Vector3 startPos = new Vector3(T.position.x, T.position.y, 0f);
        float duration = 0.15f;
        Vector3 targetPos = new Vector3(originX, originY + (songPrefab.GetComponent<RectTransform>().sizeDelta.y * index), 0f);

        while (elapsedTime < duration)
        {
            canvas.transform.localScale = Vector3.one;

            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            float easedT = Mathf.Sin(t * Mathf.PI * 0.5f);

            T.position = Vector3.Lerp(startPos, targetPos, easedT);

            yield return null;
        }

        canvas.transform.localScale = Vector3.one;
        T.position = targetPos;

        currentSetSongRoutine = null;

        yield break;
    }

    private void DifficultySetter(string key)
    {
        List<SongInfoClass> songList = loader.songDictionary[key];

        bgnText.color = bgnText.color.SetAlpha(0f);
        bgnText.text = $"0";
        midText.color = midText.color.SetAlpha(0f);
        midText.text = $"0";
        endText.color = endText.color.SetAlpha(0f);
        endText.text = $"0";
        sndText.color = sndText.color.SetAlpha(0f);
        sndText.text = $"0";

        foreach (SongInfoClass infos in songList)
        {
            if (infos.difficulty == "BEGINNING")
            {
                bgnText.color = bgnText.color.SetAlpha(1f);
                bgnText.text = $"{infos.level}";
            }
            if (infos.difficulty == "MIDDLE")
            {
                midText.color = midText.color.SetAlpha(1f);
                midText.text = $"{infos.level}";
            }
            if (infos.difficulty == "END")
            {
                endText.color = endText.color.SetAlpha(1f);
                endText.text = $"{infos.level}";
            }
            if (infos.difficulty == "STARTEND")
            {
                sndText.color = sndText.color.SetAlpha(1f);
                sndText.text = $"{infos.level}";
            }
            if (infos.difficulty == null)
            {
                sndText.color = sndText.color.SetAlpha(0f);
                sndText.text = $"{infos.level}";
            }
        }
    }

    private void SetDifficulty(int toIndex, int index)
    {
        if (toIndex > 0 && toIndex <= 4)
        {
            if (toIndex == 1)
            {
                if (bgnText.color.a == 0)
                {
                    SetDifficulty(toIndex + index, index);
                    return;
                }
            }
            if (toIndex == 2)
            {
                if (midText.color.a == 0)
                {
                    SetDifficulty(toIndex + index, index);
                    return;
                }
            }
            if (toIndex == 3)
            {
                if (endText.color.a == 0)
                {
                    SetDifficulty(toIndex + index, index);
                    return;
                }
            }
            if (toIndex == 4)
            {
                if (sndText.color.a == 0)
                {
                    return;
                }
            }
            selectedDifficulty = toIndex;
            if (currentSetDifficultyRoutine != null)
            {
                StopCoroutine(currentSetDifficultyRoutine);
            }
            currentSetDifficultyRoutine = StartCoroutine(SetDifficultyIndicator(toIndex - 1));
        }
    }

    private IEnumerator SetDifficultyIndicator(int index)
    {
        canvas.transform.localScale = Vector3.one;

        Transform T = difficultyIndicator.transform;

        float elapsedTime = 0f;
        Vector3 startPos = new(T.position.x, T.position.y, 0f);
        float duration = 0.15f;
        Vector3 targetPos = new(indicatorOriginX + 75f * index, T.position.y, 0f);

        while (elapsedTime < duration)
        {
            canvas.transform.localScale = Vector3.one;

            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            float easedT = Mathf.Sin(t * Mathf.PI * 0.5f);

            T.position = Vector3.Lerp(startPos, targetPos, easedT);

            yield return null;
        }

        canvas.transform.localScale = Vector3.one;
        T.position = targetPos;

        currentSetDifficultyRoutine = null;

        yield break;
    }

    [System.Obsolete]
    private IEnumerator RepeatKeyPress(string actionName)
    {
        yield return new WaitForSeconds(0.3f);

        while (isHold)
        {
            switch(actionName)
            {
                case "ListUp":
                    SetList(listNum - 1);
                    break;
                case "ListDown":
                    SetList(listNum + 1);
                    break;
                case "DifficultyUp":
                    SetDifficulty(selectedDifficulty + 1, 1);
                    break;
                case "DifficultyDown":
                    SetDifficulty(selectedDifficulty - 1, -1);
                    break;
                case "SpeedUp":
                    SpeedOneUp();
                    break;
                case "SpeedDown":
                    SpeedOneDown();
                    break;

            }
            yield return new WaitForSeconds(0.05f);
        }
    }

    public void SelectSong(int n)
    {
        SongListInfoSetter setter = contentFolder.transform.GetChild(n).GetComponent<SongListInfoSetter>();

        settings.SetFileName($"{setter.filePath[selectedDifficulty - 1]}");
        settings.songTitle = setter.title;
        settings.songArtist = setter.artist;

        SceneManager.LoadSceneAsync("InGame");
    }

    public void SetSync(string inputed)
    {
        settings.SetSync(inputed);
    }
    public void SetSpeed(string inputed)
    {
        settings.SetSpeed(inputed);
    }

    private void OnDropdownValueChanged(int index)
    {
        string selectedOption = dropdown.options[index].text;
        DropdownHandler(selectedOption);
    }

    public void DropdownHandler(string option)
    {
        settings.settings.effectOption = option;
    }
}
