using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ChapterDatabase", menuName = "GameData/ChapterDatabase")]
public class ChapterDatabase : ScriptableObject
{
    public List<ChapterData> chapterList = new List<ChapterData>();
}
