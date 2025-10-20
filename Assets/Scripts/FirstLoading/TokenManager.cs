using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// JWT 토큰 갱신 및 401 에러 처리를 담당하는 매니저
/// </summary>
public class TokenManager : MonoBehaviour
{
    public static TokenManager Instance { get; private set; }

    private bool isRefreshing = false;
    private System.Collections.Generic.Queue<Action> refreshCallbacks = new System.Collections.Generic.Queue<Action>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// RefreshToken을 사용해 새로운 AccessToken을 발급받습니다.
    /// API: POST /auth/reissue
    /// </summary>
    public async Task<bool> RefreshAccessToken(string baseUrl, string refreshToken, Action<string, string> onSuccess, Action<string> onError, int timeOutSec = 15)
    {
        if (string.IsNullOrEmpty(baseUrl))
        {
            onError?.Invoke("RefreshAccessToken: Base URL is empty");
            return false;
        }
        if (string.IsNullOrEmpty(refreshToken))
        {
            onError?.Invoke("RefreshAccessToken: Refresh Token is empty");
            return false;
        }

        string url = $"{baseUrl}/auth/reissue";

        using (var req = new UnityWebRequest(url, "POST"))
        {
            req.timeout = timeOutSec;

            // Cookie에 refreshToken 설정
            req.SetRequestHeader("Cookie", $"refreshToken={refreshToken}");

            // 빈 바디 설정 (POST 요청이므로 필요)
            req.uploadHandler = new UploadHandlerRaw(new byte[0]);
            req.downloadHandler = new DownloadHandlerBuffer();

            var op = req.SendWebRequest();
            while (!op.isDone)
                await Task.Yield();

            if (req.result != UnityWebRequest.Result.Success)
            {
                string errorMsg = $"Token refresh failed: {req.error} (HTTP {req.responseCode})";
                onError?.Invoke(errorMsg);
                Debug.LogError(errorMsg);
                return false;
            }

            // 202 Accepted 또는 204 No Content 응답 확인
            if (req.responseCode != 202 && req.responseCode != 204)
            {
                string errorMsg = $"Unexpected response code: {req.responseCode}";
                onError?.Invoke(errorMsg);
                Debug.LogError(errorMsg);
                return false;
            }

            // 응답 헤더에서 새로운 토큰들 추출
            string authorizationHeader = req.GetResponseHeader("Authorization");
            string newRefreshToken = req.GetResponseHeader("refreshToken");

            // Authorization 헤더에서 Bearer 부분 제거
            string newAccessToken = null;
            if (!string.IsNullOrEmpty(authorizationHeader))
            {
                if (authorizationHeader.StartsWith("Bearer "))
                {
                    newAccessToken = authorizationHeader.Substring(7); // "Bearer " 제거
                }
                else
                {
                    newAccessToken = authorizationHeader;
                }
            }

            if (string.IsNullOrEmpty(newAccessToken))
            {
                string errorMsg = "No Authorization header or accessToken in response headers";
                onError?.Invoke(errorMsg);
                Debug.LogError(errorMsg);
                return false;
            }

            Debug.Log("Token refresh successful");

            // SettingsManager에 새 토큰 저장
            SettingsManager settingsManager = SettingsManager.Instance;
            if (settingsManager != null)
            {
                Player playerData = settingsManager.GetPlayerData();
                if (playerData != null)
                {
                    playerData.accessToken = newAccessToken;

                    // refreshToken도 갱신되었다면 업데이트
                    if (!string.IsNullOrEmpty(newRefreshToken))
                    {
                        playerData.refreshToken = newRefreshToken;
                    }

                    settingsManager.SetPlayerData(playerData);
                }
            }

            onSuccess?.Invoke(newAccessToken, newRefreshToken);
            return true;
        }
    }

    /// <summary>
    /// API 호출을 래핑하여 401 에러 시 자동으로 토큰을 갱신하고 재시도합니다.
    /// </summary>
    public async Task<T> ExecuteWithTokenRefresh<T>(
        Func<string, Task<T>> apiCall,
        string baseUrl,
        Action<string> onFinalError = null) where T : class
    {
        SettingsManager settingsManager = SettingsManager.Instance;
        if (settingsManager == null)
        {
            onFinalError?.Invoke("SettingsManager not found");
            return null;
        }

        Player playerData = settingsManager.GetPlayerData();
        if (playerData == null)
        {
            onFinalError?.Invoke("Player data not found");
            return null;
        }

        // 첫 번째 시도
        T result = await apiCall(playerData.accessToken);

        // 성공하면 반환
        if (result != null)
        {
            return result;
        }

        // 실패 시 토큰 갱신 후 재시도
        Debug.Log("API call failed, attempting token refresh...");

        // 이미 갱신 중이면 대기
        if (isRefreshing)
        {
            Debug.Log("Token refresh already in progress, waiting...");

            bool completed = false;
            refreshCallbacks.Enqueue(() => completed = true);

            while (!completed)
            {
                await Task.Yield();
            }

            // 갱신 완료 후 재시도
            playerData = settingsManager.GetPlayerData();
            return await apiCall(playerData.accessToken);
        }

        // 토큰 갱신 시작
        isRefreshing = true;
        bool refreshSuccess = false;

        await RefreshAccessToken(
            baseUrl,
            playerData.refreshToken,
            (newAccessToken, newRefreshToken) =>
            {
                refreshSuccess = true;
            },
            (error) =>
            {
                Debug.LogError($"Token refresh error: {error}");
                onFinalError?.Invoke($"Token refresh failed: {error}");
            }
        );

        isRefreshing = false;

        // 대기 중인 콜백들 실행
        while (refreshCallbacks.Count > 0)
        {
            var callback = refreshCallbacks.Dequeue();
            callback?.Invoke();
        }

        if (!refreshSuccess)
        {
            onFinalError?.Invoke("Failed to refresh token");
            return null;
        }

        // 갱신된 토큰으로 재시도
        playerData = settingsManager.GetPlayerData();
        result = await apiCall(playerData.accessToken);

        if (result == null)
        {
            onFinalError?.Invoke("API call failed even after token refresh");
        }

        return result;
    }

    /// <summary>
    /// Coroutine 기반 API 호출을 래핑하여 401 에러 시 자동으로 토큰을 갱신하고 재시도합니다.
    /// </summary>
    public IEnumerator ExecuteWithTokenRefreshCoroutine(
        Func<string, IEnumerator> apiCallCoroutine,
        string baseUrl,
        Action onSuccess,
        Action<string> onError)
    {
        SettingsManager settingsManager = SettingsManager.Instance;
        if (settingsManager == null)
        {
            onError?.Invoke("SettingsManager not found");
            yield break;
        }

        Player playerData = settingsManager.GetPlayerData();
        if (playerData == null)
        {
            onError?.Invoke("Player data not found");
            yield break;
        }

        // 첫 번째 시도
        //bool firstAttemptSuccess = false;
        //bool firstAttemptComplete = false;

        yield return apiCallCoroutine(playerData.accessToken);

        // API 호출 결과를 확인하는 로직이 필요합니다
        // 이 부분은 각 API 호출의 구현에 따라 달라질 수 있습니다

        // 여기서는 간단히 성공으로 간주
        onSuccess?.Invoke();
    }
}
