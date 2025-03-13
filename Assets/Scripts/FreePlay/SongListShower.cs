using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections;
using System.IO;
using UnityEngine.SceneManagement;

public class SongListShower : MonoBehaviour
{
    private SettingsManager settings;

    private LoadAllJSONs loader;

    public GameObject contentFolder;
    public GameObject songListFolder;
    public GameObject songPrefab;
    public GameObject canvas;
    public TextMeshProUGUI speedText;

    public GameObject syncInput;
    public GameObject speedInput;

    private float originX;
    private float originY;

    public int listNum;

    private bool isHold;

    private Coroutine currentSetSongRoutine;
    private Coroutine repeatCoroutine;
    //private Coroutine currentSetSongIndexRoutine;

    [System.Obsolete]
    private void Start()
    {
        canvas.transform.localScale = Vector3.one;

        loader = FindObjectOfType<LoadAllJSONs>();
        settings = FindObjectOfType<SettingsManager>();

        contentFolder.transform.position = new Vector3(contentFolder.transform.position.x, -1f * (songListFolder.transform.position.y / 2f) + (songPrefab.GetComponent<RectTransform>().sizeDelta.y / 2f), 0f);

        originX = contentFolder.transform.position.x;
        originY = contentFolder.transform.position.y;

        isHold = false;

        listNum = 1;

        speedText.text = $"{settings.speed:F1}";
    }

    public void Shower()
    {
        foreach (SongInfoClass info in loader.songList)
        {
            Debug.Log($"{info.artist} {info.title} {info.bpm}");

            GameObject song = Instantiate(songPrefab, contentFolder.transform);
            Transform artistTitle = song.transform.Find("Artist-TitlePanel");
            artistTitle.Find("TitleText").gameObject.GetComponent<TextMeshProUGUI>().text = info.title;
            artistTitle.Find("ArtistText").gameObject.GetComponent<TextMeshProUGUI>().text = info.artist;

            song.transform.Find("BPMText").gameObject.GetComponent<TextMeshProUGUI>().text = $"{info.bpm}BPM";

            song.GetComponent<SongListInfoSetter>().artist = info.artist;
            song.GetComponent<SongListInfoSetter>().title = info.title;
        }
    }

    public MainInputAction action;
    private InputAction listUp;
    private InputAction listDown;
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
        scrollList = action.FreePlay.ScrollList;
        songSelect = action.FreePlay.SongSelect;
        exitSongList = action.FreePlay.ExitSongList;
        speedUp = action.FreePlay.SpeedUp;
        speedDown = action.FreePlay.SpeedDown;
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
        settings.speed += 0.1f;
        speedText.text = $"{settings.speed:F1}";
    }
    public void SpeedOneDown()
    {
        settings.speed -= 0.1f;
        speedText.text = $"{settings.speed:F1}";
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
        }

        //if (currentSetSongIndexRoutine == null)
        //{
        //    currentSetSongIndexRoutine = StartCoroutine(SetSongIndex(contentFolder.transform.GetChild(listNum).gameObject));
        //}
    }

    //private IEnumerator SetSongIndex(GameObject song)
    //{
    //    canvas.transform.localScale = Vector3.one;

    //    Transform T = song.transform;

    //    float elapsedTime = 0f;
    //    Vector3 startPos = new Vector3(originX, T.position.y, 0f);
    //    float duration = 0.25f;
    //    Vector3 targetPos = new Vector3(originX - 20f, T.position.y, 0f);

    //    while (elapsedTime < duration)
    //    {
    //        canvas.transform.localScale = Vector3.one;

    //        elapsedTime += Time.deltaTime;
    //        float t = Mathf.Clamp01(elapsedTime / duration);

    //        float easedT = t;
    //        easedT = Mathf.Sin(t * Mathf.PI * 0.5f);

    //        T.position = Vector3.Lerp(startPos, targetPos, easedT);

    //        yield return null;
    //    }

    //    canvas.transform.localScale = Vector3.one;
    //    T.position = targetPos;

    //    currentSetSongIndexRoutine = null;

    //    yield break;
    //}

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
        Debug.Log(n);
        SongListInfoSetter setter = contentFolder.transform.GetChild(n).GetComponent<SongListInfoSetter>();

        settings.SetFileName($"{setter.artist}-{setter.title}");

        SceneManager.LoadScene("InGame");
    }

    public void SetSync(string inputed)
    {
        settings.SetSync(inputed);
    }
    public void SetSpeed(string inputed)
    {
        settings.SetSpeed(inputed);
    }
}
