using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Fader : MonoBehaviour
{
    public Image fadeImage;

    public void FadeIn(float duration = 0.5f, Action onComplete = null)
    {
        fadeImage.color = fadeImage.color.SetAlpha(0f);
        fadeImage
            .DOFade(1f, duration)
            .SetEase(Ease.InSine)
            .OnComplete(() => onComplete?.Invoke())
            .SetAutoKill(true);
    }

    public void FadeOut(float duration = 0.5f, Action onComplete = null)
    {
        fadeImage.color = fadeImage.color.SetAlpha(1f);
        fadeImage
            .DOFade(0f, duration)
            .SetEase(Ease.InSine)
            .OnComplete(() => onComplete?.Invoke())
            .SetAutoKill(true);
    }
}
