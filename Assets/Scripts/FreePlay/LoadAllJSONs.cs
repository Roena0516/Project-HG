using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class LoadAllJSONs : MonoBehaviour
{
    public List<SongInfoClass> songList = new List<SongInfoClass>();

    private SongInfoClass tempSongInfoClass;

    private SongListShower shower;

    [System.Obsolete]
    private void Start()
    {
        shower = FindObjectOfType<SongListShower>();

        string[] jsonFiles = Directory.GetFiles(Application.streamingAssetsPath, "*.roena");

        foreach (string filePath in jsonFiles)
        {
            string json = File.ReadAllText(filePath);

            string decrypted = EncryptionHelper.Decrypt(json);

            SongContainer container = JsonUtility.FromJson<SongContainer>(decrypted);

            tempSongInfoClass = container.info;

            songList.Add(tempSongInfoClass);
        }

        shower.Shower();
    }

    [System.Serializable]
    private class SongContainer
    {
        public SongInfoClass info;

    }
}
