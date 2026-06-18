using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Space : MonoBehaviour
{
    [HideInInspector] public List<CardObject> cardList = new List<CardObject>();

    public void Init()
    {
        cardList = new List<CardObject>(GetComponentsInChildren<CardObject>(true));
    }

    public void EndTurn()
    {
        if (cardList != null && cardList.Count > 0)
        {
            var lastCard = cardList.Last();
            // 마지막 카드 안 열려 있으면 오픈
            if (lastCard.isShow == false)
            {
                lastCard.isShow = true;
                lastCard.InitUI();
            }
        }

        // 완료한 수트 있는지 체크
        var completedCardList = GetCompletedCardList();
        if (completedCardList != null)
        {
            InGameManager.Instance.CompleteOneSuit(completedCardList);
        }
    }

    public void AddCardObject(CardObject cardObject)
    {
        cardList.Add(cardObject);
    }

    public void RemoveCardObject(CardObject cardObject)
    {
        cardList.Remove(cardObject);
    }

    public List<CardObject> GetCompletedCardList()
    {
        int suitCount = Enum.GetValues(typeof(CardData.Rank)).Length;

        // 카드 13장 체크
        if (cardList == null || cardList.Count < suitCount)
        {
            return null;
        }

        // 마지막 13장 체크 시작 (킹 > 차례대로 > 에이스)
        int startIndex = cardList.Count - suitCount;

        if (cardList[startIndex].cardData.rank != CardData.Rank.King)
        {
            return null;
        }

        for (int i = startIndex; i < cardList.Count - 1; i++)
        {
            CardObject currentCard = cardList[i];
            CardObject nextCard = cardList[i + 1];

            bool isSameSuit = currentCard.cardData.suit == nextCard.cardData.suit;
            bool isNextRank = currentCard.cardData.rank == nextCard.cardData.rank + 1;

            if (!isSameSuit || !isNextRank)
            {
                return null;
            }
        }

        if (cardList[cardList.Count - 1].cardData.rank != CardData.Rank.Ace)
        {
            return null;
        }

        List<CardObject> completedCardList = new List<CardObject>();
        for (int i = startIndex; i < cardList.Count; i++)
        {
            completedCardList.Add(cardList[i]);
        }

        return completedCardList;
    }
}
