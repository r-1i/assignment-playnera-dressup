using System;
using Cysharp.Threading.Tasks;

namespace DressUp.Core
{
public interface ITool
{
    void Activate();
    UniTask Deactivate();
    event Action OnReady;
}
}
