using System;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace DressUp.Core
{
public class MakeupController : MonoBehaviour
{
    [SerializeField]
    private DragController dragController;
    [SerializeField]
    private FaceVisuals faceVisuals;

    private readonly SemaphoreSlim _toolSwitchLock = new SemaphoreSlim(1, 1);

    private ITool _currentTool;
    private UniTaskCompletionSource _readyWait;

    public FaceVisuals FaceVisuals => faceVisuals;
    public DragController DragController => dragController;
    public FaceZone FaceZone => dragController != null ? dragController.FaceZone : null;

    public bool IsCurrentTool(ITool tool) => _currentTool == tool;

    private void OnDestroy()
    {
        _toolSwitchLock.Dispose();
    }

    public void SelectTool(ITool tool)
    {
        if (tool == null)
            return;

        SelectToolAsync(tool).Forget();
    }

    private async UniTaskVoid SelectToolAsync(ITool tool)
    {
        await _toolSwitchLock.WaitAsync();
        UniTaskCompletionSource tcs = null;
        Action onReady = null;
        try
        {
            _readyWait?.TrySetResult();
            _readyWait = null;

            if (_currentTool != null)
                await _currentTool.Deactivate();

            _currentTool = tool;

            tcs = new UniTaskCompletionSource();
            _readyWait = tcs;
            onReady = () => tcs.TrySetResult();
            tool.OnReady += onReady;
            tool.Activate();
        }
        finally
        {
            _toolSwitchLock.Release();
        }

        if (tcs == null || onReady == null)
            return;

        await tcs.Task;

        tool.OnReady -= onReady;
        if (ReferenceEquals(_readyWait, tcs))
            _readyWait = null;

        await ReleaseCurrentToolSlotIfSameAsync(tool);
    }

    private async UniTask ReleaseCurrentToolSlotIfSameAsync(ITool tool)
    {
        await _toolSwitchLock.WaitAsync();
        try
        {
            if (_currentTool == tool)
                _currentTool = null;
        }
        finally
        {
            _toolSwitchLock.Release();
        }
    }

    public void ClearTool()
    {
        ClearToolAsync().Forget();
    }

    private async UniTaskVoid ClearToolAsync()
    {
        await _toolSwitchLock.WaitAsync();
        ITool prev;
        try
        {
            _readyWait?.TrySetResult();
            _readyWait = null;
            prev = _currentTool;
            _currentTool = null;
        }
        finally
        {
            _toolSwitchLock.Release();
        }

        if (prev != null)
            await prev.Deactivate();
    }
}
}
