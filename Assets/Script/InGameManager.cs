using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class InGameManager : MonoBehaviour
{   
    public static InGameManager Instance { get; private set; }

    public const int RANK_COUNT = 13;
    public const int CARD_COUNT = 104;

    public Canvas canvas;
    public List<CardObject> cardObjectList;
    public Transform dragLayer; // 카드 이동 시 Space 뎁스보다 위쪽에 위치하게끔 하기 위함
    public List<Space> spaceList;
    public List<Dummy> dummyList;
    public CardObject cardPrefab;

    private List<CardData> cardDataList = new List<CardData>();

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        StartGame();
    }

    private void StartGame()
    {
        CreateCardData();
        ShuffleCardData();
        CreateCardObject();

        foreach (var space in spaceList)
        {
            space.Init();
        }
    }

    private void CreateCardData()
    {
        int setCount = CARD_COUNT / ((int)RuleManager.Instance.ruleType * RANK_COUNT);
        for (int set = 0; set < setCount; set++)
        {
            for (int suit = 0; suit < (int) RuleManager.Instance.ruleType; suit++)
            {
                for (int rank = 0; rank < RANK_COUNT; rank++)
                {
                    CardData cardData = new CardData()
                    {
                        index = set * ((int)RuleManager.Instance.ruleType * RANK_COUNT) + suit * RANK_COUNT + rank,
                        suit = (CardData.Suit)suit,
                        rank = (CardData.Rank)rank,
                    };

                    cardDataList.Add(cardData);
                }
            }
        }
    }

    private void ShuffleCardData()
    {
        for (int i = cardDataList.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);

            CardData temp = cardDataList[i];
            cardDataList[i] = cardDataList[randomIndex];
            cardDataList[randomIndex] = temp;
        }
    }

    /// <summary>
    /// 카드 배치 : Space(54) > 6 6 6 6 5 5 5 5 5 5 / Dummy(50) > 10 10 10 10 10
    /// </summary>
    private void CreateCardObject()
    {
        List<int> spaceInitCount = new List<int> { 6, 6, 6, 6, 5, 5, 5, 5, 5, 5 };
        int cardIndex = 0;

        for (int i = 0; i < spaceList.Count; i++)
        {
            for (int j = 0; j < spaceInitCount[i]; j++)
            {
                CardObject cardObject = Instantiate(cardPrefab, spaceList[i].transform);
                cardObject.cardData = cardDataList[cardIndex];
                cardObjectList.Add(cardObject);
                cardIndex++;
                if (j == spaceInitCount[i] - 1)
                {
                    cardObject.isShow = true;
                }
                cardObject.InitUI();
                spaceList[i].AddCardObject(cardObject);
            }
        }

        for (int i = 0; i < dummyList.Count; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                CardObject cardObject = Instantiate(cardPrefab, dummyList[i].transform);
                cardObject.cardData = cardDataList[cardIndex];
                cardObjectList.Add(cardObject);
                cardIndex++;
                dummyList[i].AddCardObject(cardObject);
            }
        }
    }

    public void EndDummyOpen(Dummy dummy)
    {
        dummyList.Remove(dummy);
        EndTurn();
    }

    public void EndTurn()
    {
        foreach (Space space in spaceList)
        {
            space.EndTurn();
        }
    }

    public void OnClickHintButton()
    {
        foreach (var card in GetHintCardList())
        {
            // 힌트 1순위 : 동일 수트 이동
            foreach (var space in spaceList)
            {
                if (card.IsCanEndDragOnSameSuit(space))
                {
                    var movingCard = card.cardData;
                    var targetCard = space.cardList.Last().cardData;
                    Debug.Log($"Move {movingCard.suit}-{movingCard.rank} to {targetCard.suit}-{targetCard.rank}");
                    return;
                }
            }
        }

        foreach (var card in GetHintCardList())
        {
            // 힌트 2순위 : 다른 수트 이동
            foreach (var space in spaceList)
            {
                if (card.IsCanEndDrag(space))
                {
                    var movingCard = card.cardData;
                    var targetCard = space.cardList.Last().cardData;
                    Debug.Log($"Move {movingCard.suit}-{movingCard.rank} to {targetCard.suit}-{targetCard.rank}");
                    return;
                }
            }
        }

        // 힌트 3순위 : 더미 오픈
        foreach (var dummy in dummyList)
        {
            if (dummy.gameObject.activeSelf)
            {
                Debug.Log($"Open Dummy");
                return;
            }
        }

        // 이동 불가 시 게임 오버
        Debug.Log("Can't Move / Game Over");
    }

    private List<CardObject> GetHintCardList()
    {
        List<CardObject> hintCardList = new List<CardObject>();
        foreach (var space in spaceList)
        {
            foreach (var card in space.cardList)
            {
                if (card.IsCanBeginHint(space))
                {
                    hintCardList.Add(card);
                }
            }
        }
        return hintCardList;
    }
}
