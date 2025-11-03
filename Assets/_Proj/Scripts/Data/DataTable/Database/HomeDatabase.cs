using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HomeDatabase", menuName = "GameData/HomeDatabase")]
public class HomeDatabase : ScriptableObject
{
    public List<HomeData> homeList = new List<HomeData>();
}
