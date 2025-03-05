using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

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

    private void Awake()
    {
        action = new MainInputAction();
        listUp = action.Menu.ListUp;
        listDown = action.Menu.ListDown;
        menuSelect = action.Menu.MenuSelect;
        exit = action.Menu.Exit;
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

    [System.Obsolete]
    private void SetMenu(int toIndex)
    {
        if (toIndex > 0 && toIndex <= menuCount)
        {
            selectedMenu = toIndex;
            Transform freePlayTransform = menuFolder.transform.FindChild("FreePlay");
            Transform settingsTransform = menuFolder.transform.FindChild("Settings");

            TextMeshProUGUI freePlay = freePlayTransform.gameObject.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI settings = settingsTransform.gameObject.GetComponent<TextMeshProUGUI>();

            freePlay.color = Color.white;
            settings.color = Color.white;

            if (selectedMenu == 1)
            {
                freePlay.color = Color.yellow;
            }
            if (selectedMenu == 2)
            {
                settings.color = Color.yellow;
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
                SceneManager.LoadScene("FreePlay");
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
    }

    private void SetSettingsPanel()
    {
        settingsPanel.SetActive(true);
        musicDelayValue.text = $"{settingsManager.sync}";

        RaneButtonText[0].text = $"{settingsManager.Line1Action.bindings[0].ToDisplayString()}";
        RaneButtonText[1].text = $"{settingsManager.Line2Action.bindings[0].ToDisplayString()}";
        RaneButtonText[2].text = $"{settingsManager.Line3Action.bindings[0].ToDisplayString()}";
        RaneButtonText[3].text = $"{settingsManager.Line4Action.bindings[0].ToDisplayString()}";
    }

    public void SetKeyBindsInput(int rane)
    {
        Debug.Log(rane);
        settedButtonInputRane = rane;
        Rebind(settedButtonInputRane);
    }

    private void Rebind(int rane)
    {
        switch(rane)
        {
            case 1:
                settingsManager.Line1Action.Disable();
                settingsManager.Line1Action.PerformInteractiveRebinding()
                .WithControlsExcluding("Mouse")
                .OnComplete(operation => // 리바인딩 완료 시 실행
                {
                    Debug.Log($"{settingsManager.Line1Action.bindings[0].effectivePath}");
                    operation.Dispose(); // 메모리 해제
                    settingsManager.Line1Action.Enable(); // 다시 활성화
                })
                .Start(); // 리바인딩 시작
                settedButtonInputRane = 0;
                break;
            case 2:
                settingsManager.Line2Action.Disable();
                settingsManager.Line2Action.PerformInteractiveRebinding()
                .WithControlsExcluding("Mouse")
                .OnComplete(operation =>
                {
                    Debug.Log($"{settingsManager.Line2Action.bindings[0].effectivePath}");
                    operation.Dispose();
                    settingsManager.Line2Action.Enable();
                })
                .Start();
                settedButtonInputRane = 0;
                break;
            case 3:
                settingsManager.Line3Action.Disable();
                settingsManager.Line3Action.PerformInteractiveRebinding()
                .WithControlsExcluding("Mouse")
                .OnComplete(operation =>
                {
                    Debug.Log($"{settingsManager.Line3Action.bindings[0].effectivePath}");
                    operation.Dispose();
                    settingsManager.Line3Action.Enable();
                })
                .Start();
                settedButtonInputRane = 0;
                break;
            case 4:
                settingsManager.Line4Action.Disable();
                settingsManager.Line4Action.PerformInteractiveRebinding()
                .WithControlsExcluding("Mouse")
                .OnComplete(operation =>
                {
                    Debug.Log($"{settingsManager.Line4Action.bindings[0].effectivePath}");
                    operation.Dispose();
                    settingsManager.Line4Action.Enable();
                })
                .Start();
                settedButtonInputRane = 0;
                break;
        }
    }

    [System.Obsolete]
    private void Start()
    {
        isSet = true;
        settedButtonInputRane = 0;

        selectedMenu = 1;
        menuCount = menuFolder.transform.childCount;

        settingsManager = FindObjectOfType<SettingsManager>();
        
        SetMenu(1);
    }
}
