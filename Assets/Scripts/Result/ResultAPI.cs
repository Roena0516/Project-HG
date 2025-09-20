using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class ResultAPI : MonoBehaviour
{
    SettingsManager settings;

    public IEnumerator PostResultAPI(string baseUrl, ResultRequest result, string accessToken, Action<ResultResponse> onSuccess, Action<string> onError, int timeOutSec = 15)
    {
        settings = SettingsManager.Instance;

        if (string.IsNullOrEmpty(baseUrl))
        {
            onError?.Invoke("PostResultAPI: Base URL is empty");
            yield break;
        }
        if (string.IsNullOrEmpty(accessToken))
        {
            onError?.Invoke("PostResultAPI: Access Token is empty");
            yield break;
        }

        string url = $"{baseUrl}/game/game-play-history";
        string jsonData = JsonUtility.ToJson(result);

        using (var req = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST))
        {
            byte[] body = Encoding.UTF8.GetBytes(jsonData);
            req.uploadHandler = new UploadHandlerRaw(body);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.timeout = timeOutSec;

            req.SetRequestHeader("Content-Type", "application/json");
            req.SetRequestHeader("Authorization", $"Bearer {accessToken}");

            yield return req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
            {
                onError?.Invoke($"Network error: {req.error}");
                yield break;
            }

            // not HTTP 200
            if (req.responseCode < 200 || req.responseCode >= 300)
            {
                onError?.Invoke($"HTTP {req.responseCode}: {req.downloadHandler.text}");
                yield break;
            }

            // success
            ResponseEntity_ResultResponse wrapper = null;
            try
            {
                wrapper = JsonUtility.FromJson<ResponseEntity_ResultResponse>(req.downloadHandler.text);
            }
            catch (Exception e)
            {
                onError?.Invoke("JSON parse failed: " + e.Message);
                yield break;
            }

            if (wrapper == null || wrapper.data == null)
            {
                onError?.Invoke("Empty response or missing data.");
                yield break;
            }

            onSuccess?.Invoke(wrapper.data);
        }
    }
}
