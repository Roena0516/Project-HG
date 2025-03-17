using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class LoadAllJSONs : MonoBehaviour
{
    public List<SongInfoClass> songList = new List<SongInfoClass>();
    public Dictionary<string, List<SongInfoClass>> songDictionary = new Dictionary<string, List<SongInfoClass>>();

    private SongInfoClass tempSongInfoClass;

    public SongListShower shower;

    //public static LoadAllJSONs Instance { get; private set; }

    //private void Awake()
    //{
    //    if (Instance == null)
    //    {
    //        Instance = this;
    //    }
    //    else
    //    {
    //        Destroy(gameObject);
    //    }
    //}

    private void Start()
    {
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

                tempSongInfoClass.songFile = filePath;

                string key = tempSongInfoClass.artist + "-" + tempSongInfoClass.title;
                if (!songDictionary.ContainsKey(key))
                {
                    songDictionary[key] = new List<SongInfoClass>();
                }
                songDictionary[key].Add(tempSongInfoClass);

                //foreach (SongInfoClass infoss in songDictionary[key])
                //{
                //    Debug.Log(infoss.title);
                //    Debug.Log(infoss.songFile);
                //    Debug.Log(infoss.difficulty);
                //}
                //Debug.Log("----");

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
