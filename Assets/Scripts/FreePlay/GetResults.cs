using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class GetResults : MonoBehaviour
{
    public ResultsContainer GetResultsAPIs()
    {
        string[] results = Directory.GetFiles(Application.streamingAssetsPath, "result.json");
        string resultsJson = File.ReadAllText(results[0]);
        ResultsContainer resultsContainer = JsonUtility.FromJson<ResultsContainer>(resultsJson);

        return resultsContainer;
    }

    public IEnumerator GetResultsAPI(string baseUrl, string accessToken, int size, long? cursor, Action<ResponseEntity_CursorPageResultResponse> onSuccess, Action<string> onError, int timeOutSec = 15)
    {
        if (string.IsNullOrEmpty(baseUrl))
        {
            onError?.Invoke("GetResultsAPI: Base URL is empty");
            yield break;
        }
        if (string.IsNullOrEmpty(accessToken))
        {
            onError?.Invoke("GetResultsAPI: Access Token is empty");
            yield break;
        }

        string url = $"{baseUrl}/game/game-play-history/my?size={size}";
        if (cursor.HasValue)
        {
            url += $"&cursor={cursor.Value}";
        }

        using (var req = UnityWebRequest.Get(url))
        {
            req.timeout = timeOutSec;

            req.SetRequestHeader("Authorization", $"Bearer {accessToken}");

            yield return req.SendWebRequest();

            bool isNetworkError = req.result == UnityWebRequest.Result.ConnectionError || req.result == UnityWebRequest.Result.ProtocolError;
            if (isNetworkError)
            {
                onError?.Invoke(req.error);
                yield break;
            }

            string json = req.downloadHandler.text;
            // Debug.Log(json);

            ResponseEntity_CursorPageResultResponse parsed = null;
            try
            {
                parsed = JsonUtility.FromJson<ResponseEntity_CursorPageResultResponse>(json);
            }
            catch (System.Exception e)
            {
                onError?.Invoke($"JSON parse error: {e.Message}");
                yield break;
            }

            if (parsed == null || parsed.data == null)
            {
                onError?.Invoke("Empty response or invalid shape");
                yield break;
            }

            onSuccess?.Invoke(parsed);
        }
    }
}