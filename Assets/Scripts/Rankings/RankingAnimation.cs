using System;
using DG.Tweening;
using UnityEngine;

public class RankingAnimation : MonoBehaviour
{
    [SerializeField] private Fader _fader;

    [SerializeField] private RectTransform _rankingsPanel;

    private void Awake()
    {
        _fader.fadeImage.color = _fader.fadeImage.color.SetAlpha(1f);
    }

    public void SetPanel()
    {
        SetRankings();
    }

    private void SetRankings()
    {
        _rankingsPanel.DOAnchorPosX(1800f, 0).SetAutoKill(true);
        _rankingsPanel.DOAnchorPosX(0f, 0.5f).SetEase(Ease.OutSine).SetAutoKill(true);
    }

    public void FadeIn(float duration = 0.5f, Action onComplete = null)
    {
        _fader.FadeIn(duration, onComplete);
    }

    public void FadeOut(float duration = 0.5f, Action onComplete = null)
    {
        _fader.FadeOut(duration, onComplete);
    }
}
