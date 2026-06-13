using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardObject : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public CardData.Rank rank;
    public CardData.Suit suit;

    public TextMeshProUGUI tempText;

    private RectTransform rectTransform;
    private Space originalParent;
    private CardData cardData = new CardData();

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Start()
    {
        InitData();
        InitUI();
    }

    private void InitData()
    {
        cardData.rank = rank;
        cardData.suit = suit;
    }

    private void InitUI()
    {
        tempText.text = $"{cardData.rank}-{cardData.suit}";
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = this.gameObject.GetComponentInParent<Space>();

        // Space의 가장 마지막 카드만 이동 가능
        if (originalParent.cardList[originalParent.cardList.Count - 1] != this)
            return;

        // originalParent 설정 후 DragLayer로 카드 부모 변경
        transform.SetParent(GameManager.Instance.dragLayer);

    }

    public void OnDrag(PointerEventData eventData)
    {
        // Space의 가장 마지막 카드만 이동 가능
        if (originalParent.cardList[originalParent.cardList.Count - 1] != this)
            return;

        rectTransform.anchoredPosition += eventData.delta / GameManager.Instance.canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Space의 가장 마지막 카드만 이동 가능
        if (originalParent.cardList[originalParent.cardList.Count - 1] != this)
            return;

        Space nearestSpace = FindNearestSpace();
        
        // 선택한 Space로 이동
        if (nearestSpace != null)
        {
            originalParent.RemoveCardObject(this);
            nearestSpace.AddCardObject(this);

            transform.SetParent(nearestSpace.transform);
            transform.SetSiblingIndex(nearestSpace.transform.childCount - 1);
            LayoutRebuilder.ForceRebuildLayoutImmediate(nearestSpace.GetComponent<RectTransform>());
        }
        // Space 미선택 시 기존 Space로 복귀
        else
        {
            transform.SetParent(originalParent.transform);
            LayoutRebuilder.ForceRebuildLayoutImmediate(originalParent.GetComponent<RectTransform>());
        }
    }

    /// <summary>
    /// 드래그 후 선택된 Space 리턴
    /// </summary>
    private Space FindNearestSpace()
    {
        List<Space> spaces = GameManager.Instance.spaceList;

        Space nearestSpace = null;
        float minDistance = float.MaxValue;

        // Space 근처에 커서 위치 시
        foreach (Space space in spaces)
        {
            float nearestDistanceInSpace = Vector2.Distance(
                rectTransform.position,
                space.GetComponent<RectTransform>().position);

            List<CardObject> cardList = space.cardList;

            // Space 아래 카드들에 커서 위치 시
            foreach (CardObject card in cardList)
            {
                if (card == this)
                    continue;

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

}
