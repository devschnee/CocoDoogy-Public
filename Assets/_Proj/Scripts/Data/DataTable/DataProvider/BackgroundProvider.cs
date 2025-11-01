using UnityEngine;

public class BackgroundProvider : IDataProvider<int, BackgroundData>
{
    private BackgroundDatabase database;
    private IResourceLoader loader;

    public BackgroundProvider(BackgroundDatabase db, IResourceLoader resloader)
    {
        database = db;
        loader = resloader;
    }

    public BackgroundData GetData(int id)
    {
        return database.bgList.Find(a => a.bg_id == id);
    }

    public Material GetMaterial(int id)
    {
        var data = GetData(id);
        return data?.GetMaterial(loader);
    }
    public Sprite GetIcon(int id)
    {
        var data = GetData(id);
        return data?.GetIcon(loader);
    }

}
