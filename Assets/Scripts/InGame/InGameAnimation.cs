using System;
using DG.Tweening;
using UnityEngine;

public class InGameAnimation : MonoBehaviour
{
    [SerializeField] private Fader _fader;

    [SerializeField] private GameObject _keyBombPrefab;
    [SerializeField] private Transform _effectsFolder;

    public void FadeIn(float duration = 0.5f, Action onComplete = null)
    {
        _fader.FadeIn(duration, onComplete);
    }

    public void FadeOut(float duration = 0.5f, Action onComplete = null)
    {
        _fader.FadeOut(duration, onComplete);
    }

    public void SpawnKeyBombEffect(int index)
    {
        if (_keyBombPrefab == null)
        {
            Debug.LogError("KeyBombPrefab is not assigned");
            return;
        }

        if (_effectsFolder == null)
        {
            Debug.LogError("EffectsFolder is not assigned");
            return;
        }

        // 인덱스 검증
        if (index < 0 || index > 3)
        {
            Debug.LogWarning($"Invalid index: {index}. Must be between 0 and 3");
            return;
        }

        // X 위치 매핑
        float[] xPositions = { -189f, -63f, 63f, 189f };
        float xPos = xPositions[index];
        float yPos = -238f;

        // 프리팹 복제
        GameObject keyBomb = Instantiate(_keyBombPrefab, _effectsFolder);
        RectTransform rectTransform = keyBomb.GetComponent<RectTransform>();

        if (rectTransform == null)
        {
            Debug.LogError("KeyBombPrefab does not have a RectTransform component");
            Destroy(keyBomb);
            return;
        }

        // 위치 설정
        rectTransform.anchoredPosition = new Vector2(xPos, yPos);

        // 초기 스케일 0으로 설정
        //rectTransform.localScale = Vector3.zero;
        rectTransform.DOScale(0.65f, 0f);

        // DOTween 팝인 애니메이션
        rectTransform.DOScaleX(1f, 0.06f)
            .SetEase(Ease.OutQuad);
        rectTransform.DOScaleY(1f, 0.05f)
            .SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                // 애니메이션 완료 후 페이드 아웃 및 파괴
                rectTransform.DOScale(0f, 0.1f)
                    //.SetDelay(0.1f)
                    .SetEase(Ease.InBack)
                    .OnComplete(() => Destroy(keyBomb));
            });
    }
}
