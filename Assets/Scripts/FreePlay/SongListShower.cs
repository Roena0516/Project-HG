using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections;
using System.IO;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class SongListShower : MonoBehaviour
{
    private SettingsManager settings;

    public LoadAllJSONs loader;
    public GetResults getResults;
    private List<SongInfoClass> allOfInfos;

    public GameObject contentFolder;
    public GameObject songListFolder;
    public GameObject viewPortFolder;
    public GameObject songPrefab;
    public GameObject canvas;
    public GameObject difficultyIndicator;

    public TextMeshProUGUI speedText;
    public TextMeshProUGUI bgnText;
    public TextMeshProUGUI midText;
    public TextMeshProUGUI endText;
    public TextMeshProUGUI sndText;

    public TextMeshProUGUI info_titleText;
    public TextMeshProUGUI info_artistText;
    public TextMeshProUGUI info_bpmText;

    public TextMeshProUGUI info_rateText;
    public TextMeshProUGUI info_comboText;
    public TextMeshProUGUI info_ratingText;

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

    private List<Result> results;

    private void Start()
    {
        canvas.transform.localScale = Vector3.one;

        settings = SettingsManager.Instance;

        //contentFolder.transform.position = new Vector3(contentFolder.transform.position.x, -1f * (viewPortFolder.transform.position.y / 2f) + (songPrefab.GetComponent<RectTransform>().sizeDelta.y / 2f), 0f);

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
        // Result 리스트 불러오기
        results = getResults.GetResultsAPI().results;

        foreach (var pair in loader.songDictionary)
        {

            // songDictionary의 키
            string key = pair.Key;

            // 노래 정보를 담아둘 클래스
            SongInfoClass info = null;

            // 노래 정보 가져오기
            if (loader.songDictionary.TryGetValue(key, out var songDifficulty))
            {
                info = songDifficulty
                    .Take(4) // 0 ~ 3 인덱스
                    .FirstOrDefault(s => s.level != 0); // level이 0이 아닌 첫 번째
            }

            if (selectedSongInfo.artist == "")
            {
                SetSelectedSongInfo(info);
            }

            Debug.Log($"{info.id} {info.artist} {info.title} {info.bpm}");


            // 노래 리스트에 노래 추가
            GameObject song = Instantiate(songPrefab, contentFolder.transform);
            Transform left = song.transform.Find("Left");
            Transform right = song.transform.Find("Right");
            Transform artistTitle = left.Find("SongInfo");
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

            // 난이도 텍스트 표시
            string mem = loader.songDictionary[key][0].level > 0 ? $"{loader.songDictionary[key][0].level}" : "";
            string adv = loader.songDictionary[key][1].level > 0 ? $"{loader.songDictionary[key][1].level}" : "";
            string nmr = loader.songDictionary[key][2].level > 0 ? $"{loader.songDictionary[key][2].level}" : "";
            string inf = loader.songDictionary[key][3].level > 0 ? $"{loader.songDictionary[key][3].level}" : "";

            Transform difficulty = right.Find("Difficulty");
            difficulty.transform.Find("MEM").gameObject.GetComponent<TextMeshProUGUI>().text = $"{mem}";
            difficulty.transform.Find("ADV").gameObject.GetComponent<TextMeshProUGUI>().text = $"{adv}";
            difficulty.transform.Find("NMR").gameObject.GetComponent<TextMeshProUGUI>().text = $"{nmr}";
            difficulty.transform.Find("INF").gameObject.GetComponent<TextMeshProUGUI>().text = $"{inf}";

            SongListInfoSetter setter = song.GetComponent<SongListInfoSetter>();

            // 빈 기록
            Result empty = new()
            {
                playerId = "1",
                musicId = info.id,
                rate = 0,
                combo = 0,
                perfectPlus = 0,
                perfect = 0,
                great = 0,
                good = 0,
                miss = 0,
                played_at = ""
            };

            // 난이도 별 기록 리스트
            if (setter.results.Count == 0)
            {
                setter.results.Add(empty);
                setter.results.Add(empty);
                setter.results.Add(empty);
                setter.results.Add(empty);
            }

            // 난이도 별 채보 파일 경로 리스트
            if (setter.filePath.Count == 0)
            {
                setter.filePath.Add("");
                setter.filePath.Add("");
                setter.filePath.Add("");
                setter.filePath.Add("");
            }

            // 난이도 별 노래 id 리스트
            if (setter.ids.Count == 0)
            {
                setter.ids.Add(0);
                setter.ids.Add(0);
                setter.ids.Add(0);
                setter.ids.Add(0);
            }

            setter.artist = info.artist;
            setter.jpArtist = info.jpArtist;
            setter.title = info.title;
            setter.jpTitle = info.jpTitle;
            setter.BPM = info.bpm;
            setter.eventName = info.eventName;

            // 난이도 따른 파일 경로 및 기록 지정
            List<SongInfoClass> songList = loader.songDictionary[key];
            foreach (SongInfoClass infos in songList)
            {
                if (allOfInfos == null)
                {
                    allOfInfos = new();
                }
                allOfInfos.Add(infos);

                if (infos.difficulty == "MEMORY")
                {
                    setter.filePath[0] = infos.fileLocation;

                    // 해당 노래의 기록 불러오기
                    Result found = null;
                    if (results != null)
                    {
                        found = results.FirstOrDefault(r => r.musicId == infos.id);
                    }
                    if (found != null)
                    {
                        setter.results[0] = found;
                    }

                    // id 저장
                    setter.ids[0] = infos.id;
                }
                if (infos.difficulty == "ADVERSITY")
                {
                    setter.filePath[1] = infos.fileLocation;

                    Result found = null;
                    if (results != null)
                    {
                        found = results.FirstOrDefault(r => r.musicId == infos.id);
                    }
                    if (found != null)
                    {
                        setter.results[1] = found;
                    }

                    setter.ids[1] = infos.id;
                }
                if (infos.difficulty == "NIGHTMARE")
                {
                    setter.filePath[2] = infos.fileLocation;

                    Result found = null;
                    if (results != null)
                    {
                        found = results.FirstOrDefault(r => r.musicId == infos.id);
                    }
                    if (found != null)
                    {
                        setter.results[2] = found;
                    }

                    setter.ids[2] = infos.id;
                }
                if (infos.difficulty == "INFERNO")
                {
                    setter.filePath[3] = infos.fileLocation;

                    Result found = null;
                    if (results != null)
                    {
                        found = results.FirstOrDefault(r => r.musicId == infos.id);
                    }
                    if (found != null)
                    {
                        setter.results[3] = found;
                    }

                    setter.ids[3] = infos.id;
                }
            }
        }

        SetList(1);
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
        settings.SetSpeed($"{(settings.settings.speed + 0.1f):F1}");
        speedText.text = $"{settings.settings.speed:F1}";
        settings.SaveSettings();
    }
    public void SpeedOneDown()
    {
        settings.SetSpeed($"{(settings.settings.speed - 0.1f):F1}");
        speedText.text = $"{settings.settings.speed:F1}";
        settings.SaveSettings();
    }

    // id로 Result 가져오기
    private Result GetResult(int musicId)
    {
        // 해당 id의 기록 불러오기
        Result found = null;
        if (results != null)
        {
            found = results.FirstOrDefault(r => r.musicId == musicId);
        }
        if (found == null || found.playerId != $"{settings.GetPlayerData().id}")
        {
            // 빈 기록
            Result empty = new()
            {
                playerId = $"{settings.GetPlayerData().id}",
                musicId = musicId,
                rate = 0,
                combo = 0,
                perfectPlus = 0,
                perfect = 0,
                great = 0,
                good = 0,
                miss = 0,
                played_at = ""
            };

            return empty;
        }

        return found;
    }

    // Result UI 변경
    private void SetResult(Result result)
    {
        info_rateText.text = $"{result.rate:F2}%";
        info_comboText.text = $"{result.combo}";
        info_ratingText.text = $"{result.rate * result.combo / 1000f:F3}";
    }

    private void SetList(int toIndex)
    {
        if (toIndex > 0 && toIndex <= contentFolder.transform.childCount)
        {
            int prev = listNum;
            listNum = toIndex;

            Debug.Log(toIndex);

            if (listNum > 3 && listNum < contentFolder.transform.childCount - 4)
            {
                if (currentSetSongRoutine != null)
                {
                    StopCoroutine(currentSetSongRoutine);
                }
                currentSetSongRoutine = StartCoroutine(SetSong(listNum - 1));
            }

            Image prevImage = contentFolder.transform.GetChild(prev - 1).GetComponent<Image>();
            prevImage.color = prevImage.color.SetAlpha(0f);

            Transform current = contentFolder.transform.GetChild(listNum - 1);
            current.GetComponent<Image>().color = current.GetComponent<Image>().color.SetAlpha(0.4f);

            SongListInfoSetter setter = current.GetComponent<SongListInfoSetter>();

            DifficultySetter(setter.artist + "-" + setter.title);
            SetInfoBoard(setter);

            SetDifficulty(selectedDifficulty, 1);

            Result found = GetResult(setter.ids[selectedDifficulty - 1]);
            if (found != null)
            {
                SetResult(found);
            }
        }
    }

    private void SetInfoBoard(SongListInfoSetter setter)
    {
        info_titleText.text = $"{setter.title}";
        info_artistText.text = $"{setter.artist}";
        info_bpmText.text = $"{setter.BPM} BPM";
    }

    private IEnumerator SetSong(int index)
    {
        canvas.transform.localScale = Vector3.one;

        Transform T = contentFolder.transform;

        float elapsedTime = 0f;
        Vector3 startPos = new Vector3(T.position.x, T.position.y, 0f);
        float duration = 0.15f;
        Vector3 targetPos = new Vector3(originX, originY - originY + (songPrefab.GetComponent<RectTransform>().sizeDelta.y * index), 0f);

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
            if (infos.difficulty == "MEMORY")
            {
                bgnText.color = bgnText.color.SetAlpha(1f);
                bgnText.text = $"{infos.level}";
            }
            if (infos.difficulty == "ADVERSITY")
            {
                midText.color = midText.color.SetAlpha(1f);
                midText.text = $"{infos.level}";
            }
            if (infos.difficulty == "NIGHTMARE")
            {
                endText.color = endText.color.SetAlpha(1f);
                endText.text = $"{infos.level}";
            }
            if (infos.difficulty == "INFERNO")
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

            Transform current = contentFolder.transform.GetChild(listNum - 1);
            SongListInfoSetter setter = current.GetComponent<SongListInfoSetter>();
            Result found = GetResult(setter.ids[selectedDifficulty - 1]);
            if (found != null)
            {
                SetResult(found);
            }

            SongInfoClass foundInfoClass = allOfInfos.FirstOrDefault(info => info.id == setter.ids[selectedDifficulty - 1]);
            if (foundInfoClass == null)
            {
                Debug.LogError($"info is not found: {setter.ids[selectedDifficulty - 1]} id");
            }
            SetSelectedSongInfo(foundInfoClass);

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
        Vector3 targetPos = new(indicatorOriginX + 111.75f * index, T.position.y, 0f);

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
        settings.SetSongTitle(setter.title);
        settings.SetSongArtist(setter.artist);
        settings.SetEventName(setter.eventName);
        settings.Info = selectedSongInfo;

        SceneManager.LoadSceneAsync("InGame");
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
