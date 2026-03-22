using UnityEngine;
using UnityEngine.UI;

namespace DressUp.Core
{
[RequireComponent(typeof(Button))]
public class ColorButton : MonoBehaviour
{
    [SerializeField]
    private MakeupController makeupController;
    [SerializeField]
    private Sprite makeupSprite;
    [SerializeField]
    private MonoBehaviour tool;

    private ITool _tool;

    private void Awake()
    {
        _tool = tool as ITool;

        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        var paletteUpdated = false;
        if (_tool is IColorableTool colorable)
        {
            colorable.SetSprite(makeupSprite);
            colorable.SetPalette(GetComponent<RectTransform>());
            paletteUpdated = true;
        }

        if (_tool == null)
            return;
        if (makeupController == null)
        {
            Debug.LogError("ColorButton: назначьте MakeupController.", this);
            return;
        }

        if (paletteUpdated && makeupController.IsCurrentTool(_tool))
            return;

        makeupController.SelectTool(_tool);
    }
}
}
