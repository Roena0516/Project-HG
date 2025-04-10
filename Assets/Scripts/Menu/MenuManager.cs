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

    public GameObject settingsPanel;

    private int selectedMenu;
    public int menuCount;

    public MainInputAction action;
    private InputAction listUp;
    private InputAction listDown;
    private InputAction menuSelect;
    private InputAction exit;

    public TextMeshProUGUI musicDelayValue;

    public List<TextMeshProUGUI> RaneButtonText;

    public int settedButtonInputRane;

    private Transform freePlayTransform;
    private Transform settingsTransform;
    private Transform syncRoomTransform;

    private TextMeshProUGUI freePlay;
    private TextMeshProUGUI settings;
    private TextMeshProUGUI syncRoom;

    public Toggle setAutoPlayToggle;

    public List<InputAction> LineActions;

    private float sync;

    private void Awake()
    {
        action = new MainInputAction();
        listUp = action.Menu.ListUp;
        listDown = action.Menu.ListDown;
        menuSelect = action.Menu.MenuSelect;
        exit = action.Menu.Exit;

        LineActions.Add(action.Temp.Line1Action);
        LineActions.Add(action.Temp.Line2Action);
        LineActions.Add(action.Temp.Line3Action);
        LineActions.Add(action.Temp.Line4Action);
    }

    [System.Obsolete]
    private void OnEnable()
    {
        listUp.Enable();
        listUp.started += Started;

        listDown.Enable();
        listDown.started += Started;

        menuSelect.Enable();
        menuSelect.started += Started;

        exit.Enable();
        exit.started += Started;

        for (int i = 0; i < 4; i++)
        {
            LineActions[i].Enable();
        }
    }

    [System.Obsolete]
    private void OnDisable()
    {
        listUp.Disable();
        listUp.started -= Started;

        listDown.Disable();
        listDown.started -= Started;

        menuSelect.Disable();
        menuSelect.started -= Started;

        exit.Disable();
        exit.started -= Started;

        for (int i = 0; i < 4; i++)
        {
            LineActions[i].Disable();
        }
    }

    [System.Obsolete]
    void Started(InputAction.CallbackContext context)
    {
        string actionName = context.action.name;

        switch (actionName)
        {
            case "ListUp":
                SetMenu(selectedMenu + 1);
                break;
            case "ListDown":
                SetMenu(selectedMenu - 1);
                break;
            case "MenuSelect":
                SelectMenu(selectedMenu);
                break;
            case "Exit":
                Debug.Log("Exited");
                if (!isSet)
                {
                    settingsPanel.SetActive(false);
                    isSet = true;
                }
                break;
        }
    }

    private void SetMenu(int toIndex)
    {
        if (toIndex > 0 && toIndex <= menuCount)
        {
            selectedMenu = toIndex;   

            freePlay.color = Color.white;
            settings.color = Color.white;
            syncRoom.color = Color.white;

            if (selectedMenu == 1)
            {
                freePlay.color = Color.yellow;
            }
            if (selectedMenu == 2)
            {
                settings.color = Color.yellow;
            }
            if (selectedMenu == 3)
            {
                syncRoom.color = Color.yellow;
            }
        }
    }

    private void SelectMenu(int index)
    {
        if (index == 1)
        {
            if (isSet)
            {
                isSet = false;
                SceneManager.LoadSceneAsync("FreePlay");
            }
        }
        if (index == 2)
        {
            Debug.Log("Settings");
            if (isSet)
            {
                isSet = false;
                SetSettingsPanel();
            }
        }
        if (index == 3)
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

    public void SaveSettingsData()
    {
        for (int i = 0; i < 4; i++)
        {
            settingsManager.LineActions[i].ApplyBindingOverride(LineActions[i].bindings[0].effectivePath);
        }

        settingsManager.settings.KeyBinds = new()
        {
            $"{LineActions[0].bindings[0].effectivePath}",
            $"{LineActions[1].bindings[0].effectivePath}",
            $"{LineActions[2].bindings[0].effectivePath}",
            $"{LineActions[3].bindings[0].effectivePath}"
        };

        settingsManager.settings.sync = sync;

        settingsManager.SaveSettings();
    }

    private void Start()
    {
        isSet = true;
        settedButtonInputRane = 0;

        selectedMenu = 1;
        menuCount = menuFolder.transform.childCount;

        settingsManager = SettingsManager.Instance;

        sync = settingsManager.settings.sync;

        freePlayTransform = menuFolder.transform.Find("FreePlay");
        settingsTransform = menuFolder.transform.Find("Settings");
        syncRoomTransform = menuFolder.transform.Find("SyncRoom");

        freePlay = freePlayTransform.gameObject.GetComponent<TextMeshProUGUI>();
        settings = settingsTransform.gameObject.GetComponent<TextMeshProUGUI>();
        syncRoom = syncRoomTransform.gameObject.GetComponent<TextMeshProUGUI>();

        //setAutoPlayToggle.enabled = settingsManager.isAutoPlay;

        LineActions[0].ApplyBindingOverride(settingsManager.settings.KeyBinds[0]);
        LineActions[1].ApplyBindingOverride(settingsManager.settings.KeyBinds[1]);
        LineActions[2].ApplyBindingOverride(settingsManager.settings.KeyBinds[2]);
        LineActions[3].ApplyBindingOverride(settingsManager.settings.KeyBinds[3]);

        SetMenu(1);
    }
}
