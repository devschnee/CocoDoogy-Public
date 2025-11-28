using UnityEngine;

// 각 동물들 소리 및 애니메이션을 추가한다면 각 이벤트 딕셔너리에도 추가해야 함.

interface IAnimalAnimation
{
    void Init(Animator anim, MonoBehaviour mono);
    void OnEnabled();
    void Update();
    void OnDisable();
    void HandleAnimEvent(string eventName);
    void HandleSoundEvent(string eventName);

}