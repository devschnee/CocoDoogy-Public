using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;





public class BlockFactory : MonoBehaviour
{
    //전역으로 접근할 필요 없음. StageManager가 이 클래스의 객체를 하나만 알고 있으면 됨.

    [Header("등록된 블록 데이터")]
    public List<BlockData> allBlocks = new List<BlockData>();


    public GameObject CreateBlock(BlockSaveData data)
    {
        if (data == null) return null;

        Vector3Int position = data.position;
        Quaternion rotation = data.rotation;
        var blockPrefab = FindBlockPrefab(data.blockName);

        var obj = Instantiate(blockPrefab, position, rotation);
        

        return obj;
    }


    

    public GameObject FindBlockPrefab(string blockName)
    {
        BlockData data = allBlocks.Find(x => x.blockName == blockName);
        if (data == null)
        {
            Debug.LogWarning($"BlockFactory: '{blockName}' 데이터를 찾을 수 없습니다.");
            return null;
        }
        return data.prefab;
    }
}