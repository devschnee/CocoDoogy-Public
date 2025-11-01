using UnityEngine;

public class ShopProvider : IDataProvider<int, ShopData>
{
    private ShopDatabase database;
    private IResourceLoader loader;

    public ShopProvider(ShopDatabase db, IResourceLoader resLoader)
    {
        database = db;
        loader = resLoader;
    }

    public ShopData GetData(int id)
    {
        return database.shopDataList.Find(a => a.shop_id == id);
    }

    public Sprite GetIcon(int id)
    {
        var data = GetData(id);
        return data?.GetIcon(loader);
    }
}