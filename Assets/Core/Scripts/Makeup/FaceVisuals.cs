using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace DressUp.Core
{
public class FaceVisuals : MonoBehaviour
{
    [Header("Acne")]
    [SerializeField]
    private GameObject acneImage;

    [Header("Eyeshadow")]
    [SerializeField]
    private Image eyeshadowLeftImage;
    [SerializeField]
    private Image eyeshadowRightImage;

    [Header("Lips")]
    [SerializeField]
    private Image lipsImage;

    [Header("Blush")]
    [SerializeField]
    private Image blushLeftImage;
    [SerializeField]
    private Image blushRightImage;

    private static readonly float _fadeDuration = 0.3f;

    public void RemoveAcne()
    {
        acneImage.SetActive(false);
    }

    public void ApplyEyeshadow(Sprite sprite)
    {
        eyeshadowLeftImage.sprite = sprite;
        eyeshadowRightImage.sprite = sprite;
        eyeshadowLeftImage.DOFade(1f, _fadeDuration);
        eyeshadowRightImage.DOFade(1f, _fadeDuration);
    }

    public void ApplyLipstick(Sprite sprite)
    {
        lipsImage.sprite = sprite;
        lipsImage.DOFade(1f, _fadeDuration);
    }

    public void ApplyBlush(Sprite sprite)
    {
        blushLeftImage.sprite = sprite;
        blushRightImage.sprite = sprite;
        blushLeftImage.DOFade(1f, _fadeDuration);
        blushRightImage.DOFade(1f, _fadeDuration);
    }

    public void ResetAll()
    {
        acneImage.SetActive(true);
        eyeshadowLeftImage.DOFade(0f, _fadeDuration);
        eyeshadowRightImage.DOFade(0f, _fadeDuration);
        lipsImage.DOFade(0f, _fadeDuration);
        blushLeftImage.DOFade(0f, _fadeDuration);
        blushRightImage.DOFade(0f, _fadeDuration);
    }
}
}
