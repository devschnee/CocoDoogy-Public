using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StageDatabase", menuName = "GameData/StageDatabase")]
public class StageDatabase : ScriptableObject
{
    public List<StageData> stageDataList = new List<StageData>();
}
