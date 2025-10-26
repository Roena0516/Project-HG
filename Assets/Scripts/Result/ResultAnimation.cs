using System;
using DG.Tweening;
using UnityEngine;

public class ResultAnimation : MonoBehaviour
{
    [SerializeField] private Fader _fader;

    [SerializeField] private RectTransform _rightPanel;
    [SerializeField] private RectTransform _boardPanel;

    private void Awake()
    {
        _fader.fadeImage.color = _fader.fadeImage.color.SetAlpha(1f);
    }

    public void SetPanels()
    {
        SetRightDescription();
        SetBoard();
    }

    private void SetRightDescription()
    {
        _rightPanel.DOAnchorPosX(1300, 0).SetAutoKill(true);
        _rightPanel.DOAnchorPosX(300, 0.5f).SetEase(Ease.OutSine).SetAutoKill(true);
    }

    private void SetBoard()
    {
        _boardPanel.DOAnchorPosX(-350, 0).SetAutoKill(true);
        _boardPanel.DOAnchorPosX(350, 0.5f).SetEase(Ease.OutSine).SetAutoKill(true);
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
