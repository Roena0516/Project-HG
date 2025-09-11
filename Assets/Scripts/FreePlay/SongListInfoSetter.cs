using System.Collections.Generic;
using UnityEngine;

public class SongListInfoSetter : MonoBehaviour
{
    public List<int> ids = new();
    public string artist;
    public string jpArtist;
    public string title;
    public string jpTitle;
    public float BPM;
    public List<string> filePath = new();
    public string eventName;

    public List<Result> results = new();
}
