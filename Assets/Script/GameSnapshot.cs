using System.Collections.Generic;

public class GameSnapshot
{
    public List<CardState> cardStates = new();
}

public class CardState
{
    public int cardIndex;

    public bool isShow;

    public int parentType;    // 0 = Space, 1 = Dummy

    public int parentIndex;

    public int siblingIndex;
}