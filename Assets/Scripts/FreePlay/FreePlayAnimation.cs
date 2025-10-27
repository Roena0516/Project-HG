using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System;
using TMPro;

public class FreePlayAnimation : MonoBehaviour
{
    [SerializeField] private RectTransform _songListPanel;
    [SerializeField] private RectTransform _songInfoPanel;
    [SerializeField] private RectTransform _approachBackground1;
    [SerializeField] private Image _approachBackground1_5;
    [SerializeField] private RectTransform _approachBackground2;
    [SerializeField] private Image _card;
    [SerializeField] private Image _jacket;
    [SerializeField] private Image _levelSprite;
    [SerializeField] private TextMeshProUGUI _levelTitle;
    [SerializeField] private TextMeshProUGUI _levelValue;
    [SerializeField] private Image _songTitleBackground;
    [SerializeField] private TextMeshProUGUI _songTitle;
    [SerializeField] private TextMeshProUGUI _songArtist;

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
        _songListPanel.DOAnchorPosX(-250, 0.5f).SetEase(Ease.OutSine).SetAutoKill(true);
    }

    private void ShowLevelInfo()
    {
        _songInfoPanel.DOAnchorPosX(-450, 0).SetAutoKill(true);
        _songInfoPanel.DOAnchorPosX(150, 0.5f).SetEase(Ease.OutSine).SetAutoKill(true);
    }

    public void Approach(Action onComplete = null)
    {
        _approachBackground1.DOAnchorPosY(900f, 1f)
            .SetEase(Ease.OutSine)
            .OnComplete(() =>
            {
                _approachBackground1.DOAnchorPosY(1000f, 4f)
                    .OnComplete(() => onComplete?.Invoke())
                    .SetAutoKill(true);
            })
            .SetAutoKill(true);
        _approachBackground2.DOAnchorPosY(450f, 1f)
            .SetEase(Ease.OutQuad)
            .SetAutoKill(true);

        _card.DOFade(1f, 0.25f)
            .SetEase(Ease.OutSine)
            .SetAutoKill(true);
        _jacket.DOFade(1f, 0.25f)
            .SetEase(Ease.OutSine)
            .SetAutoKill(true);
        _levelSprite.DOFade(1f, 0.25f)
            .SetEase(Ease.OutSine)
            .SetAutoKill(true);
        _levelTitle.DOFade(1f, 0.25f)
            .SetEase(Ease.OutSine)
            .SetAutoKill(true);
        _levelValue.DOFade(1f, 0.25f)
            .SetEase(Ease.OutSine)
            .SetAutoKill(true);
        _songTitleBackground.DOFade(1f, 0.25f)
            .SetEase(Ease.OutSine)
            .SetAutoKill(true);
        _songTitle.DOFade(1f, 0.25f)
            .SetEase(Ease.OutSine)
            .SetAutoKill(true);
        _songArtist.DOFade(1f, 0.25f)
            .SetEase(Ease.OutSine)
            .SetAutoKill(true);
    }
}
