using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClothObject : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private RectTransform hoverLayer;

    [SerializeField] private float hoverScale = 1.08f;
    [SerializeField] private float rotateAngle = 3f;

    private RectTransform rectTransform;

    private Transform originalParent;
    private int originalSiblingIndex;

    private Vector3 defaultScale;
    private Quaternion defaultRotation;
    private Vector2 defaultAnchoredPos;

    private Tween scaleTween;
    private Tween rotateTween;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        defaultScale = rectTransform.localScale;
        defaultRotation = rectTransform.localRotation;
        defaultAnchoredPos = rectTransform.anchoredPosition;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        scaleTween?.Kill();
        rotateTween?.Kill();

        originalParent = transform.parent;
        originalSiblingIndex = transform.GetSiblingIndex();

        transform.SetParent(hoverLayer, true);

        rectTransform.DOScale(defaultScale * hoverScale, 0.2f)
            .SetEase(Ease.OutBack);

        rotateTween = rectTransform
            .DOLocalRotate(new Vector3(0, 0, rotateAngle), 0.18f)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        scaleTween?.Kill();
        rotateTween?.Kill();

        transform.SetParent(originalParent, true);
        transform.SetSiblingIndex(originalSiblingIndex);

        rectTransform.DOScale(defaultScale, 0.15f)
            .SetEase(Ease.OutBack);

        rectTransform.DOLocalRotate(Vector3.zero, 0.15f)
            .SetEase(Ease.OutQuad);

        rectTransform.anchoredPosition = defaultAnchoredPos;
    }

    public void OnClickCloth(int num)
    {
        UIManager.Instance.ChangeCloth(num);
    }
}