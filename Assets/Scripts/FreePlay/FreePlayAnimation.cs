using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System;

public class FreePlayAnimation : MonoBehaviour
{
    [SerializeField] private RectTransform _songListPanel;
    [SerializeField] private RectTransform _songInfoPanel;
    [SerializeField] private Image _fadeImage;

    private void Awake()
    {
        _fadeImage.color = _fadeImage.color.SetAlpha(1f);
    }

    public void FadeIn(float duration = 0.75f, Action onComplete = null)
    {
        _fadeImage.color = _fadeImage.color.SetAlpha(0f);
        _fadeImage
            .DOFade(1f, duration)
            .SetEase(Ease.InSine)
            .OnComplete(() => onComplete?.Invoke())
            .SetAutoKill(true);
    }

    public void FadeOut(float duration = 0.75f, Action onComplete = null)
    {
        _fadeImage.color = _fadeImage.color.SetAlpha(1f);
        _fadeImage
            .DOFade(0f, duration)
            .SetEase(Ease.InSine)
            .OnComplete(() => onComplete?.Invoke())
            .SetAutoKill(true);
    }

    public void ShowPanels()
    {
        ShowSongList();
        ShowLevelInfo();
    }

    private void ShowSongList()
    {
        _songListPanel.DOAnchorPosX(750, 0).SetAutoKill(true);
        _songListPanel.DOAnchorPosX(-250, 0.5f).SetEase(Ease.InOutSine).SetAutoKill(true);
    }

    private void ShowLevelInfo()
    {
        _songInfoPanel.DOAnchorPosX(-450, 0).SetAutoKill(true);
        _songInfoPanel.DOAnchorPosX(150, 0.5f).SetEase(Ease.InOutSine).SetAutoKill(true);
    }
}
