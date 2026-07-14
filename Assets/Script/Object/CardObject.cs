using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardObject : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public CardData cardData;
    public bool isShow = false;

    private RectTransform rectTransform;
    private Space beforeSpace;
    private List<CardObject> cardList = new List<CardObject>();

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void InitUI()
    {
        if (isShow)
        {
            GetComponent<Image>().sprite = Resources.Load<Sprite>($"UI/Card/{(int)cardData.suit}_{(int)cardData.rank}");
        }
        else
        {
            GetComponent<Image>().sprite = Resources.Load<Sprite>($"UI/Card/card");
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        beforeSpace = this.gameObject.GetComponentInParent<Space>();

        if (!IsCanBeginDrag())
        {
            return;
        }

        // originalParent 설정 후 DragLayer로 카드 부모 변경
        foreach (CardObject card in cardList)
        {
            card.transform.SetParent(InGameManager.Instance.dragLayer);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!IsCanBeginDrag())
        {
            return;
        }

        foreach (CardObject card in cardList)
        {
            card.rectTransform.anchoredPosition += eventData.delta / InGameManager.Instance.canvas.scaleFactor;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!IsCanBeginDrag())
        {
            return;
        }

        Space afterSpace = FindNearestSpace();
        
        // 선택한 Space로 이동
        if (afterSpace != null && IsCanEndDrag(afterSpace))
        {
            foreach (CardObject card in cardList)
            {
                beforeSpace.RemoveCardObject(card);
                afterSpace.AddCardObject(card);
                card.transform.SetParent(afterSpace.transform);
                card.transform.SetSiblingIndex(afterSpace.transform.childCount - 1);
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(afterSpace.GetComponent<RectTransform>());
        }
        // Space 미선택 시 기존 Space로 복귀
        else
        {
            foreach (CardObject card in cardList)
            {
                card.transform.SetParent(beforeSpace.transform);
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(beforeSpace.GetComponent<RectTransform>());
        }

        InGameManager.Instance.EndTurn();
    }

    /// <summary>
    /// 드래그 후 선택된 Space 리턴
    /// </summary>
    private Space FindNearestSpace()
    {
        List<Space> spaces = InGameManager.Instance.spaceList;

        Space nearestSpace = null;
        float minDistance = float.MaxValue;

        // Space 근처에 커서 위치 시
        foreach (Space space in spaces)
        {
            float nearestDistanceInSpace = Vector2.Distance(
                rectTransform.position,
                space.GetComponent<RectTransform>().position);

            // Space 아래 카드들에 커서 위치 시
            foreach (CardObject card in space.cardList)
            {
                if (card == this)
                {
                    continue;
                }

                float cardDistance = Vector2.Distance(
                    rectTransform.position,
                    card.transform.position);

                if (cardDistance < nearestDistanceInSpace)
                {
                    nearestDistanceInSpace = cardDistance;
                }
            }

            if (nearestDistanceInSpace < minDistance)
            {
                minDistance = nearestDistanceInSpace;
                nearestSpace = space;
            }
        }

        return minDistance <= 150f ? nearestSpace : null;
    }

    /// <summary>
    /// 드래그 가능 조건 : (해당 카드 포함) 마지막 카드가 해당 카드의 리스트에 포함되어 있을 떄
    /// </summary>
    private bool IsCanBeginDrag()
    {
        if (!isShow)
        {
            return false;
        }

        cardList = GetCardList(beforeSpace);
        if (cardList == null || cardList.Count == 0)
        {
            return false;
        }

        if (beforeSpace.cardList.Last().cardData.index == cardList.Last().cardData.index)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// IsCanBeginDrag과 기능은 동일하나, 힌트 기능 추가를 위해 함수 분리
    /// </summary>
    public bool IsCanBeginHint(Space space)
    {
        if (!isShow)
            return false;

        var movableCards = GetCardList(space);

        if (movableCards.Count == 0)
            return false;

        return space.cardList.Last() == movableCards.Last();
    }

    /// <summary>
    /// 해당 카드 아래 리스트 리턴
    /// </summary>
    private List<CardObject> GetCardList(Space space)
    {
        List<CardObject> tempCardList = new List<CardObject>();
        CardObject tempCard = this;
        tempCardList.Add(tempCard);

        int tempIndex = space.cardList.IndexOf(this);
        for (int i = tempIndex; i < space.cardList.Count; i++)
        {
            var card = space.cardList[i];
            // 리스트에 추가되는 카드 개별 조건 : 1. 동일 수트, 2. 다음 랭크 (내림차순), 3. 다음 순서
            bool isSameSuit = card.cardData.suit == tempCard.cardData.suit;
            bool isNextRank = card.cardData.rank == tempCard.cardData.rank - 1;
            bool isNextCard = i == tempIndex + 1;

            if (isSameSuit && isNextRank && isNextCard)
            {
                tempCard = card;
                tempCardList.Add(tempCard);
                tempIndex = i;
            }
        }
        return tempCardList;
    }

    public bool IsCanEndDrag(Space afterSpace)
    {
        if (afterSpace.cardList == null || afterSpace.cardList.Count == 0)
        {
            return true;
        }

        var card = afterSpace.cardList.Last();
        if (card.cardData.rank == this.cardData.rank + 1)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// IsCanEndDrag에 수트 조건 추가 (힌트 기능에 사용)
    /// </summary>
    public bool IsCanEndDragOnSameSuit(Space afterSpace)
    {
        if (afterSpace.cardList == null || afterSpace.cardList.Count == 0)
        {
            return true;
        }

        var card = afterSpace.cardList.Last();
        bool isSameSuit = card.cardData.suit == this.cardData.suit;
        bool isPrevRank = card.cardData.rank == this.cardData.rank + 1;
        if (isSameSuit && isPrevRank)
        {
            return true;
        }

        return false;
    }
}
