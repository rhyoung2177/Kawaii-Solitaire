using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class RuleCard : MonoBehaviour
{
    public Image glowImage;
    private bool isAnim = false;

    public void OnClickCardButton()
    {
        if (isAnim)
            return;

        isAnim = true;

        if (glowImage.gameObject.activeSelf)
        {
            glowImage.DOFade(0, 0.5f).OnComplete(() =>
            {
                glowImage.gameObject.SetActive(false);
                isAnim = false;
            });
        }
        else
        {
            glowImage.gameObject.SetActive(true);
            glowImage.DOFade(1f, 0.5f).OnComplete(() => isAnim = false);
        }
    }
}
