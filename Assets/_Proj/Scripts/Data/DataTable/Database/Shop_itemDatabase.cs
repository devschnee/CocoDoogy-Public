using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Shop_itemDatabase", menuName = "GameData/Shop_itemDatabase")]
public class Shop_itemDatabase : ScriptableObject
{
    public List<Shop_itemData> shopItemList = new List<Shop_itemData>();
}
