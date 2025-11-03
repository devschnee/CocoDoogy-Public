using UnityEngine;

public class Shop_itemProvider : IDataProvider<int, Shop_itemData>
{
    private Shop_itemDatabase database;
    private IResourceLoader loader;

    public Shop_itemProvider(Shop_itemDatabase db, IResourceLoader resLoader)
    {
        database = db;
        loader = resLoader;
    }

    public Shop_itemData GetData(int id)
    {
        return database.shopItemList.Find(a => a.shop_item_id == id);
    }
}
