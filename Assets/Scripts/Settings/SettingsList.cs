using System.Collections.Generic;
using UnityEngine;

public class SettingsList : MonoBehaviour
{
    public List<SettingComponent> settingsList;

    private void Start()
    {
        settingsList = new();

        settingsList.Add(new SettingComponent()
        {
            title = "Display Mode",
            value = new()
            {
                "Fullscreen Window",
                "Fullscreen",
                "Window"
            },
            initialIndex = 0,
            category = "display"
        });

        settingsList.Add(new SettingComponent()
        {
            title = "Display Resolution",
            value = new()
            {
                "1600 X 900",
                "1920 X 1080",
                "2560 X 1440"
            },
            initialIndex = 1,
            category = "display"
        });

        settingsList.Add(new SettingComponent()
        {
            title = "Frame Limit",
            value = new()
            {
                "30",
                "60",
                "144",
                "165",
                "Unlimited"
            },
            initialIndex = 4,
            category = "display"
        });
    }
}
