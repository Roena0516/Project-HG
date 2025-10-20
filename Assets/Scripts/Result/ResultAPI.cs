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

        string url = $"{baseUrl}/game/rhythm-game-play-history";
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

            // 401 에러 감지 시 토큰 갱신 후 재시도
            if (req.responseCode == 401)
            {
                Debug.Log("401 Unauthorized detected, attempting token refresh...");

                TokenManager tokenManager = TokenManager.Instance;
                if (tokenManager != null)
                {
                    SettingsManager settingsManager = SettingsManager.Instance;
                    if (settingsManager != null)
                    {
                        Player playerData = settingsManager.GetPlayerData();
                        if (playerData != null && !string.IsNullOrEmpty(playerData.refreshToken))
                        {
                            bool refreshSuccess = false;
                            System.Threading.Tasks.Task refreshTask = tokenManager.RefreshAccessToken(
                                baseUrl,
                                playerData.refreshToken,
                                (newAccessToken, newRefreshToken) =>
                                {
                                    Debug.Log("Token refreshed successfully, retrying API call...");
                                    refreshSuccess = true;
                                },
                                (error) =>
                                {
                                    Debug.LogError($"Token refresh failed: {error}");
                                }
                            );

                            // Task가 완료될 때까지 대기
                            yield return new WaitUntil(() => refreshTask.IsCompleted);

                            if (refreshSuccess)
                            {
                                // 갱신된 토큰으로 재시도
                                playerData = settingsManager.GetPlayerData();
                                yield return PostResultAPI(baseUrl, result, playerData.accessToken, onSuccess, onError, timeOutSec);
                                yield break;
                            }
                        }
                    }
                }

                onError?.Invoke("Unauthorized and token refresh failed");
                yield break;
            }

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
