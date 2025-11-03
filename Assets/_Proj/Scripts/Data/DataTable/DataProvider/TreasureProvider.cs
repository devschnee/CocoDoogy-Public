using UnityEngine;

public class TreasureProvider : IDataProvider<string, TreasureData>
{
    private TreasureDatabase database;
    private IResourceLoader loader;

    public TreasureProvider(TreasureDatabase db, IResourceLoader resLoader)
    {
        database = db;
        loader = resLoader;
    }

    public TreasureData GetData(string id)
    {
        return database.treasureList.Find(a => a.treasure_id == id);
    }
}
