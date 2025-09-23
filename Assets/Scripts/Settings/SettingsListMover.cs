using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsListMover : MonoBehaviour
{
    private SettingsManager settings;
    [SerializeField] private SettingsList settingsListComponent;

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

    private void Start()
    {
        canvas.transform.localScale = Vector3.one;

        settings = SettingsManager.Instance;

        originX = contentFolder.transform.position.x;
        originY = contentFolder.transform.position.y;

        listNum = 1;
    }

    private void Update()
    {
        // 한 번 눌렀을 때
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            SetList(listNum - 1);
            BeginRepeat("Up");
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            SetList(listNum + 1);
            BeginRepeat("Down");
        }

        // 뗐을 때 반복 중지
        if (Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.DownArrow))
        {
            EndRepeat();
        }
    }

    private void BeginRepeat(string dir)
    {
        if (isHold) return;
        isHold = true;

        if (repeatCoroutine != null)
        {
            StopCoroutine(repeatCoroutine);
        }
        repeatCoroutine = StartCoroutine(RepeatKeyPress(dir));
    }

    private void EndRepeat()
    {
        isHold = false;
        if (repeatCoroutine != null)
        {
            StopCoroutine(repeatCoroutine);
            repeatCoroutine = null;
        }
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

            if (listNum > 3 && listNum < contentFolder.transform.childCount - 4)
            {
                if (currentSetSongRoutine != null)
                {
                    StopCoroutine(currentSetSongRoutine);
                }
                currentSetSongRoutine = StartCoroutine(SetSong(listNum - 1));
            }

            // 이전 선택 항목 비활성 비주얼
            Image prevImage = contentFolder.transform.GetChild(prev - 1).GetComponent<Image>();
            prevImage.color = prevImage.color.SetAlpha(0f);

            Image prevLeftArrow = prevImage.transform.Find("LeftArrow").GetComponent<Image>();
            Image prevRightArrow = prevImage.transform.Find("RightArrow").GetComponent<Image>();
            prevLeftArrow.color = prevLeftArrow.color.SetAlpha(0f);
            prevRightArrow.color = prevRightArrow.color.SetAlpha(0f);

            // 현재 선택 항목 활성 비주얼
            Transform current = contentFolder.transform.GetChild(listNum - 1);
            Image currentBg = current.GetComponent<Image>();
            currentBg.color = currentBg.color.SetAlpha(0.4f);

            Image currentLeftArrow = current.Find("LeftArrow").GetComponent<Image>();
            Image currentRightArrow = current.Find("RightArrow").GetComponent<Image>();
            currentLeftArrow.color = currentLeftArrow.color.SetAlpha(1f);
            currentRightArrow.color = currentRightArrow.color.SetAlpha(1f);

            // 필요 시 현재 항목의 데이터 참조
            SettingObjectComponent settingObjectComponent = current.GetComponent<SettingObjectComponent>();
            // TODO: settingObjectComponent를 이용한 추가 로직
        }
    }

    private IEnumerator SetSong(int index)
    {
        canvas.transform.localScale = Vector3.one;

        Transform T = contentFolder.transform;

        float elapsedTime = 0f;
        Vector3 startPos = new Vector3(T.position.x, T.position.y, 0f);
        float duration = 0.15f;
        Vector3 targetPos = new Vector3(
            originX,
            originY - originY + (defaultSettingPrefab.GetComponent<RectTransform>().sizeDelta.y * index),
            0f
        );

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
    }

    private IEnumerator RepeatKeyPress(string actionName)
    {
        // 초기 딜레이
        yield return new WaitForSeconds(0.3f);

        while (isHold)
        {
            if (actionName == "Up")
            {
                SetList(listNum - 1);
            }
            else if (actionName == "Down")
            {
                SetList(listNum + 1);
            }
            yield return new WaitForSeconds(0.05f);
        }
    }
}