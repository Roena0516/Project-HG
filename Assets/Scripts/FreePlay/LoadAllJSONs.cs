using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class LoadAllJSONs : MonoBehaviour
{
    public List<SongInfoClass> songList = new List<SongInfoClass>();
    public Dictionary<string, List<SongInfoClass>> songDictionary = new Dictionary<string, List<SongInfoClass>>();

    private SongInfoClass tempSongInfoClass;

    private SongListShower shower;

    [System.Obsolete]
    private void Start()
    {
        shower = FindObjectOfType<SongListShower>();

        string[] directories = Directory.GetDirectories(Application.streamingAssetsPath, "*");

        foreach (string folderPath in directories)
        {
            string[] jsonFiles = Directory.GetFiles(folderPath, "*.roena");

            foreach (string filePath in jsonFiles)
            {
                string json = File.ReadAllText(filePath);
                string decrypted = EncryptionHelper.Decrypt(json);

                SongContainer container = JsonUtility.FromJson<SongContainer>(decrypted);
                tempSongInfoClass = container.info;

                songList.Add(tempSongInfoClass);

                string key = tempSongInfoClass.artist + "-" + tempSongInfoClass.title;
                if (!songDictionary.ContainsKey(key))
                {
                    songDictionary[key] = new List<SongInfoClass>();
                }
                songDictionary[key].Add(tempSongInfoClass);
                Debug.Log(songDictionary[key][0].title);
                Debug.Log(key);
            }
        }

        shower.Shower();
    }

    [System.Serializable]
    private class SongContainer
    {
        public SongInfoClass info;

    }
}
