using Cysharp.Threading.Tasks;
using DG.Tweening;
using DressUp.Utils;
using UnityEngine;

namespace DressUp.Core
{
public class CreamTool : BaseToolBehaviour
{
    [Header("References")]
    [SerializeField]
    private RectTransform creamRect;

    private Vector2 _creamDefaultPos;
    private RectTransform _originalCreamParent;
    private Vector2 _originalCreamPos;

    private void Awake()
    {
        _originalCreamParent = creamRect.parent as RectTransform;
        _originalCreamPos = creamRect.anchoredPosition;
    }

    protected override async UniTask PickUp()
    {
        _state = ToolState.PickingUp;
        handRect.anchoredPosition = handHiddenPos;

        await handRect.DOMove(creamRect.position, moveSpeed).ToUniTask();
        _creamDefaultPos = handRect.anchoredPosition;

        Vector2 faceAnchoredPos = await GetAnchoredFromWorld(controller.DragController.FaceZone.GetFaceRectPosition());
        _handReadyPos = new Vector2(_creamDefaultPos.x, (_creamDefaultPos.y + faceAnchoredPos.y) * 0.5f);

        creamRect.SetParent(handRect, true);
        await UniTask.Delay(150);
        await handRect.DOAnchorPos(_handReadyPos, moveSpeed * 0.5f).ToUniTask();

        _state = ToolState.PlayerDrag;
        await controller.DragController.WaitForNoTouch();
        controller.DragController.EnableDrag();
    }

    protected override async UniTask WaitForPlayerDrag()
    {
        var tcs = new UniTaskCompletionSource();

        void Unsubscribe()
        {
            controller.DragController.OnRelease -= OnRelease;
            controller.DragController.OnTap -= OnTap;
        }

        void OnRelease(bool inZone)
        {
            if (inZone)
                tcs.TrySetResult();
        }

        void OnTap(Vector2 screenPos)
        {
            HandleTapAsync(screenPos, tcs).Forget();
        }

        RegisterDragWaitInterrupt(() =>
        {
            Unsubscribe();
            tcs.TrySetResult();
        });

        controller.DragController.OnRelease += OnRelease;
        controller.DragController.OnTap += OnTap;

        try
        {
            await tcs.Task;
        }
        finally
        {
            Unsubscribe();
            ClearDragWaitInterrupt();
        }

        controller.DragController.DisableDrag();

        if (_isCancelled)
            return;

        await Apply();
    }

    private async UniTaskVoid HandleTapAsync(Vector2 screenPos, UniTaskCompletionSource tcs)
    {
        if (!controller.DragController.FaceZone.Contains(screenPos))
            return;
        Vector3 facePos = controller.DragController.FaceZone.GetFaceRectPosition();
        await handRect.DOMove(facePos, moveSpeed).ToUniTask();
        if (_isCancelled)
            return;
        tcs.TrySetResult();
    }

    private async UniTask Apply()
    {
        if (_isCancelled)
            return;
        _state = ToolState.Applying;
        AudioEvents.CreamApplied();
        await handRect.DOPunchPosition(new Vector3(0, 30f, 0), 0.4f, 2).ToUniTask();
        controller.FaceVisuals.RemoveAcne();
    }

    protected override async UniTask ReturnTool()
    {
        _state = ToolState.Returning;
        await handRect.DOAnchorPos(_creamDefaultPos, moveSpeed).ToUniTask();
        await UniTask.Delay(150);
        creamRect.SetParent(_originalCreamParent, true);
        creamRect.anchoredPosition = _originalCreamPos;
        await handRect.DOAnchorPos(handHiddenPos, moveSpeed).ToUniTask();
        _state = ToolState.Idle;
    }

    protected override void OnDeactivate()
    {
        if (creamRect.parent != handRect)
            return;
        creamRect.SetParent(_originalCreamParent, true);
        creamRect.anchoredPosition = _originalCreamPos;
    }

    public void ActivateFromButton()
    {
        controller.SelectTool(this);
    }
}
}
