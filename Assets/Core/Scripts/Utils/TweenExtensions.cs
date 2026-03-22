using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;

namespace DressUp.Utils
{
public static class TweenExtensions
{
    public static UniTask ToUniTask(this Tween tween)
    {
        var tcs = new UniTaskCompletionSource();
        tween.OnComplete(() => tcs.TrySetResult());
        tween.OnKill(() => tcs.TrySetResult());
        return tcs.Task;
    }

    public static UniTask ToUniTask(this Tween tween, CancellationToken cancellationToken)
    {
        var tcs = new UniTaskCompletionSource();
        tween.OnComplete(() => tcs.TrySetResult());
        tween.OnKill(() => tcs.TrySetResult());
        if (cancellationToken.CanBeCanceled)
        {
            cancellationToken.Register(() =>
            {
                if (tween != null && tween.active)
                    tween.Kill();
            });
        }

        return tcs.Task;
    }
}
}
