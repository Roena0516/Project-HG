using System.Collections.Generic;
using UnityEngine;

public class SettingsList : MonoBehaviour
{
    public List<SettingComponent> settingsList;
    public List<string> categories;
    public SettingsListMover settingsListMover;
    private LocalizationManager _localizationManager;

    private void Awake()
    {
        _localizationManager = LocalizationManager.Instance;
        categories = new()
        {
            "DISPLAY",
            "GAME",
            "SOUND",
            "ACCOUNT"
        };

        settingsList = new()
        {
            new SettingComponent()
            {
                title = _localizationManager.GetText("display_mode"),
                value = new()
                {
                    _localizationManager.GetText("fullscreen_window"),
                    _localizationManager.GetText("fullscreen"),
                    _localizationManager.GetText("window")
                },
                initialIndex = 0,
                category = categories[0]
            },
            new SettingComponent()
            {
                title = _localizationManager.GetText("display_resolution"),
                value = new()
                {
                    "1600 X 900",
                    "1920 X 1080",
                    "2560 X 1440"
                },
                initialIndex = 1,
                category = categories[0]
            },
            new SettingComponent()
            {
                title = _localizationManager.GetText("frame_limit"),
                value = new()
            {
                "30",
                "60",
                "144",
                "165",
                "Unlimited"
            },
                initialIndex = 4,
                category = categories[0]
            }
        };

        settingsListMover.SettingsListSetter();
    }
}
