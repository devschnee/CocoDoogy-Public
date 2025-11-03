using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestDatabase", menuName = "GameData/QuestDatabase")]
public class QuestDatabase : ScriptableObject
{
    public List<QuestData> questList = new List<QuestData>();
}
