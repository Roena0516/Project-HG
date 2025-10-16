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
            },
            new SettingComponent()
            {
                title = _localizationManager.GetText("default_language"),
                value = new()
                {
                    _localizationManager.GetText("ko"),
                    _localizationManager.GetText("en"),
                    _localizationManager.GetText("jp")
                },
                initialIndex = 0,
                category = categories[1]
            },
            new SettingComponent()
            {
                title = _localizationManager.GetText("song_info_language"),
                value = new()
                {
                    _localizationManager.GetText("ko"),
                    _localizationManager.GetText("jp")
                },
                initialIndex = 0,
                category = categories[1]
            },
            new SettingComponent()
            {
                title = _localizationManager.GetText("judgement_line_height"),
                value = new()
                {
                    "0",
                },
                initialIndex = 0,
                category = categories[1]
            },
            new SettingComponent()
            {
                title = _localizationManager.GetText("song_output_delay"),
                value = new()
                {
                    "0",
                },
                initialIndex = 0,
                category = categories[1]
            },
            new SettingComponent()
            {
                title = _localizationManager.GetText("fast_slow_exp"),
                value = new()
                {
                    _localizationManager.GetText("off_fast_slow"),
                    _localizationManager.GetText("good_down"),
                    _localizationManager.GetText("great_down"),
                    _localizationManager.GetText("perfect_down")
                },
                initialIndex = 3,
                category = categories[1]
            },
        };

        settingsListMover.SettingsListSetter();
    }
}
