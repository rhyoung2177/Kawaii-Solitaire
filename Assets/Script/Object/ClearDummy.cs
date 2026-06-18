using System.Collections.Generic;
using UnityEngine;

public class ClearDummy : MonoBehaviour
{
    [HideInInspector] public List<CardObject> cardList = new List<CardObject>();
    public bool IsClear = false;

    public void AddCardObject(CardObject cardObject)
    {
        cardList.Add(cardObject);
    }
}
