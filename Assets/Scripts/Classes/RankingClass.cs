using System;
using System.Collections.Generic;
using UnityEngine;

public class RankingClass : MonoBehaviour
{
}

[Serializable]
public class PlayerResponse
{
    public string playerId;
    public float rating;
    public long ranking;
    public string createdAt;
    public string updatedAt;
}

[Serializable]
public class CursorPagePlayerResponse
{
    public List<PlayerResponse> values;
    public bool hasNext;
}

[Serializable]
public class ResponseEntity_CursorPagePlayerResponse
{
    public string message;
    public CursorPagePlayerResponse data;
}
