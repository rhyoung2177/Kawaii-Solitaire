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

    public void OnClickMenuButton()
    {
        SceneManager.LoadScene("Menu");
        // 위에서 서랍 내려오는 애니메이션
    }
}
