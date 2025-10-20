using UnityEngine;

public class UserClass : MonoBehaviour
{
}

[System.Serializable]
public class Player
{
    public string id;
    public string accessToken;
    public string refreshToken;
    public string playerName;
    public float rating;
    public long ranking;
    public string createdAt;
    public string updatedAt;
}

[System.Serializable]
public class GetMyRatingResponse
{
    public string playerId;
    public float rating;
    public long ranking;
    public string createdAt;
    public string updatedAt;
}

[System.Serializable]
public class ResponseEntity_GetMyRatingResponse
{
    public string message;
    public GetMyRatingResponse data;
}