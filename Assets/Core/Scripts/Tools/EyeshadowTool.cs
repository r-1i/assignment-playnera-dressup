using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using DressUp.Utils;

namespace DressUp.Core
{
public class EyeshadowTool : BaseToolBehaviour, IColorableTool
{
    [Header("References")]
    [SerializeField]
    private RectTransform brushRect;

    [Header("Brush")]
    [SerializeField]
    private Vector2 brushTipOffset;

    private Vector2 _brushDefaultPos;
    private RectTransform _brushOriginalParent;
    private RectTransform _selectedPaletteRect;
    private Sprite _selectedSprite;

    private void Awake()
    {
        _brushOriginalParent = brushRect.parent as RectTransform;
        _brushDefaultPos = brushRect.anchoredPosition;
    }

    public void SetSprite(Sprite sprite) => _selectedSprite = sprite;
    public void SetPalette(RectTransform paletteRect) => _selectedPaletteRect = paletteRect;

    protected override bool CanActivate() => _selectedSprite != null && _selectedPaletteRect != null;

    protected override void OnDeactivate()
    {
        if (brushRect.parent != handRect)
            return;
        brushRect.SetParent(_brushOriginalParent, true);
        brushRect.anchoredPosition = _brushDefaultPos;
    }

    protected override async UniTask PickUp()
    {
        _state = ToolState.PickingUp;
        handRect.anchoredPosition = handHiddenPos;

        await handRect.DOMove(brushRect.position, moveSpeed).ToUniTask();
        brushRect.SetParent(handRect, true);
        AudioEvents.BrushPickup();
        await UniTask.Delay(150);

        Vector3 paletteWorldPos = _selectedPaletteRect.position;
        paletteWorldPos.y += brushTipOffset.y;
        await handRect.DOMove(paletteWorldPos, moveSpeed).ToUniTask();
        await handRect.DOPunchPosition(new Vector3(20f, 20f, 0), 0.6f, 5, 0.5f).ToUniTask();

        Vector2 palettePos = handRect.anchoredPosition;
        Vector2 faceAnchoredPos = await GetAnchoredFromWorld(controller.DragController.FaceZone.GetFaceRectPosition());
        _handReadyPos = new Vector2(palettePos.x, (palettePos.y + faceAnchoredPos.y) * 0.5f);
        await handRect.DOAnchorPos(_handReadyPos, moveSpeed * 0.5f).ToUniTask();

        _state = ToolState.PlayerDrag;
        await controller.DragController.WaitForNoTouch();
        controller.DragController.EnableDrag();
    }

    protected override async UniTask WaitForPlayerDrag()
    {
        var tcs = new UniTaskCompletionSource<bool>();

        void Unsubscribe()
        {
            controller.DragController.OnRelease -= OnRelease;
            controller.DragController.OnTap -= OnTap;
        }

        void OnRelease(bool inZone)
        {
            if (!inZone)
            {
                tcs.TrySetResult(false);
                return;
            }
            MoveToFaceAndApply(tcs).Forget();
        }

        void OnTap(Vector2 screenPos)
        {
            if (!controller.DragController.FaceZone.ContainsEyes(screenPos))
                return;
            MoveToFaceAndApply(tcs).Forget();
        }

        RegisterDragWaitInterrupt(() =>
        {
            Unsubscribe();
            tcs.TrySetResult(false);
        });

        controller.DragController.OnRelease += OnRelease;
        controller.DragController.OnTap += OnTap;

        bool inZone = false;
        try
        {
            inZone = await tcs.Task;
        }
        finally
        {
            Unsubscribe();
            ClearDragWaitInterrupt();
        }

        controller.DragController.DisableDrag();

        if (_isCancelled)
            return;

        if (inZone)
            await Apply();
    }

    private async UniTaskVoid MoveToFaceAndApply(UniTaskCompletionSource<bool> tcs)
    {
        controller.DragController.DisableDrag();
        Vector3 targetPos = controller.DragController.FaceZone.GetEyesRectPosition() +
                            new Vector3(brushTipOffset.x, brushTipOffset.y, 0);
        await handRect.DOMove(targetPos, moveSpeed).ToUniTask();
        if (_isCancelled)
            return;
        tcs.TrySetResult(true);
    }

    private async UniTask Apply()
    {
        if (_isCancelled)
            return;
        _state = ToolState.Applying;
        AudioEvents.BrushApplied();
        await handRect.DOPunchPosition(new Vector3(0, 25f, 0), 1f, 5, 0.5f).ToUniTask();
        controller.FaceVisuals.ApplyEyeshadow(_selectedSprite);
    }

    protected override async UniTask ReturnTool()
    {
        _state = ToolState.Returning;
        await handRect.DOMove(brushRect.position, moveSpeed).ToUniTask();
        await UniTask.Delay(150);
        brushRect.SetParent(_brushOriginalParent, true);
        brushRect.anchoredPosition = _brushDefaultPos;
        await handRect.DOAnchorPos(handHiddenPos, moveSpeed).ToUniTask();
        _state = ToolState.Idle;
    }
}
}
