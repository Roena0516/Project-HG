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
    public string playedAt;
}

[System.Serializable]
public class ResultRequest
{
    public long musicId;
    public float completionRate;
    public long combo;
    public long perfectPlus;
    public long perfect;
    public long great;
    public long good;
    public long miss;
    public string rank;
    public string state;
}

[System.Serializable]
public class ResultResponse
{
    public long gamePlayHistoryId;
    public string playerId;
    public long musicId;
    public float completionRate;
    public long combo;
    public long perfectPlus;
    public long perfect;
    public long great;
    public long good;
    public long miss;
    public string rank;
    public string state;
    public string playedAt;
}

[System.Serializable]
public class CursorPageResultResponse
{
    public ResultResponse[] values;
    public bool hasNext;
}

[Serializable]
public class ResponseEntity_ResultResponse
{
    public string message;
    public ResultResponse data;
}

[System.Serializable]
public class ResponseEntity_CursorPageResultResponse
{
    public string message;
    public CursorPageResultResponse data;
}

[System.Serializable]
public class ResultsContainer
{
    public List<Result> results;
}
