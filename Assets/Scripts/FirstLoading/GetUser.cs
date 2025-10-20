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
                                return await GetUserRatingAPI(baseUrl, playerData.accessToken, onSuccess, onError, timeOutSec);
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
