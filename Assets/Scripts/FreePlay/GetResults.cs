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
                            bool refreshSuccess = await tokenManager.RefreshAccessToken(
                                baseUrl,
                                playerData.refreshToken,
                                (newAccessToken, newRefreshToken) =>
                                {
                                    Debug.Log("Token refreshed successfully, retrying API call...");
                                },
                                (error) =>
                                {
                                    Debug.LogError($"Token refresh failed: {error}");
                                }
                            );

                            if (refreshSuccess)
                            {
                                // 갱신된 토큰으로 재시도
                                playerData = settingsManager.GetPlayerData();
                                return await GetBestResultAPI(baseUrl, playerData.accessToken, size, cursor, onSuccess, onError, timeOutSec);
                            }
                        }
                    }
                }

                onError?.Invoke("Unauthorized and token refresh failed");
                return null;
            }

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
                            System.Threading.Tasks.Task<bool> refreshTask = tokenManager.RefreshAccessToken(
                                baseUrl,
                                playerData.refreshToken,
                                (newAccessToken, newRefreshToken) =>
                                {
                                    Debug.Log("Token refreshed successfully, retrying API call...");
                                },
                                (error) =>
                                {
                                    Debug.LogError($"Token refresh failed: {error}");
                                }
                            );

                            // Task가 완료될 때까지 대기
                            yield return new WaitUntil(() => refreshTask.IsCompleted);

                            // Task의 반환값으로 성공 여부 확인
                            if (refreshTask.Result)
                            {
                                // 갱신된 토큰으로 재시도
                                playerData = settingsManager.GetPlayerData();
                                yield return GetResultsAPI(baseUrl, playerData.accessToken, size, cursor, onSuccess, onError, timeOutSec);
                                yield break;
                            }
                        }
                    }
                }

                onError?.Invoke("Unauthorized and token refresh failed");
                yield break;
            }

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