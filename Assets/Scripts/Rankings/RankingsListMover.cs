using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RankingsListMover : MonoBehaviour
{
    [SerializeField] private GameObject _canvas;
    [SerializeField] private GameObject _contentFolder;
    [SerializeField] private GameObject _defaultRankingPrefab;
    [SerializeField] private RankingAPI _rankingAPI;
    [SerializeField] private TextMeshProUGUI _loadingText;

    [SerializeField] private RankingAnimation _animator;

    private int _listNum = 1;
    private float _originX;
    private float _originY;
    private bool _isHold;

    private List<PlayerResponse> _rankingsList;
    private long? _lastCursorRanking;
    private bool _hasNext;
    private bool _isLoading;

    private Coroutine _currentSetRankingRoutine;
    private Coroutine _repeatCoroutine;

    private SettingsManager _settings;

    private string baseUrl = "https://prod.windeath44.wiki/api";

    private void Start()
    {
        _canvas.transform.localScale = Vector3.one;

        _settings = SettingsManager.Instance;

        _originX = _contentFolder.transform.position.x;
        _originY = _contentFolder.transform.position.y;

        _listNum = 1;
        _rankingsList = new List<PlayerResponse>();
        _hasNext = true;
        _isLoading = false;

        LoadRankings();
    }

    private void Update()
    {
        if (_settings.IsBlocked || _isLoading) return;

        // 한 번 눌렀을 때
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            SetList(_listNum - 1);
            BeginRepeat("Up");
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            SetList(_listNum + 1);
            BeginRepeat("Down");
        }

        // 나가기
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _animator.FadeIn(onComplete: () =>
            {
                SceneManager.LoadSceneAsync("Menu");
            });
        }

        // 뗐을 때 반복 중지
        if (Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.DownArrow))
        {
            EndRepeat();
        }
    }

    private void BeginRepeat(string dir)
    {
        if (_isHold) return;
        _isHold = true;

        if (_repeatCoroutine != null)
        {
            StopCoroutine(_repeatCoroutine);
        }
        _repeatCoroutine = StartCoroutine(RepeatKeyPress(dir));
    }

    private void EndRepeat()
    {
        _isHold = false;
        if (_repeatCoroutine != null)
        {
            StopCoroutine(_repeatCoroutine);
            _repeatCoroutine = null;
        }
    }

    public async void LoadRankings()
    {
        _isLoading = true;

        if (_loadingText != null)
        {
            _loadingText.gameObject.SetActive(true);
            _loadingText.text = "Loading Rankings...";
        }

        CursorPagePlayerResponse response = await _rankingAPI.GetRankingsAPI(
            baseUrl,
            50, // 50개씩 로드
            _lastCursorRanking,
            onSuccess: (res) =>
            {
                Debug.Log($"[RankingsListMover] Loaded {res.data.values.Count} rankings");
            },
            onError: (err) =>
            {
                Debug.LogError($"[RankingsListMover] Failed to load rankings: {err}");
            }
        );

        _isLoading = false;

        if (_loadingText != null)
        {
            _loadingText.gameObject.SetActive(false);
        }

        if (response != null && response.values != null)
        {
            _rankingsList.AddRange(response.values);
            _hasNext = response.hasNext;

            if (response.values.Count > 0)
            {
                _lastCursorRanking = response.values[response.values.Count - 1].ranking;
            }

            RefreshRankingsList();
        }

        _animator.FadeOut();
    }

    private void RefreshRankingsList()
    {
        // 기존 랭킹 아이템 모두 제거 (Top은 제외)
        List<GameObject> toDestroy = new List<GameObject>();
        foreach (Transform child in _contentFolder.transform)
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

        // 랭킹 아이템 생성
        foreach (PlayerResponse playerData in _rankingsList)
        {
            GameObject rankingObject = Instantiate(_defaultRankingPrefab, _contentFolder.transform);

            // 순위
            Transform left = rankingObject.transform.Find("Left");
            if (left != null)
            {
                Transform rankingText = left.Find("Ranking");
                if (rankingText != null)
                {
                    rankingText.GetComponent<TextMeshProUGUI>().text = $"#{playerData.ranking}";
                }
            }

            // 플레이어 이름
            Transform center = rankingObject.transform.Find("Center");
            if (center != null)
            {
                Transform playerNameText = center.Find("PlayerName");
                if (playerNameText != null)
                {
                    playerNameText.GetComponent<TextMeshProUGUI>().text = playerData.playerId;
                }
            }

            // 레이팅
            Transform right = rankingObject.transform.Find("Right");
            if (right != null)
            {
                Transform ratingText = right.Find("Rating");
                if (ratingText != null)
                {
                    ratingText.GetComponent<TextMeshProUGUI>().text = $"{playerData.rating:F3}";
                }
            }

            // 컴포넌트에 데이터 저장
            RankingItemComponent itemComponent = rankingObject.GetComponent<RankingItemComponent>();
            if (itemComponent != null)
            {
                itemComponent.playerData = playerData;
            }
        }

        // 리스트 초기화
        _listNum = 1;
        SetList(1);
    }

    private void SetList(int toIndex)
    {
        int totalItems = _contentFolder.transform.childCount - 1; // Top 제외

        if (toIndex > 0 && toIndex <= totalItems)
        {
            int prev = _listNum;
            _listNum = toIndex;

            // 리스트 끝에 도달하고 더 불러올 데이터가 있으면 로드
            if (_listNum >= totalItems - 3 && _hasNext && !_isLoading)
            {
                LoadRankings();
            }

            // 스크롤 필요 여부 확인
            if (_listNum > 3 && _listNum < totalItems - 2)
            {
                if (_currentSetRankingRoutine != null)
                {
                    StopCoroutine(_currentSetRankingRoutine);
                }
                _currentSetRankingRoutine = StartCoroutine(SetRanking(_listNum));
            }

            // 이전 선택 항목 비활성 비주얼
            if (prev > 0 && prev <= _contentFolder.transform.childCount - 1)
            {
                Transform prevTransform = _contentFolder.transform.GetChild(prev);
                Image prevImage = prevTransform.GetComponent<Image>();
                if (prevImage != null)
                {
                    prevImage.color = prevImage.color.SetAlpha(0f);
                }
            }

            // 현재 선택 항목 활성 비주얼
            Transform current = _contentFolder.transform.GetChild(_listNum);
            Image currentBg = current.GetComponent<Image>();
            if (currentBg != null)
            {
                currentBg.color = currentBg.color.SetAlpha(0.4f);
            }
        }
    }

    private IEnumerator SetRanking(int index)
    {
        _canvas.transform.localScale = Vector3.one;

        Transform T = _contentFolder.transform;

        float elapsedTime = 0f;
        Vector3 startPos = new Vector3(T.position.x, T.position.y, 0f);
        float duration = 0.15f;
        Vector3 targetPos = new Vector3(
            _originX,
            _originY - _originY + (_defaultRankingPrefab.GetComponent<RectTransform>().sizeDelta.y * index),
            0f
        );

        while (elapsedTime < duration)
        {
            _canvas.transform.localScale = Vector3.one;

            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            float easedT = Mathf.Sin(t * Mathf.PI * 0.5f);

            T.position = Vector3.Lerp(startPos, targetPos, easedT);

            yield return null;
        }

        _canvas.transform.localScale = Vector3.one;
        T.position = targetPos;

        _currentSetRankingRoutine = null;
    }

    private IEnumerator RepeatKeyPress(string actionName)
    {
        // 초기 딜레이
        yield return new WaitForSeconds(0.3f);

        while (_isHold)
        {
            if (actionName == "Up")
            {
                SetList(_listNum - 1);
            }
            else if (actionName == "Down")
            {
                SetList(_listNum + 1);
            }
            yield return new WaitForSeconds(0.05f);
        }
    }
}
