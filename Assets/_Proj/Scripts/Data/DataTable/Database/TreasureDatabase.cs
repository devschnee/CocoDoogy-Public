using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "TreasureDatabase", menuName = "GameData/TreasureDatabase")]
public class TreasureDatabase : ScriptableObject
{
    public List<TreasureData> treasureList = new List<TreasureData>();
}
