using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public enum RuleType
{
    OneSuit = 1,
    TwoSuit = 2,
    FourSuit = 4
}

public class InGameManager : MonoBehaviour
{   
    public static InGameManager Instance { get; private set; }

    public const int RANK_COUNT = 13;
    public const int CARD_COUNT = 104;
    public const int SET_COUNT = 8;

    private int score;
    private int move;
    private int completeSet;

    public int Score
    {
        get => score;
        set
        {
            score = value;
            UIManager.Instance.SetScoreText();
        }
    }
    public int Move
    {
        get => move;
        set
        {
            move = value;
            UIManager.Instance.SetMoveText();
        }
    }
    public int CompleteSet
    {
        get => completeSet;
        set
        {
            completeSet = value;
            UIManager.Instance.SetCompleteSetText();
        }
    }

    public Canvas canvas;
    public List<CardObject> cardObjectList;
    public Transform dragLayer; // 카드 이동 시 Space 뎁스보다 위쪽에 위치하게끔 하기 위함
    public List<Space> spaceList;
    public List<Dummy> dummyList;
    public List<ClearDummy> clearDummyList;
    public CardObject cardPrefab;

    private List<CardData> cardDataList = new List<CardData>();
    private Stack<SnapShotData> snapShotDataStack = new Stack<SnapShotData>();

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
        SaveSnapshot();
    }

    private void StartGame()
    {
        CreateCardData();
        ShuffleCardData();
        CreateCardObject();
        InitData();

        foreach (var space in spaceList)
        {
            space.Init();
        }
    }

    private void InitData()
    {
        Score = 0;
        Move = 0;
        CompleteSet = 0;
    }

    private void CreateCardData()
    {
        int ruleType = RuleManager.Instance == null ? 1 : (int) RuleManager.Instance.cardTypeList.Count;

        int setCount = CARD_COUNT / (ruleType * RANK_COUNT);
        for (int set = 0; set < setCount; set++)
        {
            for (int suit = 0; suit < ruleType; suit++)
            {
                for (int rank = 0; rank < RANK_COUNT; rank++)
                {
                    CardData cardData = new CardData()
                    {
                        index = set * (ruleType * RANK_COUNT) + suit * RANK_COUNT + rank,
                        suit = RuleManager.Instance.cardTypeList[suit],
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

    public void EndTurn()
    {
        foreach (Space space in spaceList)
        {
            space.EndTurn();
        }

        Move++;
        SetCompleteSetCount();
        SaveSnapshot();
    }

    public void SetCompleteSetCount()
    {
        int tempCount = 0;
        foreach (ClearDummy clearDummy in clearDummyList)
        {
            if (clearDummy.transform.childCount > 0)
            {
                tempCount++;
            }
        }
        CompleteSet = tempCount;
    }

    public void GetHint()
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

    public void Undo()
    {
        if (snapShotDataStack == null || snapShotDataStack.Count <= 1)
        {
            return;
        }

        SnapShotData snapshot = snapShotDataStack.Pop();

        if (snapShotDataStack.Count == 0)
        {
            return;
        }

        RestoreSnapshot(snapShotDataStack.Peek());
        Move++;
        SetCompleteSetCount();
    }
    public void Restart()
    {
        if (snapShotDataStack == null)
        {
            return;
        }

        RestoreSnapshot(snapShotDataStack.Last());
        snapShotDataStack.Clear();
        SaveSnapshot();
        InitData();
    }

    private void SaveSnapshot()
    {
        SnapShotData snapshot = new();
        foreach (var space in spaceList)
        {
            if (space.cardList != null && space.cardList.Count > 0)
            {
                var spaceData = new SpaceData();
                foreach (var card in space.cardList)
                {
                    spaceData.cardStateList.Add((card.cardData.index, card.isShow));
                }
                snapshot.spaceList.Add(spaceData);
            }
        }
        foreach (var dummy in dummyList)
        {
            if (dummy.cardList != null && dummy.cardList.Count > 0)
            {
                var dummyData = new DummyData();
                foreach (var card in dummy.cardList)
                {
                    dummyData.cardIndexList.Add(card.cardData.index);
                }
                snapshot.dummyList.Add(dummyData);
            }
        }
        foreach (var clearDummy in clearDummyList)
        {
            if (clearDummy.cardList != null && clearDummy.cardList.Count > 0)
            {
                var clearDummyData = new ClearDummyData();
                foreach (var card in clearDummy.cardList)
                {
                    clearDummyData.cardIndexList.Add(card.cardData.index);
                }
                snapshot.clearDummyList.Add(clearDummyData);
            }
        }

        snapShotDataStack.Push(snapshot);
    }

    private void RestoreSnapshot(SnapShotData snapshot)
    {
        foreach (Space space in spaceList)
        {
            space.cardList.Clear();
        }

        foreach (Dummy dummy in dummyList)
        {
            dummy.cardList.Clear();
        }

        foreach (ClearDummy clearDummy in clearDummyList)
        {
            clearDummy.cardList.Clear();
        }

        for (int i = 0; i < snapshot.spaceList.Count; i++)
        {
            Space space = spaceList[i];
            SpaceData spaceData = snapshot.spaceList[i];

            foreach (var pair in spaceData.cardStateList)
            {
                int cardIndex = pair.Item1;
                bool isShow = pair.Item2;

                CardObject card = cardObjectList.Find(x => x.cardData.index == cardIndex);

                if (card == null)
                {
                    continue;
                }

                card.isShow = isShow;

                card.transform.SetParent(space.transform);
                space.AddCardObject(card);

                card.InitUI();
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(space.GetComponent<RectTransform>());
            space.RefreshSpacing();
        }

        for (int i = 0; i < snapshot.dummyList.Count; i++)
        {
            Dummy dummy = dummyList[i];
            DummyData dummyData = snapshot.dummyList[i];

            foreach (int cardIndex in dummyData.cardIndexList)
            {
                CardObject card = cardObjectList.Find(x => x.cardData.index == cardIndex);

                card.transform.SetParent(dummy.transform);
                card.transform.localPosition = Vector2.zero;
                dummy.AddCardObject(card);
                card.isShow = false;
                card.InitUI();
            }
            dummy.gameObject.SetActive(true);
        }

        for (int i = 0; i < snapshot.clearDummyList.Count; i++)
        {
            ClearDummy clearDummy = clearDummyList[i];
            ClearDummyData clearDummyData = snapshot.clearDummyList[i];

            foreach (int cardIndex in clearDummyData.cardIndexList)
            {
                CardObject card = cardObjectList.Find(x => x.cardData.index == cardIndex);

                card.transform.SetParent(clearDummy.transform);
                card.transform.localPosition = Vector2.zero;
                clearDummy.AddCardObject(card);
                clearDummy.IsClear = true;
                card.InitUI();
            }
        }

        if (snapshot.clearDummyList.Count == 0)
        {
            foreach (var clearDummy in clearDummyList)
            {
                clearDummy.IsClear = false;
            }
        }
    }

    public void CompleteOneSuit(List<CardObject> completedCardList)
    {
        foreach (var clearDummy in clearDummyList)
        {
            if (clearDummy.transform.childCount == 0)
            {
                clearDummy.IsClear = true;
                clearDummy.cardList = completedCardList;

                foreach (var card in completedCardList)
                {
                    card.isShow = false;
                    card.transform.SetParent(clearDummy.transform);
                    card.transform.localPosition = Vector2.zero;
                }
                return;
            }
        }
    }

    public void CompleteAllSuit()
    {
        Debug.Log($"CompleteAllSuit");
    }
}
