using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RuleManager : MonoBehaviour
{
    public static RuleManager Instance;

    public List<Image> selectCardList;
    [HideInInspector] public List<CardData.Suit> cardTypeList;

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

    public void OnClickStartButton()
    {
        cardTypeList = new List<CardData.Suit>();

        for (int i = 0; i < selectCardList.Count; i++)
        {
            if (selectCardList[i].gameObject.activeSelf)
            {
                cardTypeList.Add((CardData.Suit)i);
            }
        }

        if (cardTypeList.Count == 0 || cardTypeList.Count == 3)
        {
            var popup = PopupManager.Instance.OpenPopup();
            popup.Open("You Can't Select 3. Select 1, 2, 4");
            return;
        }

        SceneManager.LoadScene("InGame");
    }
}