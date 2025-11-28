using System;
using UnityEngine;

public static class ETCEvent
{
    public static event Action OnSaveAnimalPositions;
    public static event Action<Transform> OnDeleteAnimalPosition;
    public static event Action OnCocoInteractSoundInGame;

    /// <summary>
    /// 로비에 있는 동물들의 위치를 저장
    /// </summary>
    public static void InvokeSaveAnimalPos()
    {
        Debug.Log("동물들아 너네 위치 저장해라");
        OnSaveAnimalPositions?.Invoke();
    }

    /// <summary>
    /// 로비에 있는 동물을 인벤토리로 보낼 때 해당 동물의 저장된 포지션 값 삭제
    /// </summary>
    /// <param name="target">로비에 있는 동물</param>
    public static void InvokeDeleteAnimalPos(Transform target)
    {
        Debug.Log($"{target}아 잘가");
        OnDeleteAnimalPosition?.Invoke(target);
    }

    public static void InvokeCocoInteractSoundInGame()
    {
        OnCocoInteractSoundInGame?.Invoke();
    }
}
