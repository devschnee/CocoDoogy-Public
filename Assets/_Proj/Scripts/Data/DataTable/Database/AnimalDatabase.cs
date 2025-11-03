using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimalDatabase", menuName = "GameData/AnimalDatabase")]
public class AnimalDatabase : ScriptableObject
{
    public List<AnimalData> animalList = new List<AnimalData>();
}
