using System;
using System.Collections.Generic;
using UnityEngine;

public class ResultClass : MonoBehaviour
{
}

[Serializable]
public class Result
{
    public long gamePlayHistoryId;
    public string userId;
    public long musicId;
    public float rate;
    public float rating;
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

[Serializable]
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
    public string state;
}

[Serializable]
public class ResultResponse
{
    public long gamePlayHistoryId;
    public string userId;
    public long musicId;
    public float completionRate;
    public float rating;
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

[Serializable]
public class CursorPageResultResponse
{
    public List<ResultResponse> values;
    public bool hasNext;
}

[Serializable]
public class ResponseEntity_ResultResponse
{
    public string message;
    public ResultResponse data;
}

[Serializable]
public class ResponseEntity_CursorPageResultResponse
{
    public string message;
    public CursorPageResultResponse data;
}

[Serializable]
public class ResultsContainer
{
    public List<Result> results;
}
