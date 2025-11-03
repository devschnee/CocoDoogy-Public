using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CostumeDatabase", menuName = "GameData/CostumeDatabase")]
public class CostumeDatabase : ScriptableObject
{
    public List<CostumeData> costumeList = new List<CostumeData>();
}
