using System;
using UnityEngine;

[Serializable]
public class AnimalData
{
    public int animal_id;
    public string animal_name;
    public AnimalType animal_type;
    public AnimalTag animal_tag;
    public string animal_prefab;
    public string animal_icon;
    public AnimalAcquire animal_acquire;
    public string animal_desc;

    [NonSerialized] public GameObject prefab;
    [NonSerialized] public Sprite icon;
    public GameObject GetPrefab(IResourceLoader loader)
    {
        if (prefab == null && !string.IsNullOrEmpty(animal_prefab))
            prefab = loader.LoadPrefab(animal_prefab);
        return prefab;
    }

    public Sprite GetIcon(IResourceLoader loader)
    {
        if (icon == null && !string.IsNullOrEmpty(animal_icon))
            icon = loader.LoadSprite(animal_icon);
        return icon;
    }
}
public enum AnimalType
{
    pig, turtle, cow, horse, bird
}
public enum AnimalTag
{
    basic, xmas, halloween
}
public enum AnimalAcquire
{
    quest, ingame, shop
}
