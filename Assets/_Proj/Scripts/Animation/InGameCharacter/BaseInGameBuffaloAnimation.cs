using System;
using System.Collections.Generic;
using UnityEngine;

public class BaseInGameBuffaloAnimation : IAnimalAnimation
{
    private Animator anim;
    private MonoBehaviour mono;
    private Buffalo buffalo;
    public Dictionary<string,Action> animEventDictionary;
    public Dictionary<string,Action> soundEventDictionary;


    #region  애니메이션

    private void AnimIdle() { anim.Play("Idle_A"); }

    /// <summary>
    /// 버팔로 점프 후 쾅
    /// </summary>
    public void ChangeJumpToBounce() { anim.SetTrigger("Bomb"); }

    /// <summary>
    /// 버팔로 스킬 준비 Buffalo.cs 이벤트 Invoke
    /// </summary>
    public void HandleStartBuffaloJumpAnim() { anim.SetTrigger("Lets"); }
    
    #endregion

    #region 소리

    private void SoundCharging()
    {
        AudioEvents.Raise(SFXKey.InGameBuffalo, 0, loop: false, pooled: true, pos: mono.transform.position);
    }
    private void SoundBomb()
    {
        AudioEvents.Raise(SFXKey.InGameBuffalo, 1, loop: false, pooled: true, pos: mono.transform.position);
    }

    #endregion

    #region  IAnimalAnimation 인터페이스 영역

    public void Init(Animator anim, MonoBehaviour mono)
    {
        this.anim = anim;
        this.mono = mono;
        buffalo = mono.GetComponentInParent<Buffalo>();

        animEventDictionary = new Dictionary<string, Action>
        {
            { "Anim_Idle", AnimIdle },
            { "Anim_Bounce", ChangeJumpToBounce }
        };
        soundEventDictionary = new Dictionary<string, Action>
        {
            { "Sound_Charge", SoundCharging },
            { "Sound_Bomb", SoundBomb }
        };
    }

    public void OnEnabled()
    {
        buffalo.OnBombStart += HandleStartBuffaloJumpAnim;
    }

    public void Update() { }

    public void OnDisable()
    {
        buffalo.OnBombStart -= HandleStartBuffaloJumpAnim;
    }

    public void HandleAnimEvent(string eventName)
    {
        if (animEventDictionary.TryGetValue(eventName, out Action action))
        {
            action.Invoke();
        }
    }

    public void HandleSoundEvent(string eventName)
    {
        if (soundEventDictionary.TryGetValue(eventName, out Action action))
        {
            action.Invoke();
        }
    }
    
    #endregion
}
