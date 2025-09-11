using System.Collections.Generic;
using UnityEngine;

public class ResultClass : MonoBehaviour
{
}

public class Result
{
    public string playerId;
    public int musicId;
    public float rate;
    public int combo;
    public int perfectPlus;
    public int perfect;
    public int great;
    public int good;
    public int miss;
    public string played_at;
}

public class ResultsContainer
{
    public List<Result> results;
}
