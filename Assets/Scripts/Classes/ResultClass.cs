using System;
using System.Collections.Generic;
using UnityEngine;

public class ResultClass : MonoBehaviour
{
}

[System.Serializable]
public class Result
{
    public string playerId;
    public long musicId;
    public float rate;
    public long combo;
    public long perfectPlus;
    public long perfect;
    public long great;
    public long good;
    public long miss;
    public string rank;
    public string state;
    public string played_at;
}

[System.Serializable]
public class ResultResponse
{
    public long gamePlayHistoryId;
    public string playerId;
    public long musicId;
    public float rate;
    public long combo;
    public long perfectPlus;
    public long perfect;
    public long great;
    public long good;
    public long miss;
    public string rank;
    public string state;
    public string played_at;
}

[Serializable]
public class ResponseEntity_ResultResponse
{
    public string message;
    public ResultResponse data;
}

[System.Serializable]
public class ResultsContainer
{
    public List<Result> results;
}
