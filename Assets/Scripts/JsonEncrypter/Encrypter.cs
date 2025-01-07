using System.IO;
using UnityEngine;

public class Encrypter : MonoBehaviour
{
    [System.Obsolete]
    private void Start()
    {
        LoadFromJson(Path.Combine(Application.streamingAssetsPath, "n-buna-갯나리 해저담.json"));
    }

    public void LoadFromJson(string filePath)
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);

            string encrypted = EncryptionHelper.Encrypt(json);

            File.WriteAllText(filePath, encrypted);
            Debug.Log("Chart saved to: " + filePath);
        }
        else
        {
            Debug.LogError("File not found at: " + filePath);
        }
    }
}
