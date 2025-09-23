using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class GetUser : MonoBehaviour
{
    public async Task<GetMyRatingResponse> GetUserRatingAPI(string baseUrl, string accessToken, Action<ResponseEntity_GetMyRatingResponse> onSuccess, Action<string> onError, int timeOutSec = 15)
    {
        if (string.IsNullOrEmpty(baseUrl))
        {
            onError?.Invoke("GetResultsAPI: Base URL is empty");
            return null;
        }
        if (string.IsNullOrEmpty(accessToken))
        {
            onError?.Invoke("GetResultsAPI: Access Token is empty");
            return null;
        }

        string url = $"{baseUrl}/game/players/my/rating";

        using (var req = UnityWebRequest.Get(url))
        {
            req.timeout = timeOutSec;

            req.SetRequestHeader("Authorization", $"Bearer {accessToken}");

            var op = req.SendWebRequest();
            while (!op.isDone)
                await Task.Yield();

            if (req.result != UnityWebRequest.Result.Success)
            {
                onError?.Invoke(req.error);
                return null;
            }

            string json = req.downloadHandler.text;
            // Debug.Log(json);

            ResponseEntity_GetMyRatingResponse parsed = null;
            try
            {
                parsed = JsonUtility.FromJson<ResponseEntity_GetMyRatingResponse>(json);
            }
            catch (System.Exception e)
            {
                onError?.Invoke($"JSON parse error: {e.Message}");
                return null;
            }

            if (parsed == null || parsed.data == null)
            {
                onError?.Invoke("Empty response or invalid shape");
                return null;
            }

            onSuccess?.Invoke(parsed);
            return parsed.data;
        }
    }
}
