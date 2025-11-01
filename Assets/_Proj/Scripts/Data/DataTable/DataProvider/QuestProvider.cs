using UnityEngine;

public class QuestProvider : IDataProvider<int, QuestData>
{
    private QuestDatabase database;
    private IResourceLoader loader;

    public QuestProvider(QuestDatabase db, IResourceLoader resLoader)
    {
        database = db;
        loader = resLoader;
    }

    public QuestData GetData(int id)
    {
        return database.questList.Find(a => a.quest_id == id);
    }
}
