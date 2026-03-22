using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using DressUp.Utils;
using Cysharp.Threading.Tasks;

namespace DressUp.Core
{
public class ToolbookController : MonoBehaviour
{
    [SerializeField]
    private Button arrowLeft;
    [SerializeField]
    private Button arrowRight;
    [SerializeField]
    private CanvasGroup[] pages;

    private int _currentPage;
    private bool _isFlipping;

    private void Awake()
    {
        for (int i = 0; i < pages.Length; i++)
        {
            pages[i].alpha = i == 0 ? 1f : 0f;
            pages[i].blocksRaycasts = i == 0;
        }

        arrowLeft.onClick.AddListener(() => FlipPage(-1));
        arrowRight.onClick.AddListener(() => FlipPage(1));
    }

    private void FlipPage(int direction)
    {
        if (_isFlipping)
            return;

        int nextPage = (_currentPage + direction + pages.Length) % pages.Length;
        AnimateFlip(_currentPage, nextPage).Forget();
        _currentPage = nextPage;
    }

    private async UniTaskVoid AnimateFlip(int from, int to)
    {
        _isFlipping = true;
        arrowLeft.interactable = false;
        arrowRight.interactable = false;

        try
        {
            pages[from].blocksRaycasts = false;
            pages[to].blocksRaycasts = false;

            await pages[from].DOFade(0f, 0.2f).ToUniTask();
            await pages[to].DOFade(1f, 0.2f).ToUniTask();

            pages[to].blocksRaycasts = true;
        }
        finally
        {
            _isFlipping = false;
            arrowLeft.interactable = true;
            arrowRight.interactable = true;
        }
    }
}
}
