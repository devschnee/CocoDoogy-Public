using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CodexDatabase", menuName = "Scriptable Objects/CodexDatabase")]
public class CodexDatabase : ScriptableObject
{
    public List<CodexData> codexList = new List<CodexData>();
}
