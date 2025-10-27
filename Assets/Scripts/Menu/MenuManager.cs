using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class MenuManager : MonoBehaviour
{
    private bool isSet;

    public GameObject menuFolder;

    private SettingsManager settingsManager;
    public CircleMenuController menuController;
    [SerializeField] private SettingsListMover settingsListMover;
    [SerializeField] private MenuAnimation _animator;
    [SerializeField] private GetUser getUser;

    public GameObject settingsPanel;

    public int menuCount;

    public MainInputAction action;

    public TextMeshProUGUI musicDelayValue;

    public List<TextMeshProUGUI> RaneButtonText;

    public int settedButtonInputRane;

    public Toggle setAutoPlayToggle;

    public List<InputAction> LineActions;

    private float sync;

    private string baseUrl = "https://prod.windeath44.wiki/api";

    [SerializeField] private TextMeshProUGUI ratingText;
    [SerializeField] private TextMeshProUGUI ratingShadowText;
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private TextMeshProUGUI playerNameShadowText;

    private void Awake()
    {
        action = new MainInputAction();

        LineActions.Add(action.Temp.Line1Action);
        LineActions.Add(action.Temp.Line2Action);
        LineActions.Add(action.Temp.Line3Action);
        LineActions.Add(action.Temp.Line4Action);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            SelectMenu(menuController.currentIndex + 1);
        }
    }

    private void SetPlayerInfos()
    {
        Player player = settingsManager.GetPlayerData();

        if (player == null)
        {
            Debug.LogWarning("[MenuManager] Cannot set player info, player data is null");
            ratingText.text = "0.000";
            ratingShadowText.text = "0.000";
            playerNameText.text = "Player";
            playerNameShadowText.text = "Player";
            return;
        }

        ratingText.text = $"{player.rating:F3}";
        ratingShadowText.text = $"{player.rating:F3}";
        playerNameText.text = player.playerName;
        playerNameShadowText.text = player.playerName;
    }

    private async void RefreshPlayerRating()
    {
        // playerData가 설정될 때까지 대기 (최대 5초)
        float waitTime = 0f;
        Player player = null;

        while (waitTime < 5f)
        {
            player = settingsManager.GetPlayerData();
            if (player != null)
            {
                break;
            }

            Debug.Log($"[MenuManager] Waiting for player data... ({waitTime:F1}s)");
            await System.Threading.Tasks.Task.Delay(100); // 100ms 대기
            waitTime += 0.1f;
        }

        if (player == null)
        {
            Debug.LogError("[MenuManager] Player data not available after waiting 5 seconds");
            return;
        }

        if (string.IsNullOrEmpty(player.accessToken))
        {
            Debug.LogWarning("[MenuManager] No access token available");
            return;
        }

        Debug.Log("[MenuManager] Fetching latest rating...");

        // 사용자 프로필 갱신
        UserProfileResponse userProfile = await getUser.GetUserProfileAPI(baseUrl, player.accessToken, onSuccess: (res) =>
        {
            Debug.Log($"[MenuManager] User profile refreshed: userId={res.data.userId}, name={res.data.name}");
        }, onError: (err) =>
        {
            Debug.LogWarning($"[MenuManager] Failed to refresh user profile: {err}");
        });

        // 레이팅 갱신
        GetMyRatingResponse myRating = await getUser.GetUserRatingAPI(baseUrl, player.accessToken, onSuccess: (res) =>
        {
            Debug.Log($"[MenuManager] Rating refreshed: {res.data.rating}, ranking: {res.data.ranking}");
        }, onError: (err) =>
        {
            Debug.LogWarning($"[MenuManager] Failed to refresh rating: {err}");
        });

        // Player 데이터 업데이트
        if (userProfile != null || myRating != null)
        {
            Player updatedPlayer = new()
            {
                id = userProfile?.userId ?? player.id,
                accessToken = player.accessToken,
                refreshToken = player.refreshToken,
                playerName = userProfile?.name ?? player.playerName,
                rating = myRating?.rating ?? player.rating,
                ranking = myRating?.ranking ?? player.ranking,
                createdAt = myRating?.createdAt ?? player.createdAt,
                updatedAt = myRating?.updatedAt ?? player.updatedAt
            };

            settingsManager.SetPlayerData(updatedPlayer);

            // UI 갱신
            SetPlayerInfos();

            Debug.Log($"[MenuManager] Player data updated: rating={updatedPlayer.rating:F3}, ranking={updatedPlayer.ranking}");
        }
    }

    private void SelectMenu(int index)
    {
        if (index == 2)
        {
            if (isSet)
            {
                isSet = false;
                _animator.FadeIn(onComplete: () => {
                    SceneManager.LoadSceneAsync("FreePlay");
                 });
            }
        }
        if (index == 1)
        {
            Debug.Log("Settings");
            if (isSet)
            {
                isSet = false;

                _animator.FadeIn(onComplete: () => {
                    SceneManager.LoadSceneAsync("Settings");
                });
            }
        }
        if (index == 3)
        {
            if (isSet)
            {
                isSet = false;
                //settingsManager.SetFileName($"{Application.streamingAssetsPath}/system/SyncRoom-Level.roena");

                _animator.FadeIn(onComplete: () => {
                    SceneManager.LoadSceneAsync("Rankings");
                });
            }
        }
    }

    private void SetSettingsPanel()
    {
        settingsPanel.SetActive(true);
        musicDelayValue.text = $"{settingsManager.settings.sync}ms";

        for (int i = 0; i < 4; i++)
        {
            RaneButtonText[i].text = $"{settingsManager.LineActions[i].bindings[0].ToDisplayString()}";
        }
    }

    public void ChangeSync(float duration)
    {
        sync += duration;
        musicDelayValue.text = $"{sync}ms";
    }

    public void SetKeyBindsInput(int rane)
    {
        Debug.Log(rane);
        settedButtonInputRane = rane;
        Rebind(settedButtonInputRane);
    }

    private void Rebind(int rane)
    {
        LineActions[rane - 1].Disable();
        LineActions[rane - 1].PerformInteractiveRebinding()
        .WithControlsExcluding("Mouse")
        .OnComplete(operation => // 리바인딩 완료 시 실행
                {
            Debug.Log($"{LineActions[rane - 1].bindings[0].effectivePath}");
            operation.Dispose(); // 메모리 해제
                    LineActions[rane - 1].Enable(); // 다시 활성화
                    RaneButtonText[rane - 1].text = $"{LineActions[rane - 1].bindings[0].ToDisplayString()}";
        })
        .Start(); // 리바인딩 시작
        settedButtonInputRane = 0;
    }

    public void SetAutoPlay(bool setted)
    {
        settingsManager.isAutoPlay = setted;
    }

    //public void SetToKR(bool setted)
    //{
    //    settingsManager.isKR = setted;
    //}

    public void SaveSettingsData()
    {
        for (int i = 0; i < 4; i++)
        {
            settingsManager.LineActions[i].ApplyBindingOverride(LineActions[i].bindings[0].effectivePath);
        }

        settingsManager.SetKeyBinds(new()
        {
#if UNITY_STANDALONE_OSX
            $"{LineActions[0].bindings[0].effectivePath}",
            $"{LineActions[1].bindings[0].effectivePath}",
            $"{LineActions[2].bindings[0].effectivePath}",
            $"{LineActions[3].bindings[0].effectivePath}"
#elif UNITY_STANDALONE_WIN
            $"{LineActions[0].bindings[0].ToDisplayString()}",
            $"{LineActions[1].bindings[0].ToDisplayString()}",
            $"{LineActions[2].bindings[0].ToDisplayString()}",
            $"{LineActions[3].bindings[0].ToDisplayString()}"
#endif
        });

        settingsManager.SetSync($"{sync}");
        //settingsManager.SetToKR(settingsManager.isKR);

        settingsManager.SaveSettings();
    }

    private void Start()
    {
        isSet = true;
        settedButtonInputRane = 0;
        menuCount = menuFolder.transform.childCount;

        settingsManager = SettingsManager.Instance;

        // 먼저 저장된 정보 표시
        SetPlayerInfos();

        // 최신 레이팅 가져오기
        RefreshPlayerRating();

        sync = settingsManager.settings.sync;

        //setAutoPlayToggle.enabled = settingsManager.isAutoPlay;

        LineActions[0].ApplyBindingOverride(settingsManager.settings.KeyBinds[0]);
        LineActions[1].ApplyBindingOverride(settingsManager.settings.KeyBinds[1]);
        LineActions[2].ApplyBindingOverride(settingsManager.settings.KeyBinds[2]);
        LineActions[3].ApplyBindingOverride(settingsManager.settings.KeyBinds[3]);
    }
}
