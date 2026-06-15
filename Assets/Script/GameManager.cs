using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{   
    public static GameManager Instance { get; private set; }

    public const int SET_COUNT = 2;
    public const int SUIT_COUNT = 4;
    public const int RANK_COUNT = 13;

    public Canvas canvas;
    public List<CardObject> cardObjectList;
    public Transform dragLayer; // 카드 이동 시 Space 뎁스보다 위쪽에 위치하게끔 하기 위함
    public List<Space> spaceList;
    public List<Transform> dummyList;
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
        for (int set = 0; set < 2; set++)
        {
            for (int suit = 0; suit < SUIT_COUNT; suit++)
            {
                for (int rank = 0; rank < RANK_COUNT; rank++)
                {
                    CardData cardData = new CardData()
                    {
                        index = set * (SUIT_COUNT * RANK_COUNT) + suit * RANK_COUNT + rank,
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
            int randomIndex = UnityEngine.Random.Range(0, i + 1);

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
            }
        }
    }

    public void EndTurn()
    {
        foreach (Space space in spaceList)
        {
            space.EndTurn();
        }
    }
}
