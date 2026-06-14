using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Space : MonoBehaviour
{
    public GridLayoutGroup grid;
    [HideInInspector] public List<CardObject> cardList = new List<CardObject>();

    private void Start()
    {
        cardList = new List<CardObject>(GetComponentsInChildren<CardObject>(true));
    }

    public void AddCardObject(CardObject cardObject)
    {
        cardList.Add(cardObject);
    }

    public void RemoveCardObject(CardObject cardObject)
    {
        cardList.Remove(cardObject);
    }

    public bool HasCompletedSuit()
    {
        int suitCount = Enum.GetValues(typeof(CardData.Rank)).Length;

        // 카드 13장 체크
        if (cardList == null || cardList.Count < suitCount)
            return false;

        // 마지막 13장 체크 시작 (킹 > 차례대로 > 에이스)
        int startIndex = cardList.Count - suitCount;

        if (cardList[startIndex].rank != CardData.Rank.King)
            return false;

        for (int i = startIndex; i < cardList.Count - 1; i++)
        {
            CardObject currentCard = cardList[i];
            CardObject nextCard = cardList[i + 1];

            bool isSameSuit = currentCard.suit == nextCard.suit;
            bool isNextRank = currentCard.rank == nextCard.rank + 1;

            if (!isSameSuit || !isNextRank)
                return false;
        }

        if (cardList[cardList.Count - 1].rank != CardData.Rank.Ace)
            return false;

        return true;
    }
}
