using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.Networking;

public class SongList
{
    public SongInfoClass[] songs;
}

public class LoadAllJSONs : MonoBehaviour
{
    public List<SongInfoClass> songList = new List<SongInfoClass>();
    public Dictionary<string, List<SongInfoClass>> songDictionary = new Dictionary<string, List<SongInfoClass>>();

    public SongListShower shower;

#if UNITY_WEBGL
    private async void Start()
    {
        await GetAllInWebGL();
#elif UNITY_STANDALONE || UNITY_EDITOR
    private void Start()
    {
        GetAll();
#endif

        shower.Shower();
    }

    private void GetAll()
    {
        string[] songList = Directory.GetFiles(Application.streamingAssetsPath, "songList.json");
        string songListJson = File.ReadAllText(songList[0]);
        SongList songListContainer = JsonUtility.FromJson<SongList>(songListJson);

        foreach (var song in songListContainer.songs)
        {
            string[] directory = Directory.GetDirectories(Application.streamingAssetsPath, song.fileLocation);

            if (directory.Length == 0)
            {
                Debug.LogWarning($"missing directory: {song.fileLocation}");
                continue;
            }

            string[] jsonFiles = Directory.GetFiles(directory[0], song.difficulty + ".roena");

            if (jsonFiles.Length == 0)
            {
                Debug.LogWarning($"missing file: {directory[0]} {song.difficulty}");
                continue;
            }

            string json = File.ReadAllText(jsonFiles[0]);
            string decrypted = EncryptionHelper.Decrypt(json);

            SongInfoClass info = new();

            info.fileLocation = jsonFiles[0];
            info.id = song.id;
            info.difficulty = song.difficulty;
            info.artist = song.artist;
            info.jpArtist = song.jpArtist;
            info.title = song.title;
            info.jpTitle = song.jpTitle;
            info.category = song.category;
            info.bpm = song.bpm;
            info.eventName = song.eventName;
            info.level = song.level;

            string key = info.artist + "-" + info.title;
            if (!songDictionary.ContainsKey(key))
            {
                songDictionary[key] = new List<SongInfoClass>();
                SongInfoClass temp = new();

                songDictionary[key].Add(temp);
                songDictionary[key].Add(temp);
                songDictionary[key].Add(temp);
                songDictionary[key].Add(temp);
            }

            if (song.difficulty == "MEMORY")
            {
                songDictionary[key][0] = info;
            }
            if (song.difficulty == "ADVERSITY")
            {
                songDictionary[key][1] = info;
            }
            if (song.difficulty == "NIGHTMARE")
            {
                songDictionary[key][2] = info;
            }
            if (song.difficulty == "INFERNO")
            {
                songDictionary[key][3] = info;
            }
        }
    }

    public async Task GetAllInWebGL()
    {
        string songListPath = $"{Application.streamingAssetsPath}/songList.json";

        using (UnityWebRequest songListReq = UnityWebRequest.Get(songListPath))
        {
            var op = songListReq.SendWebRequest();
            while (!op.isDone)
                await Task.Yield();

            if (songListReq.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"[WebGL] Failed to load songList.json: {songListReq.error}");
                return;
            }

            string songListJson = songListReq.downloadHandler.text;
            SongList songListContainer = JsonUtility.FromJson<SongList>(songListJson);

            if (songListContainer == null || songListContainer.songs == null)
            {
                Debug.LogError("[WebGL] songListContainer is null or empty.");
                return;
            }

            foreach (var song in songListContainer.songs)
            {
                // StreamingAssets/song.fileLocation/
                string songFolderUrl = $"{Application.streamingAssetsPath}/{song.fileLocation}/";
                string songFileUrl = $"{songFolderUrl}{song.difficulty}.roena";

                using (UnityWebRequest req = UnityWebRequest.Get(songFileUrl))
                {
                    var subOp = req.SendWebRequest();
                    while (!subOp.isDone)
                        await Task.Yield();

                    if (req.result != UnityWebRequest.Result.Success)
                    {
                        Debug.LogWarning($"[WebGL] missing file: {songFileUrl}");
                        continue;
                    }

                    string encrypted = req.downloadHandler.text;
                    string decrypted = EncryptionHelper.Decrypt(encrypted);

                    SongInfoClass info = new()
                    {
                        fileLocation = songFileUrl,
                        id = song.id,
                        difficulty = song.difficulty,
                        artist = song.artist,
                        jpArtist = song.jpArtist,
                        title = song.title,
                        jpTitle = song.jpTitle,
                        category = song.category,
                        bpm = song.bpm,
                        eventName = song.eventName,
                        level = song.level
                    };

                    string key = info.artist + "-" + info.title;
                    if (!songDictionary.ContainsKey(key))
                    {
                        songDictionary[key] = new List<SongInfoClass> { new(), new(), new(), new() };
                    }

                    switch (song.difficulty)
                    {
                        case "MEMORY":
                            songDictionary[key][0] = info;
                            break;
                        case "ADVERSITY":
                            songDictionary[key][1] = info;
                            break;
                        case "NIGHTMARE":
                            songDictionary[key][2] = info;
                            break;
                        case "INFERNO":
                            songDictionary[key][3] = info;
                            break;
                    }
                }
            }
        }
    }

    [System.Serializable]
    private class SongContainer
    {
        public SongInfoClass info;

    }
}
