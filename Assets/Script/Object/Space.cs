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
}
