using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Dummy : MonoBehaviour
{
    [HideInInspector] public List<CardObject> cardList = new List<CardObject>();

    public void OnClickDummy()
    {
        if (!IsCanOpenDummy())
        {
            return;
        }

        for (int i = 0; i < cardList.Count; i++)
        {
            var space = i < InGameManager.Instance.spaceList.Count ? InGameManager.Instance.spaceList[i] : InGameManager.Instance.spaceList.Last();
            cardList[i].transform.SetParent(space.transform);
            space.AddCardObject(cardList[i]);
        }
        cardList.Clear();
        InGameManager.Instance.EndTurn();
        gameObject.SetActive(false);
    }

    private bool IsCanOpenDummy()
    {
        foreach (var dummy in InGameManager.Instance.dummyList)
        {
            if (dummy.gameObject.activeSelf && dummy == this)
            {
                return true;
            }
        }

        return false;
    }

    public void AddCardObject(CardObject cardObject)
    {
        cardList.Add(cardObject);
    }
}
