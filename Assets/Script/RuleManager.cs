using UnityEngine;
using UnityEngine.SceneManagement;

public enum RuleType
{
    OneSuit = 1,
    TwoSuit = 2,
    FourSuit = 4
}

public class RuleManager : MonoBehaviour
{
    public static RuleManager Instance;

    [HideInInspector] public RuleType ruleType;
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void OnClickRuleButton(int suitCount)
    {
        ruleType = (RuleType) suitCount;
        SceneManager.LoadScene("InGame");
    }
}