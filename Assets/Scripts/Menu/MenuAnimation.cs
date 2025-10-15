using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MenuAnimation : MonoBehaviour
{
    [SerializeField] private Image _fadeImage;

    public void FadeIn( float duration = 0.5f, Action onComplete = null )
    {
        _fadeImage.color = _fadeImage.color.SetAlpha(0f);
        _fadeImage
            .DOFade(1f, duration)
            .SetEase(Ease.InSine)
            .OnComplete(() => onComplete?.Invoke())
            .SetAutoKill(true);
    }

    public void FadeOut( float duration = 0.5f, Action onComplete = null )
    {
        _fadeImage.color = _fadeImage.color.SetAlpha(1f);
        _fadeImage
            .DOFade(0f, duration)
            .SetEase(Ease.InSine)
            .OnComplete(() => onComplete?.Invoke())
            .SetAutoKill(true);
    }
}
