using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public float sync;
    public float speed;

    public string eventName;

    public string fileName;

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
