using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ArtifactData", menuName = "GameData/ArtifactData")]
public class ArtifactDatabase : ScriptableObject
{
    public List<ArtifactData> artifactList = new List<ArtifactData>();
}
