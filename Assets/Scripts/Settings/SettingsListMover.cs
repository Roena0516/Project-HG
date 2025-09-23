using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SettingsListMover : MonoBehaviour
{
    private SettingsManager settings;
    [SerializeField] SettingsList settingsListComponent;

    [SerializeField] private GameObject canvas;
    [SerializeField] private GameObject contentFolder;
    [SerializeField] private GameObject defaultSettingPrefab;

    private int listNum = 1;
    private float originX;
    private float originY;
    private bool isHold;

    private List<SettingComponent> settingsList;

    private Coroutine currentSetSongRoutine;
    private Coroutine repeatCoroutine;

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

    private void Start()
    {
        canvas.transform.localScale = Vector3.one;

        settings = SettingsManager.Instance;

        originX = contentFolder.transform.position.x;
        originY = contentFolder.transform.position.y;

        //isHold = false;

        listNum = 1;
    }

    public void SettingsListSetter()
    {
        settingsList = settingsListComponent.settingsList;

        foreach (SettingComponent item in settingsList)
        {
            // add setting into list
            GameObject settingObject = Instantiate(defaultSettingPrefab, contentFolder.transform);
            Transform left = settingObject.transform.Find("Left");
            Transform settingTitle = left.Find("Title");
            settingTitle.GetComponent<TextMeshProUGUI>().text = item.title;

            // set setting's value
            Transform right = settingObject.transform.Find("Right");
            Transform settingValue = right.Find("Title");
            settingValue.GetComponent<TextMeshProUGUI>().text = item.value[item.initialIndex];

            SettingObjectComponent settingObjectComponent = settingObject.GetComponent<SettingObjectComponent>();

            settingObjectComponent.data.title = item.title;
            settingObjectComponent.data.value = item.value;
            settingObjectComponent.data.initialIndex = item.initialIndex;
            settingObjectComponent.data.category = item.category;
        }

        SetList(1);
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
            Image leftArrow = prevImage.gameObject.transform.Find("LeftArrow").GetComponent<Image>();
            leftArrow.color = leftArrow.color.SetAlpha(0f);
            Image rightArrow = prevImage.gameObject.transform.Find("RightArrow").GetComponent<Image>();
            leftArrow.color = rightArrow.color.SetAlpha(0f);

            Transform current = contentFolder.transform.GetChild(listNum - 1);
            current.GetComponent<Image>().color = current.GetComponent<Image>().color.SetAlpha(0.4f);
            Image currentLeftArrow = prevImage.gameObject.transform.Find("LeftArrow").GetComponent<Image>();
            leftArrow.color = currentLeftArrow.color.SetAlpha(1f);
            Image currentRightArrow = prevImage.gameObject.transform.Find("RightArrow").GetComponent<Image>();
            leftArrow.color = currentRightArrow.color.SetAlpha(1f);


            SettingObjectComponent settingObjectComponent = current.GetComponent<SettingObjectComponent>();
        }
    }

    private IEnumerator SetSong(int index)
    {
        canvas.transform.localScale = Vector3.one;

        Transform T = contentFolder.transform;

        float elapsedTime = 0f;
        Vector3 startPos = new Vector3(T.position.x, T.position.y, 0f);
        float duration = 0.15f;
        Vector3 targetPos = new Vector3(originX, originY - originY + (defaultSettingPrefab.GetComponent<RectTransform>().sizeDelta.y * index), 0f);

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

        if (!isHold)
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

    [System.Obsolete]
    private IEnumerator RepeatKeyPress(string actionName)
    {
        yield return new WaitForSeconds(0.3f);

        while (isHold)
        {
            switch (actionName)
            {
                case "ListUp":
                    SetList(listNum - 1);
                    break;
                case "ListDown":
                    SetList(listNum + 1);
                    break;

            }
            yield return new WaitForSeconds(0.05f);
        }
    }
}
