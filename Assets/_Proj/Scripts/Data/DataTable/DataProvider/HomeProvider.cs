using UnityEngine;

public class HomeProvider : IDataProvider<int, HomeData>
{
    private HomeDatabase database;
    private IResourceLoader loader;

    public HomeProvider(HomeDatabase db, IResourceLoader resLoader)
    {
        database = db;
        loader = resLoader;
    }

    public HomeData GetData(int id)
    {
        return database.homeList.Find(a => a.home_id == id);
    }

    public GameObject GetPrefab(int id)
    {
        var data = GetData(id);
        return data?.GetPrefab(loader);
    }
    public Sprite GetIcon(int id)
    {
        var data = GetData(id);
        return data?.GetIcon(loader);
    }
}