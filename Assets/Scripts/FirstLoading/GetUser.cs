using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class GetUser : MonoBehaviour
{
    public async Task<UserProfileResponse> GetUserProfileAPI(string baseUrl, string accessToken, Action<ResponseEntity_UserProfileResponse> onSuccess, Action<string> onError, int timeOutSec = 15)
    {
        if (string.IsNullOrEmpty(baseUrl))
        {
            onError?.Invoke("GetUserProfileAPI: Base URL is empty");
            return null;
        }
        if (string.IsNullOrEmpty(accessToken))
        {
            onError?.Invoke("GetUserProfileAPI: Access Token is empty");
            return null;
        }

        string url = $"{baseUrl}/users/profile";

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
                Debug.Log("[GetUserProfileAPI] 401 Unauthorized detected, attempting token refresh...");

                TokenManager tokenManager = TokenManager.Instance;
                if (tokenManager == null)
                {
                    Debug.LogError("[GetUserProfileAPI] TokenManager.Instance is null");
                    onError?.Invoke("Unauthorized: TokenManager not available");
                    return null;
                }

                SettingsManager settingsManager = SettingsManager.Instance;
                if (settingsManager == null)
                {
                    Debug.LogError("[GetUserProfileAPI] SettingsManager.Instance is null");
                    onError?.Invoke("Unauthorized: SettingsManager not available");
                    return null;
                }

                Player playerData = settingsManager.GetPlayerData();
                if (playerData == null)
                {
                    Debug.LogError("[GetUserProfileAPI] Player data is null");
                    onError?.Invoke("Unauthorized: Player data not available");
                    return null;
                }

                if (string.IsNullOrEmpty(playerData.refreshToken))
                {
                    Debug.LogError("[GetUserProfileAPI] Refresh token is empty");
                    onError?.Invoke("Unauthorized: Refresh token not available");
                    return null;
                }

                Debug.Log($"[GetUserProfileAPI] Attempting token refresh with refreshToken: {playerData.refreshToken.Substring(0, 10)}...");

                bool refreshSuccess = await tokenManager.RefreshAccessToken(
                    baseUrl,
                    playerData.refreshToken,
                    (newAccessToken, newRefreshToken) =>
                    {
                        Debug.Log("[GetUserProfileAPI] Token refreshed successfully, retrying API call...");
                    },
                    (error) =>
                    {
                        Debug.LogError($"[GetUserProfileAPI] Token refresh failed: {error}");
                    }
                );

                if (refreshSuccess)
                {
                    Debug.Log("[GetUserProfileAPI] Token refresh successful, retrying API call");
                    // 갱신된 토큰으로 재시도
                    playerData = settingsManager.GetPlayerData();
                    return await GetUserProfileAPI(baseUrl, playerData.accessToken, onSuccess, onError, timeOutSec);
                }
                else
                {
                    Debug.LogError("[GetUserProfileAPI] Token refresh returned false");
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

            ResponseEntity_UserProfileResponse parsed = null;
            try
            {
                parsed = JsonUtility.FromJson<ResponseEntity_UserProfileResponse>(json);
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
            return parsed.data;
        }
    }

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
                Debug.Log("[GetUserRatingAPI] 401 Unauthorized detected, attempting token refresh...");

                TokenManager tokenManager = TokenManager.Instance;
                if (tokenManager == null)
                {
                    Debug.LogError("[GetUserRatingAPI] TokenManager.Instance is null");
                    onError?.Invoke("Unauthorized: TokenManager not available");
                    return null;
                }

                SettingsManager settingsManager = SettingsManager.Instance;
                if (settingsManager == null)
                {
                    Debug.LogError("[GetUserRatingAPI] SettingsManager.Instance is null");
                    onError?.Invoke("Unauthorized: SettingsManager not available");
                    return null;
                }

                Player playerData = settingsManager.GetPlayerData();
                if (playerData == null)
                {
                    Debug.LogError("[GetUserRatingAPI] Player data is null");
                    onError?.Invoke("Unauthorized: Player data not available");
                    return null;
                }

                if (string.IsNullOrEmpty(playerData.refreshToken))
                {
                    Debug.LogError("[GetUserRatingAPI] Refresh token is empty");
                    onError?.Invoke("Unauthorized: Refresh token not available");
                    return null;
                }

                Debug.Log($"[GetUserRatingAPI] Attempting token refresh with refreshToken: {playerData.refreshToken.Substring(0, 10)}...");

                bool refreshSuccess = await tokenManager.RefreshAccessToken(
                    baseUrl,
                    playerData.refreshToken,
                    (newAccessToken, newRefreshToken) =>
                    {
                        Debug.Log("[GetUserRatingAPI] Token refreshed successfully, retrying API call...");
                    },
                    (error) =>
                    {
                        Debug.LogError($"[GetUserRatingAPI] Token refresh failed: {error}");
                    }
                );

                if (refreshSuccess)
                {
                    Debug.Log("[GetUserRatingAPI] Token refresh successful, retrying API call");
                    // 갱신된 토큰으로 재시도
                    playerData = settingsManager.GetPlayerData();
                    return await GetUserRatingAPI(baseUrl, playerData.accessToken, onSuccess, onError, timeOutSec);
                }
                else
                {
                    Debug.LogError("[GetUserRatingAPI] Token refresh returned false");
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
