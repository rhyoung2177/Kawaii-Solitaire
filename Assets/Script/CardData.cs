using UnityEngine;

public class CardData
{
    public Suit suit;
    public Rank rank;

    public enum Suit
    {
        Spade = 0,
        Diamond = 1,
        Heart = 2,
        Clover = 3
    }

    public enum Rank
    {
        Ace = 0,
        Two = 1,
        Three = 2,
        Four = 3,
        Five = 4,
        Six = 5,
        Seven = 6,
        Eight = 7,
        Nine = 8,
        Ten = 9,
        Jack = 10,
        Queen = 11,
        King = 12
    }
}
