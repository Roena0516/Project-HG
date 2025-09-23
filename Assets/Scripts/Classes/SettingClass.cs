using System;
using System.Collections.Generic;
using UnityEngine;

public class SettingClass : MonoBehaviour
{
}

public class SettingComponent
{
    public string title;
    public List<string> value;
    public int initialIndex;
    public string category;
}

public class SettingComponentsComponent
{
    public string title;
    public List<string> value;
    public string originValue;
    public int initialIndex;
    public string category;
}
