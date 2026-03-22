using System;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Cysharp.Threading.Tasks;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

namespace DressUp.Core
{
public class DragController : MonoBehaviour
{
    [SerializeField]
    private FaceZone faceZone;
    [SerializeField]
    private RectTransform handRect;
    [SerializeField]
    private float followSpeed = 15f;

    public event Action OnDragStart;
    public event Action<Vector2> OnDrag;
    public event Action<bool> OnRelease;
    public event Action<Vector2> OnTap;

    public FaceZone FaceZone => faceZone;

    private Vector2 _touchStartPos;
    private const float DragThreshold = 20f;
    private bool _isDrag;
    private bool _isDragging;
    private bool _isEnabled;

    public void EnableDrag() => _isEnabled = true;
    public void DisableDrag() => _isEnabled = false;

    public async UniTask WaitForNoTouch()
    {
        while (Touch.activeTouches.Count > 0)
            await UniTask.Yield();
    }

    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }

    private void OnDisable()
    {
        EnhancedTouchSupport.Disable();
        _isDragging = false;
        _isEnabled = false;
    }

    private void Update()
    {
        if (!_isEnabled)
            return;
        if (Touch.activeTouches.Count == 0)
            return;
        HandleTouch(Touch.activeTouches[0]); // ← передаём touch
    }

    private void HandleTouch(Touch touch)
    {
        var screenPos = touch.screenPosition;

        switch (touch.phase)
        {
        case TouchPhase.Began:
            _isDragging = true;
            _isDrag = false;
            _touchStartPos = screenPos;
            OnDragStart?.Invoke();
            break;

        case TouchPhase.Moved:
        case TouchPhase.Stationary:
            if (!_isDragging)
                break;
            if (!_isDrag && Vector2.Distance(screenPos, _touchStartPos) > DragThreshold)
                _isDrag = true;
            if (_isDrag)
            {
                MoveHand(screenPos);
                OnDrag?.Invoke(screenPos);
            }
            break;

        case TouchPhase.Ended:
        case TouchPhase.Canceled:
            if (!_isDragging)
                break;
            _isDragging = false;
            if (!_isDrag)
                OnTap?.Invoke(screenPos);
            else
                OnRelease?.Invoke(faceZone.Contains(screenPos));
            break;
        }
    }

    private void MoveHand(Vector2 screenPos)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(handRect.parent as RectTransform, screenPos, null,
                                                                out Vector2 localPos);
        handRect.localPosition = Vector2.Lerp(handRect.localPosition, localPos, Time.deltaTime * followSpeed);
    }
}
}
