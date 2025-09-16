using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CircleMenuController : MonoBehaviour
{
    [Header("Circle")]
    public RectTransform circleRect;     // 원 이미지 RectTransform (지름 1324px)
    public float radius;
    [Tooltip("가운데(선택) 항목이 위치할 각도(도 단위, 0°=우, 90°=상, 180°=좌, 270°=하)")]
    private float centerAngleDeg = 180f;

    [Header("Items")]
    public List<RectTransform> items = new List<RectTransform>();
    [Tooltip("한 칸 이동 시 회전 각도(도)")]
    private float stepAngleDeg = 22.5f;

    [Header("Tween")]
    private float tweenDuration = 0.18f;

    // 내부 상태
    private float _offsetDeg = 0f;    // 현재 회전 오프셋(도)
    private Coroutine _moveCo;

    void Start()
    {
        if (circleRect == null)
        {
            Debug.LogWarning("circleRect가 비어있습니다.");
            return;
        }
        radius = circleRect.rect.width * 0.5f;

        // 시작 배치
        LayoutItemsInstant();
    }

    void Update()
    {
        // 위 / 아래 입력
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            Move(-1); // 반시계(위로)
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            Move(1);  // 시계(아래로)
    }

    public void Move(int dir) // dir: +1 아래(시계), -1 위(반시계)
    {
        float target = _offsetDeg + stepAngleDeg * dir;
        if (_moveCo != null) StopCoroutine(_moveCo);
        _moveCo = StartCoroutine(AnimateOffset(_offsetDeg, target, tweenDuration));
    }

    IEnumerator AnimateOffset(float from, float to, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float k = EaseOutSine(t / duration);
            _offsetDeg = Mathf.Lerp(from, to, k);
            LayoutItemsInstant();
            yield return null;
        }
        _offsetDeg = to;
        LayoutItemsInstant();
        _moveCo = null;
    }

    // 배치 로직
    private void LayoutItemsInstant()
    {
        if (items == null || items.Count == 0 || circleRect == null) return;

        // 반지름: 실제 배치에 사용하는 값 (원 이미지 스케일을 고려)
        float r = (circleRect.rect.width * 0.5f); // circleRect가 parent의 중앙(0.5,0.5) 앵커/피벗일 때

        for (int i = 0; i < items.Count; i++)
        {
            // 가운데 아이템이 index=1이라고 가정
            int rel = i - 1;
            float deg = centerAngleDeg + _offsetDeg + rel * stepAngleDeg;
            float rad = deg * Mathf.Deg2Rad;

            // circleRect 자식이므로 circleRect 기준 로컬좌표 = anchoredPosition
            Vector2 p = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * r;
            items[i].anchoredPosition = p;

            // 회전/스케일 영향 방지
            items[i].localRotation = Quaternion.identity;
            items[i].localScale = Vector3.one;
        }
    }

    private static float EaseOutSine(float x)
    {
        x = Mathf.Clamp01(x);
        return Mathf.Sin(x * Mathf.PI * 0.5f);
    }
}