using System.Collections.Generic;
using UnityEngine;

public class SongListInfoSetter : MonoBehaviour
{
    public List<int> ids = new();
    public string artist;
    public string jpArtist;
    public string title;
    public string jpTitle;
    public string category;
    public float BPM;
    public List<string> filePath = new();
    public string eventName;
    public int previewStart;
    public int previewEnd;
    public List<Result> results = new();
}
