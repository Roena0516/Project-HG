using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelText;

    [SerializeField] private LoadManager loadManager;

    private void Start()
    {
        if (!loadManager)
        {
            Debug.LogError("loadManager not defined");
            return;
        }

        SetUIs();
    }

    private void SetUIs()
    {
        float level = loadManager.info.level != 0 ? loadManager.info.level : 0;

        SetLevelText(level);
    }

    private void SetLevelText(float level)
    {
        levelText.text = $"{level}";
    }
}
