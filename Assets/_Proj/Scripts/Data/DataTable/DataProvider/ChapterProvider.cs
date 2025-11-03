using UnityEngine;

public class ChapterProvider : IDataProvider<string, ChapterData>
{
    private ChapterDatabase database;
    private IResourceLoader loader;

    public ChapterProvider(ChapterDatabase db, IResourceLoader resLoader)
    {
        database = db;
        loader = resLoader;
    }

    public ChapterData GetData(string id)
    {
        return database.chapterList.Find(a => a.chapter_id == id);
    }

    public Sprite GetChapterIcon(string id)
    {
        var data = GetData(id);
        return data?.GetChapterIcon(loader);
    }

    public Sprite GetChapterBgIcon(string id)
    {
        var data = GetData(id);
        return data?.GetChapterBgIcon(loader);
    }
}
