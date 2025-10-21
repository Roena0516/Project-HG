using UnityEngine;
using DG.Tweening;

public class FCAPAnimation : MonoBehaviour
{
    [SerializeField] private RectTransform _FullTop;
    [SerializeField] private RectTransform _FullBottom;
    [SerializeField] private RectTransform _ComboTop;
    [SerializeField] private RectTransform _ComboBottom;

    public void OnFCAPActivated()
    {
        _FullTop.DOAnchorPosX(78, 2f).SetEase(Ease.OutCubic);
        _FullTop.DOAnchorPosY(16, 2f).SetEase(Ease.OutCubic);

        _FullBottom.DOAnchorPosX(-78, 2f).SetEase(Ease.OutCubic);
        _FullBottom.DOAnchorPosY(-16, 2f).SetEase(Ease.OutCubic);

        _ComboTop.DOAnchorPosX(102, 2f).SetEase(Ease.OutCubic);
        _ComboTop.DOAnchorPosY(-30, 2f).SetEase(Ease.OutCubic);

        _ComboBottom.DOAnchorPosX(-102, 2f).SetEase(Ease.OutCubic);
        _ComboBottom.DOAnchorPosY(30, 2f).SetEase(Ease.OutCubic);
    }
}
