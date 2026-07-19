using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class Popup : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image bgImage;
    [SerializeField] private TextMeshProUGUI text;

    public void Open(string str)
    {
        text.text = str;

        canvasGroup.transform.localScale = Vector3.zero;
        canvasGroup.alpha = 0;
        bgImage.color = new Color(0, 0, 0, 0);

        canvasGroup.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
        canvasGroup.DOFade(1f, 0.3f);
        bgImage.DOFade(0.5f, 0.3f);
    }

    public void Close()
    {
        canvasGroup.transform.DOScale(0f, 0.3f).SetEase(Ease.InBack);
        canvasGroup.DOFade(0f, 0.3f).OnComplete(() => Destroy(gameObject));
        bgImage.DOFade(0f, 0.3f);
    }
}