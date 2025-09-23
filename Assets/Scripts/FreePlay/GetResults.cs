using System;
using System.Collections;
using System.IO;
using System.Threading.Tasks;
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

    public async Task<CursorPageResultResponse> GetBestResultAPI(
        string baseUrl,
        string accessToken,
        int size,
        long? cursor,
        Action<ResponseEntity_CursorPageResultResponse> onSuccess = null,
        Action<string> onError = null,
        int timeOutSec = 15)
    {
        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            onError?.Invoke("GetBestResultAPI: Base URL is empty");
            return null;
        }
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            onError?.Invoke("GetBestResultAPI: Access Token is empty");
            return null;
        }

        string url = $"{baseUrl}/game/rhythm-game-play-history/my/best?size={size}";
        if (cursor.HasValue)
        {
            url += $"&cursor={cursor.Value}";
        }

        using (var req = UnityWebRequest.Get(url))
        {
            req.timeout = timeOutSec;
            req.SetRequestHeader("Authorization", $"Bearer {accessToken}");

            var op = req.SendWebRequest();
            // async/await?? UnityWebRequest ???? ??
            while (!op.isDone)
                await Task.Yield();

            if (req.result != UnityWebRequest.Result.Success)
            {
                onError?.Invoke(req.error);
                return null;
            }

            string json = req.downloadHandler.text;

            ResponseEntity_CursorPageResultResponse parsed = null;
            try
            {
                parsed = JsonUtility.FromJson<ResponseEntity_CursorPageResultResponse>(json);
            }
            catch (Exception e)
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
            return parsed.data; // <- ResultResponse ??
        }
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