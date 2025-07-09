using System.IO;
using UnityEngine;

public class Encrypter : MonoBehaviour
{
    [System.Obsolete]
    private void Start()
    {
        LoadFromJson(Path.Combine(Application.streamingAssetsPath, "god_ish/피노키오피-신 같네.roena"));
    }

    public void LoadFromJson(string filePath)
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);

            string encrypted = EncryptionHelper.Decrypt(json);

            File.WriteAllText(Path.Combine(Application.streamingAssetsPath, "godish.json"), encrypted);
            Debug.Log("Chart saved to: " + filePath);
        }
        else
        {
            Debug.LogError("File not found at: " + filePath);
        }
    }
}
