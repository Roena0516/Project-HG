using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SettingsManager : MonoBehaviour
{
    public float sync;
    public float speed;

    public string eventName;

    public string fileName;

    public MainInputAction action;

    public List<InputAction> LineActions;

    private void Awake()
    {
        action = new MainInputAction();
        LineActions.Add(action.Player.Line1Action);
        LineActions.Add(action.Player.Line2Action);
        LineActions.Add(action.Player.Line3Action);
        LineActions.Add(action.Player.Line4Action);
    }

    [System.Obsolete]
    private void OnEnable()
    {
        for (int i = 0; i < 4; i++)
        {
            LineActions[i].Enable();
        }
    }

    [System.Obsolete]
    private void OnDisable()
    {
        for (int i = 0; i < 4; i++)
        {
            LineActions[i].Disable();
        }
    }

    public void SetFileName(string inputed)
    {
        fileName = inputed;
    }

    public void SetSync(string inputed)
    {
        float.TryParse(inputed, out sync);
    }

    public void SetSpeed(string inputed)
    {
        float.TryParse(inputed, out speed);
    }
}
