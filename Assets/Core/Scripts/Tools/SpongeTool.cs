using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace DressUp.Core
{
public class SpongeTool : MonoBehaviour, ITool
{
    [SerializeField]
    private MakeupController controller;
    [SerializeField]
    private Button spongeButton;

    public event Action OnReady;

    private void Awake()
    {
        spongeButton.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        controller.FaceVisuals.ResetAll();
        AudioEvents.SpongeReset();
    }

    public void Activate()
    {
        OnReady?.Invoke();
    }

    public UniTask Deactivate()
    {
        return UniTask.CompletedTask;
    }
}
}
