using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BackgroundDatabase", menuName = "GameData/BackgroundDatabase")]
public class BackgroundDatabase : ScriptableObject
{
    public List<BackgroundData> bgList = new List<BackgroundData>();
}
