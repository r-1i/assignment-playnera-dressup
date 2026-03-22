using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using DressUp.Utils;

namespace DressUp.Core
{
public abstract class BaseToolBehaviour : MonoBehaviour, ITool
{
    [Header("References")]
    [SerializeField]
    protected MakeupController controller;
    [SerializeField]
    protected RectTransform handRect;

    [Header("Positions")]
    [SerializeField]
    protected Vector2 handHiddenPos;

    [Header("Timing")]
    [SerializeField]
    protected float moveSpeed = 0.4f;

    protected Vector2 _handReadyPos;
    protected ToolState _state = ToolState.Idle;
    protected bool _isCancelled;
    private Action _interruptDragWait;
    public event Action OnReady;

    protected void RegisterDragWaitInterrupt(Action interrupt)
    {
        _interruptDragWait = interrupt;
    }

    protected void ClearDragWaitInterrupt()
    {
        _interruptDragWait = null;
    }

    public void Activate()
    {
        if (_state != ToolState.Idle)
            return;
        if (!CanActivate())
            return;
        _isCancelled = false;
        RunCycle().Forget();
    }

    protected Vector2 GetReadyPos(Vector2 paletteAnchoredPos)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            handRect.parent as RectTransform,
            RectTransformUtility.WorldToScreenPoint(null, controller.DragController.FaceZone.GetFaceRectPosition()),
            null, out Vector2 faceLocalPos);
        return new Vector2(paletteAnchoredPos.x, (paletteAnchoredPos.y + faceLocalPos.y) * 0.5f);
    }

    protected Vector2 WorldToHandParentLocal(Vector3 worldPos)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(handRect.parent as RectTransform,
            RectTransformUtility.WorldToScreenPoint(null, worldPos),
            null, out Vector2 localPos);
        return localPos;
    }

    protected void NotifyReady()
    {
        OnReady?.Invoke();
    }

    protected async UniTask<Vector2> GetAnchoredFromWorld(Vector3 worldPos)
    {
        Vector3 prevWorld = handRect.position;
        handRect.position = worldPos;
        await UniTask.Yield();
        Vector2 result = handRect.anchoredPosition;
        handRect.position = prevWorld;
        return result;
    }

    public async UniTask Deactivate()
    {
        _isCancelled = true;
        _interruptDragWait?.Invoke();
        _interruptDragWait = null;
        _state = ToolState.Idle;
        controller.DragController.DisableDrag();
        OnDeactivate();
        await handRect.DOAnchorPos(handHiddenPos, moveSpeed * 0.5f).ToUniTask();
    }

    protected virtual bool CanActivate() => true;
    protected abstract UniTask PickUp();
    protected abstract UniTask WaitForPlayerDrag();
    protected abstract UniTask ReturnTool();
    protected virtual void OnDeactivate()
    {
    }

    private async UniTaskVoid RunCycle()
    {
        await PickUp();
        if (_isCancelled)
            return;
        await WaitForPlayerDrag();
        if (_isCancelled)
            return;
        await ReturnTool();
        NotifyReady();
    }
}
}
