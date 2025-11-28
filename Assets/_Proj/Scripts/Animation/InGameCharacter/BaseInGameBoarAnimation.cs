using System;
using System.Collections.Generic;
using UnityEngine;

public class BaseInGameBoarAnimation : IAnimalAnimation
{
    private Animator anim;
    private MonoBehaviour mono;
    private Boar boar;
    private bool hello;
    public Dictionary<string,Action> animEventDictionary;
    public Dictionary<string,Action> soundEventDictionary;

    #region 애니메이션

    private void AnimIdle() { anim.Play("Idle_A"); }

    /// <summary>
    /// Boar.cs 에서 Event 받음
    /// </summary>
    public void HandleStartPushPush()
    {
        SoundPush();
        anim.SetTrigger("Push");
    }

    #endregion

    #region 사운드

    private void SoundRunOne()
    {
        int index = UnityEngine.Random.Range(1, 4);
        AudioEvents.Raise(SFXKey.InGameHog, index, loop: false, pooled: true, pos: mono.transform.position);
    }

    private void SoundRunTwo()
    {
        int index = UnityEngine.Random.Range(4, 7);
        AudioEvents.Raise(SFXKey.InGameHog, index, loop: false, pooled: true, pos: mono.transform.position);
    }

    private void SoundPush()
    {
        AudioEvents.Raise(SFXKey.InGameHog, 0, loop: false, pooled: true, pos: mono.transform.position);
    }

    #endregion

    #region IAnimalAnimation 영역

    public void Init(Animator anim, MonoBehaviour mono)
    {
        this.anim = anim;
        this.mono = mono;
        boar = mono.GetComponentInParent<Boar>();

        animEventDictionary = new Dictionary<string, Action>
        {
            { "Anim_Idle", AnimIdle }
        };
        soundEventDictionary = new Dictionary<string, Action>
        {
            { "Sound_RunOne", SoundRunOne },
            { "Sound_RunTwo", SoundRunTwo },
            { "Sound_Push", SoundPush }
        };
    
    }

    public void OnEnabled()
    {
        boar.OnPushStart += HandleStartPushPush;
    }

    public void Update()
    {
        hello = boar.IsMoving;
        anim.SetBool("Run", hello);
    }

    public void OnDisable()
    {
        boar.OnPushStart -= HandleStartPushPush;
    }

    public void HandleAnimEvent(string eventName)
    {
        if (animEventDictionary.TryGetValue(eventName, out Action action))
        {
            action?.Invoke();
        }
    }

    public void HandleSoundEvent(string eventName)
    {
        if (soundEventDictionary.TryGetValue(eventName, out Action action))
        {
            action?.Invoke();
        }
    }
    
    #endregion
}
