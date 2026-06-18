using System.Collections.Generic;


public class SnapShotData
{
    public List<SpaceData> spaceList = new List<SpaceData>();
    public List<DummyData> dummyList = new List<DummyData>();
    public List<ClearDummyData> clearDummyList = new List<ClearDummyData>();
}

public class SpaceData
{
    // Key : 카드 Index 값, Value : 카드 isShow 값
    public List<(int, bool)> cardStateList = new List<(int, bool)>();
}

public class DummyData
{
    public List<int> cardIndexList = new List<int>();
}
public class ClearDummyData
{
    public List<int> cardIndexList = new List<int>();
}