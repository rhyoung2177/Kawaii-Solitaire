using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    public Text scoreText;
    public Text moveText;
    public Text setText;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void SetText()
    {
        scoreText.text = InGameManager.Instance.score.ToString();
        moveText.text = InGameManager.Instance.move.ToString();
        setText.text = $"{InGameManager.Instance.completeSet}/{InGameManager.SET_COUNT}";
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

    public void OnClickMenuButton()
    {
        // 위에서 서랍 내려오는 애니메이션
    }
}
