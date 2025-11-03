using System;
using UnityEngine;

[Serializable]
public class ChapterData
{
    public string chapter_id;
    public string chapter_name;
    public string chapter_desc;
    public string chapter_img;
    public string[] chapter_staglist;
    public string chapter_bg;

    [NonSerialized] public Sprite chapterIcon;
    [NonSerialized] public Sprite chapterBgIcon;

    public Sprite GetChapterIcon(IResourceLoader loader)
    {
        if (chapterIcon == null && !string.IsNullOrEmpty(chapter_img))
            chapterIcon = loader.LoadSprite(chapter_img);
        return chapterIcon;
    }

    public Sprite GetChapterBgIcon(IResourceLoader loader)
    {
        if (chapterBgIcon == null && !string.IsNullOrEmpty(chapter_bg))
            chapterBgIcon = loader.LoadSprite(chapter_bg);
        return chapterBgIcon;
    }
}
