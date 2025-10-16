using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System;

public class FreePlayAnimation : MonoBehaviour
{
    [SerializeField] private RectTransform _songListPanel;
    [SerializeField] private RectTransform _songInfoPanel;

    [SerializeField] private Fader _fader;

    private void Awake()
    {
        _fader.fadeImage.color = _fader.fadeImage.color.SetAlpha(1f);
    }

    public void FadeIn(float duration = 0.5f, Action onComplete = null)
    {
        _fader.FadeIn(duration, onComplete);
    }

    public void FadeOut(float duration = 0.5f, Action onComplete = null)
    {
        _fader.FadeOut(duration, onComplete);
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
