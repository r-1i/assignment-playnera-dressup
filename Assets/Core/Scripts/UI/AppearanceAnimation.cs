using UnityEngine;
using DG.Tweening;

namespace DressUp.Core
{
public enum AppearanceMoveDirection
{
    LeftToRight,
    RightToLeft,
    TopToBottom,
    BottomToTop
}

[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
public class AppearanceAnimation : MonoBehaviour
{
    [SerializeField]
    private AppearanceMoveDirection direction = AppearanceMoveDirection.LeftToRight;
    [SerializeField]
    [Min(0f)]
    private float offset = 120f;
    [SerializeField]
    [Tooltip("Скорость в единицах anchoredPosition в секунду (чем больше, тем быстрее).")]
    [Min(0.01f)]
    private float speed = 600f;
    [SerializeField]
    [Min(0f)]
    private float startDelay;
    [SerializeField]
    private Ease ease = Ease.OutQuad;
    [SerializeField]
    private bool playOnEnable = true;

    private RectTransform _rect;
    private Vector2 _restAnchoredPosition;
    private Tween _tween;

    private void Awake()
    {
        _rect = (RectTransform)transform;
        _restAnchoredPosition = _rect.anchoredPosition;
    }

    private void OnEnable()
    {
        if (!playOnEnable)
            return;
        Play();
    }

    private void OnDisable()
    {
        _tween?.Kill();
        if (_rect != null)
            _rect.anchoredPosition = _restAnchoredPosition;
    }

    public void Play()
    {
        if (_rect == null)
            _rect = (RectTransform)transform;

        _tween?.Kill();

        _restAnchoredPosition = _rect.anchoredPosition;
        var hiddenOffset = GetHiddenOffset();
        _rect.anchoredPosition = _restAnchoredPosition + hiddenOffset;

        float duration = Mathf.Max(0.01f, Mathf.Abs(offset) / speed);

        _tween = DOTween.Sequence()
            .AppendInterval(startDelay)
            .Append(_rect.DOAnchorPos(_restAnchoredPosition, duration).SetEase(ease))
            .SetLink(gameObject);
    }

    public void RefreshRestPosition()
    {
        if (_rect == null)
            _rect = (RectTransform)transform;
        _restAnchoredPosition = _rect.anchoredPosition;
    }

    private Vector2 GetHiddenOffset()
    {
        switch (direction)
        {
        case AppearanceMoveDirection.LeftToRight:
            return new Vector2(-offset, 0f);
        case AppearanceMoveDirection.RightToLeft:
            return new Vector2(offset, 0f);
        case AppearanceMoveDirection.TopToBottom:
            return new Vector2(0f, offset);
        case AppearanceMoveDirection.BottomToTop:
            return new Vector2(0f, -offset);
        default:
            return Vector2.zero;
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        speed = Mathf.Max(0.01f, speed);
    }
#endif
}
}
