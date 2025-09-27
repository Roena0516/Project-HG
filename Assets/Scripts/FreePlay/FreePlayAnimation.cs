using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class FreePlayAnimation : MonoBehaviour
{
    [SerializeField] private RectTransform _songListPanel;
    [SerializeField] private RectTransform _songInfoPanel;

    public void ShowSongList()
    {
        _songListPanel.DOAnchorPosX(1000, 0);
        _songListPanel.DOAnchorPosX(-1000, 0.5f).SetEase(Ease.InOutCubic);
    }
}
