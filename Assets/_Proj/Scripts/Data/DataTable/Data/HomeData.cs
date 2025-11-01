using System;
using UnityEngine;

[Serializable]
public class HomeData
{
    public int home_id;
    public string home_name;
    public string home_prefab;
    public string home_icon;
    public HomeTag home_tag;
    public HomeAcquire home_acquire;
    public string home_desc;

    [NonSerialized] public GameObject prefab;
    [NonSerialized] public Sprite icon;
    public GameObject GetPrefab(IResourceLoader loader)
    {
        if (prefab == null && !string.IsNullOrEmpty(home_prefab))
            prefab = loader.LoadPrefab(home_prefab);
        return prefab;
    }

    public Sprite GetIcon(IResourceLoader loader)
    {
        if (icon == null && !string.IsNullOrEmpty(home_icon))
            icon = loader.LoadSprite(home_icon);
        return icon;
    }
}

public enum HomeTag
{
    basic, xmas, halloween
}
public enum HomeAcquire
{
    quest, ingame, shop
}