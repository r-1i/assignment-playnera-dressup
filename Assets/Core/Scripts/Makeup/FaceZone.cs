using UnityEngine;

namespace DressUp.Core
{
public class FaceZone : MonoBehaviour
{
    [SerializeField]
    private RectTransform faceRect;
    [SerializeField]
    private RectTransform eyesRect;
    [SerializeField]
    private RectTransform lipsRect;
    [SerializeField]
    private RectTransform cheeksRect;

    public bool Contains(Vector2 screenPos)
    {
        return ContainsFace(screenPos);
    }

    public bool ContainsFace(Vector2 screenPos)
    {
        return faceRect != null &&
               RectTransformUtility.RectangleContainsScreenPoint(faceRect, screenPos, null);
    }

    public bool ContainsEyes(Vector2 screenPos)
    {
        return eyesRect != null &&
               RectTransformUtility.RectangleContainsScreenPoint(eyesRect, screenPos, null);
    }

    public bool ContainsLips(Vector2 screenPos)
    {
        return lipsRect != null &&
               RectTransformUtility.RectangleContainsScreenPoint(lipsRect, screenPos, null);
    }

    public bool ContainsCheeks(Vector2 screenPos)
    {
        return cheeksRect != null &&
               RectTransformUtility.RectangleContainsScreenPoint(cheeksRect, screenPos, null);
    }

    public Vector3 GetFaceRectPosition()
    {
        return faceRect.position;
    }

    public Vector3 GetEyesRectPosition()
    {
        return eyesRect.position;
    }

    public Vector3 GetLipsRectPosition()
    {
        return lipsRect.position;
    }

    public Vector3 GetCheeksRectPosition()
    {
        return cheeksRect.position;
    }
}
}
