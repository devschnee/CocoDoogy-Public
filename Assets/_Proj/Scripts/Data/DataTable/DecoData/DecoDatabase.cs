using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "DecoDatabase", menuName = "Game Data/Deco Database")]
public class DecoDatabase : ScriptableObject
{
    public List<DecoData> decos = new List<DecoData>();


    // 편의 조회
    public DecoData GetDecoById(int id)
    {
        return decos.Find(d => d.id == id);
    }


#if UNITY_EDITOR
    // 에디터에서 prefabPath/iconPath로 에셋 할당 (AssetDatabase 사용)
    public void ResolveAssets()
    {
        for (int i = 0; i < decos.Count; i++)
        {
            var d = decos[i];
            d.prefab = null;
            d.icon = null;
            if (!string.IsNullOrEmpty(d.prefabPath))
            {
                d.prefab = AssetDatabase.LoadAssetAtPath<GameObject>(d.prefabPath);
            }
            if (!string.IsNullOrEmpty(d.iconPath))
            {
                d.icon = AssetDatabase.LoadAssetAtPath<Sprite>(d.iconPath);
            }
        }
        EditorUtility.SetDirty(this);
    }
#endif
}