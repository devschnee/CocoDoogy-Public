using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShopDatabase", menuName = "GameData/ShopDatabase")]
public class ShopDatabase : ScriptableObject
{
    public List<ShopData> shopDataList = new List<ShopData>();
}
