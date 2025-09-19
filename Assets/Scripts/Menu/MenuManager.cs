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

    public GameObject settingsPanel;

    public int menuCount;

    public MainInputAction action;

    public TextMeshProUGUI musicDelayValue;

    public List<TextMeshProUGUI> RaneButtonText;

    public int settedButtonInputRane;

    public Toggle setAutoPlayToggle;

    public List<InputAction> LineActions;

    private float sync;


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

        ratingText.text = $"{player.rating:F3}";
        ratingShadowText.text = $"{player.rating:F3}";
        playerNameText.text = player.playerName;
        playerNameShadowText.text = player.playerName;
    }

    private void SelectMenu(int index)
    {
        if (index == 2)
        {
            if (isSet)
            {
                isSet = false;
                SceneManager.LoadSceneAsync("FreePlay");
            }
        }
        if (index == 3)
        {
            Debug.Log("Settings");
            if (isSet)
            {
                isSet = false;
                SetSettingsPanel();
            }
        }
        if (index == 1)
        {
            if (isSet)
            {
                isSet = false;
                settingsManager.SetFileName($"{Application.streamingAssetsPath}/system/SyncRoom-Level.roena");

                SceneManager.LoadSceneAsync("SyncRoom");
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

    public void SetToKR(bool setted)
    {
        settingsManager.isKR = setted;
    }

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
        settingsManager.SetToKR(settingsManager.isKR);

        settingsManager.SaveSettings();
    }

    private void Start()
    {
        isSet = true;
        settedButtonInputRane = 0;
        menuCount = menuFolder.transform.childCount;

        settingsManager = SettingsManager.Instance;

        SetPlayerInfos();

        sync = settingsManager.settings.sync;

        //setAutoPlayToggle.enabled = settingsManager.isAutoPlay;

        LineActions[0].ApplyBindingOverride(settingsManager.settings.KeyBinds[0]);
        LineActions[1].ApplyBindingOverride(settingsManager.settings.KeyBinds[1]);
        LineActions[2].ApplyBindingOverride(settingsManager.settings.KeyBinds[2]);
        LineActions[3].ApplyBindingOverride(settingsManager.settings.KeyBinds[3]);
    }
}
