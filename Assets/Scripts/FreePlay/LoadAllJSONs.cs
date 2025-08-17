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
                Debug.LogWarning($"해당하는 폴더를 찾지 못했습니다: {song.fileLocation}");
                continue;
            }

            string[] jsonFiles = Directory.GetFiles(directory[0], song.difficulty + ".roena");

            if (jsonFiles.Length == 0)
            {
                Debug.LogWarning($"다음 난이도의 채보 파일이 존재하지 않습니다: {directory[0]} {song.difficulty}");
                continue;
            }

            string json = File.ReadAllText(jsonFiles[0]);
            string decrypted = EncryptionHelper.Decrypt(json);

            SongInfoClass info = new();

            info.fileLocation = jsonFiles[0];
            info.difficulty = song.difficulty;
            info.artist = song.artist;
            info.jpArtist = song.jpArtist;
            info.title = song.title;
            info.jpTitle = song.jpTitle;
            info.bpm = song.bpm;
            info.eventName = song.eventName;
            info.level = song.level;

            string key = info.artist + "-" + info.title;
            if (!songDictionary.ContainsKey(key))
            {
                songDictionary[key] = new List<SongInfoClass>();
            }
            songDictionary[key].Add(info);
        }

        shower.Shower();
    }

    [System.Serializable]
    private class SongContainer
    {
        public SongInfoClass info;

    }
}
