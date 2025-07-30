using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class SongList
{
    public SongInfoClass[] songs;
}

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
        string[] songList = Directory.GetFiles(Application.streamingAssetsPath, "songList.json");
        string songListJson = File.ReadAllText(songList[0]);
        SongList songListContainer = JsonUtility.FromJson<SongList>(songListJson);

        foreach (var song in songListContainer.songs)
        {
            string[] directory = Directory.GetDirectories(Application.streamingAssetsPath, song.fileLocation);

            if (directory.Length == 0)
            {
                Debug.LogWarning($"�ش��ϴ� ������ ã�� ���߽��ϴ�: {song.fileLocation}");
                continue;
            }

            string[] jsonFiles = Directory.GetFiles(directory[0], song.difficulty + ".roena");

            if (jsonFiles.Length == 0)
            {
                Debug.LogWarning($"���� ���̵��� ä�� ������ �������� �ʽ��ϴ�: {directory[0]} {song.difficulty}");
                continue;
            }

            string json = File.ReadAllText(jsonFiles[0]);
            string decrypted = EncryptionHelper.Decrypt(json);

            SongContainer container = JsonUtility.FromJson<SongContainer>(decrypted);
            tempSongInfoClass = container.info;

            tempSongInfoClass.fileLocation = jsonFiles[0];

            string key = tempSongInfoClass.artist + "-" + tempSongInfoClass.title;
            if (!songDictionary.ContainsKey(key))
            {
                songDictionary[key] = new List<SongInfoClass>();
            }
            songDictionary[key].Add(tempSongInfoClass);
        }

        shower.Shower();
    }

    [System.Serializable]
    private class SongContainer
    {
        public SongInfoClass info;

    }
}
