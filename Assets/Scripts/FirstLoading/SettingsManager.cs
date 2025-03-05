using UnityEngine;
using UnityEngine.InputSystem;

public class SettingsManager : MonoBehaviour
{
    public float sync;
    public float speed;

    public string eventName;

    public string fileName;

    public MainInputAction action;

    public InputAction Line1Action;
    public InputAction Line2Action;
    public InputAction Line3Action;
    public InputAction Line4Action;

    private void Awake()
    {
        action = new MainInputAction();

        Line1Action = action.Player.Line1Action;
        Line2Action = action.Player.Line2Action;
        Line3Action = action.Player.Line3Action;
        Line4Action = action.Player.Line4Action;
    }

    [System.Obsolete]
    private void OnEnable()
    {
        Line1Action.Enable();
        Line2Action.Enable();
        Line3Action.Enable();
        Line4Action.Enable();
    }

    [System.Obsolete]
    private void OnDisable()
    {
        Line1Action.Disable();
        Line2Action.Disable();
        Line3Action.Disable();
        Line4Action.Disable();
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
