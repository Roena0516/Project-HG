using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsListMover : MonoBehaviour
{
    //private SettingsManager settings;
    [SerializeField] private SettingsList settingsListComponent;
    [SerializeField] private SettingsAnimation _animator;

    [SerializeField] private GameObject canvas;
    [SerializeField] private GameObject contentFolder;
    [SerializeField] private GameObject defaultSettingPrefab;
    [SerializeField] private GameObject _categoryContentFolder;
    [SerializeField] private GameObject _categoryPrefab;
    [SerializeField] private GameObject _selectedCategory;
    [SerializeField] private TextMeshProUGUI _categoryTitle;

    private int listNum = 1;
    private float originX;
    private float originY;
    private bool isHold;

    private List<SettingComponent> settingsList;
    private int currentCategoryIndex = 0;
    private float categoryOriginX;

    private Coroutine currentSetSongRoutine;
    private Coroutine repeatCoroutine;

    // 설정 백업 (ESC로 취소 시 복원용)
    private Dictionary<string, int> settingsBackup;

    private void Start()
    {
        canvas.transform.localScale = Vector3.one;

        //settings = SettingsManager.Instance;

        originX = contentFolder.transform.position.x;
        originY = contentFolder.transform.position.y;
        categoryOriginX = _selectedCategory.transform.localPosition.x;

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

        // 설정 값 변경
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            ChangeSettingValue(-1);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            ChangeSettingValue(1);
        }

        // 카테고리 전환
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            ChangeCategory(-1);
        }
        if (Input.GetKeyDown(KeyCode.RightShift))
        {
            ChangeCategory(1);
        }

        // 저장하고 나가기
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            Save();
        }

        // 저장하지 않고 나가기
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CancelAndExit();
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

        // 현재 설정 값 백업
        BackupSettings();

        // 카테고리 프리팹 생성
        CategoryListSetter();

        // 첫 번째 카테고리로 시작
        RefreshSettingsList();

        _animator.FadeOut();
        _animator.SetPanels();
    }

    private void CategoryListSetter()
    {
        List<string> categories = settingsListComponent.categories;

        foreach (string category in categories)
        {
            // 카테고리 프리팹 인스턴스화
            GameObject categoryObject = Instantiate(_categoryPrefab, _categoryContentFolder.transform);

            // Text 자식 찾기
            Transform textTransform = categoryObject.transform.Find("Text");

            if (textTransform != null)
            {
                // TextMeshProUGUI 컴포넌트에 카테고리 이름 설정
                TextMeshProUGUI textComponent = textTransform.GetComponent<TextMeshProUGUI>();
                if (textComponent != null)
                {
                    textComponent.text = category;
                }
            }
        }

        _categoryTitle.text = categories[currentCategoryIndex];
    }

    private void SetList(int toIndex)
    {
        if (toIndex > 0 && toIndex <= contentFolder.transform.childCount - 1)
        {
            int prev = listNum;
            listNum = toIndex;

            if (listNum > 3 && listNum < contentFolder.transform.childCount - 5)
            {
                if (currentSetSongRoutine != null)
                {
                    StopCoroutine(currentSetSongRoutine);
                }
                currentSetSongRoutine = StartCoroutine(SetSong(listNum));
            }

            // 이전 선택 항목 비활성 비주얼
            Image prevImage = contentFolder.transform.GetChild(prev).GetComponent<Image>();
            prevImage.color = prevImage.color.SetAlpha(0f);

            Transform prevRight = prevImage.gameObject.transform.Find("Right");
            Image prevLeftArrow = prevRight.Find("LeftArrow").GetComponent<Image>();
            Image prevRightArrow = prevRight.Find("RightArrow").GetComponent<Image>();
            prevLeftArrow.color = prevLeftArrow.color.SetAlpha(0f);
            prevRightArrow.color = prevRightArrow.color.SetAlpha(0f);

            // 현재 선택 항목 활성 비주얼
            Transform current = contentFolder.transform.GetChild(listNum);
            Image currentBg = current.GetComponent<Image>();
            currentBg.color = currentBg.color.SetAlpha(0.4f);

            Transform currentRight = current.gameObject.transform.Find("Right");
            Image currentLeftArrow = currentRight.Find("LeftArrow").GetComponent<Image>();
            Image currentRightArrow = currentRight.Find("RightArrow").GetComponent<Image>();
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

    private void ChangeSettingValue(int direction)
    {
        if (listNum <= 0 || listNum >= contentFolder.transform.childCount)
        {
            return;
        }

        // 현재 선택된 설정 아이템 가져오기
        Transform currentItem = contentFolder.transform.GetChild(listNum);
        SettingObjectComponent settingObjectComponent = currentItem.GetComponent<SettingObjectComponent>();

        if (settingObjectComponent == null)
        {
            return;
        }

        // 현재 인덱스 변경
        int currentIndex = settingObjectComponent.data.initialIndex;
        int newIndex = currentIndex + direction;

        // 범위 체크
        if (newIndex < 0 || newIndex >= settingObjectComponent.data.value.Count)
        {
            return;
        }

        // 인덱스 업데이트
        settingObjectComponent.data.initialIndex = newIndex;

        // UI 업데이트
        Transform right = currentItem.Find("Right");
        Transform settingValue = right.Find("Title");
        settingValue.GetComponent<TextMeshProUGUI>().text = settingObjectComponent.data.value[newIndex];

        // SettingsManager의 메모리에만 임시 저장 (파일에는 저장하지 않음)
        SettingsManager settingsManager = SettingsManager.Instance;
        if (settingsManager != null)
        {
            settingsManager.UpdateSettingValueWithoutSave(settingObjectComponent.data.title, newIndex);
        }
    }

    private void ChangeCategory(int direction)
    {
        List<string> categories = settingsListComponent.categories;

        // 카테고리 인덱스 변경
        currentCategoryIndex += direction;

        // 범위 제한
        if (currentCategoryIndex < 0)
        {
            currentCategoryIndex = 0;
            return;
        }
        if (currentCategoryIndex >= categories.Count)
        {
            currentCategoryIndex = categories.Count - 1;
            return;
        }

        // _selectedCategory 이동 (DOTween 사용) - 절대 위치 계산
        Vector3 currentPos = _selectedCategory.transform.localPosition;
        Vector3 targetPos = new Vector3(
            categoryOriginX + (currentCategoryIndex * 164f),
            currentPos.y,
            currentPos.z
        );

        _categoryTitle.text = categories[currentCategoryIndex];

        _selectedCategory.transform.DOLocalMove(targetPos, 0.2f).SetEase(Ease.OutSine);

        // 설정 리스트 새로고침
        RefreshSettingsList();
    }

    private void BackupSettings()
    {
        settingsBackup = new Dictionary<string, int>();
        SettingsManager settingsManager = SettingsManager.Instance;

        if (settingsManager == null) return;

        // 모든 설정 값 백업
        foreach (SettingComponent item in settingsList)
        {
            int currentValue = settingsManager.GetSettingValue(item.title);
            settingsBackup[item.title] = currentValue;
        }
    }

    private void Save()
    {
        // SettingsManager에 저장
        SettingsManager settingsManager = SettingsManager.Instance;
        if (settingsManager != null)
        {
            settingsManager.SaveSettings();

            // 백업 업데이트 - 저장된 값으로 백업을 갱신
            // 이렇게 하면 저장 후 ESC로 나갈 때 저장된 시점의 값으로 복원됨
            BackupSettings();
        }

        Debug.Log("Setting is successfully saved");
    }

    private void CancelAndExit()
    {
        // 백업된 값으로 복원
        SettingsManager settingsManager = SettingsManager.Instance;
        if (settingsManager != null && settingsBackup != null)
        {
            foreach (var kvp in settingsBackup)
            {
                settingsManager.UpdateSettingValueWithoutSave(kvp.Key, kvp.Value);
            }
        }

        // 설정 화면 닫기
        _animator.FadeIn(onComplete: () =>
        {
            SceneManager.LoadSceneAsync("Menu");
        });
    }

    private void RefreshSettingsList()
    {
        // 기존 설정 아이템 모두 제거 (Top은 제외)
        List<GameObject> toDestroy = new List<GameObject>();
        foreach (Transform child in contentFolder.transform)
        {
            if (child.gameObject.name != "Top")
            {
                toDestroy.Add(child.gameObject);
            }
        }
        foreach (GameObject obj in toDestroy)
        {
            DestroyImmediate(obj);
        }

        // 현재 카테고리 가져오기
        string currentCategory = settingsListComponent.categories[currentCategoryIndex];

        // SettingsManager에서 저장된 값 가져오기
        SettingsManager settingsManager = SettingsManager.Instance;

        // 현재 카테고리에 해당하는 설정만 추가
        foreach (SettingComponent item in settingsList)
        {
            if (item.category == currentCategory)
            {
                // SettingsManager에서 저장된 인덱스 가져오기
                int savedIndex = item.initialIndex;
                if (settingsManager != null)
                {
                    savedIndex = settingsManager.GetSettingValue(item.title);
                }

                // add setting into list
                GameObject settingObject = Instantiate(defaultSettingPrefab, contentFolder.transform);
                Transform left = settingObject.transform.Find("Left");
                Transform settingTitle = left.Find("Title");
                settingTitle.GetComponent<TextMeshProUGUI>().text = item.title;

                // set setting's value
                Transform right = settingObject.transform.Find("Right");
                Transform settingValue = right.Find("Title");
                settingValue.GetComponent<TextMeshProUGUI>().text = item.value[savedIndex];

                SettingObjectComponent settingObjectComponent = settingObject.GetComponent<SettingObjectComponent>();

                settingObjectComponent.data.title = item.title;
                settingObjectComponent.data.value = item.value;
                settingObjectComponent.data.initialIndex = savedIndex;
                settingObjectComponent.data.category = item.category;
            }
        }

        // 리스트 초기화
        listNum = 1;
        SetList(1);
    }
}