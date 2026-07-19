using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI moveText;
    public TextMeshProUGUI setText;

    public RectTransform drawer;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void SetScoreText()
    {
        scoreText.text = InGameManager.Instance.Score.ToString();
    }

    public void SetMoveText()
    {
        moveText.text = InGameManager.Instance.Move.ToString();
    }

    public void SetCompleteSetText()
    {
        setText.text = $"{InGameManager.Instance.CompleteSet}/{InGameManager.SET_COUNT}";
    }

    public void OnClickHintButton()
    {
        InGameManager.Instance.GetHint();
    }

    public void OnClickUndoButton()
    {
        InGameManager.Instance.Undo();
    }

    public void OnClickRestartButton()
    {
        InGameManager.Instance.Restart();
    }
    public void OnClickSettingButton()
    {
        var popup = PopupManager.Instance.OpenPopup();
        popup.Open("Make Setting Popup Prefab");
    }

    public void OnClickMenuButton()
    {
        drawer.DOAnchorPos(Vector2.zero, 0.35f).SetEase(Ease.OutCubic);
    }

    public void OnClickCloseMenuButton()
    {
        drawer.DOAnchorPos(new Vector2(0, 1100), 0.35f).SetEase(Ease.InCubic);
    }
}
