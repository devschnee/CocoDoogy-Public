using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BaseInGameTurtleAnimation : IAnimalAnimation
{
    private Animator anim;
    private MonoBehaviour mono;
    private Turtle turtle;
    private bool isMoving;
    public Dictionary<string,Action> animEventDictionary;
    public Dictionary<string,Action> soundEventDictionary;

    #region 애니메이션

    #endregion

    #region 소리

    private void SoundSwim()
    {
        AudioEvents.Raise(SFXKey.InGameTortoise, 0, loop: false, pooled: true, pos: mono.transform.position);
    }

    #endregion

    #region IAnimalAnimation 인터페이스 영역

    public void Init(Animator anim, MonoBehaviour mono)
    {
        this.anim = anim;
        this.mono = mono;
        turtle = mono.GetComponentInParent<Turtle>();

        animEventDictionary = new Dictionary<string, Action>
        {
            
        };
        soundEventDictionary = new Dictionary<string, Action>
        {
            { "Sound_Swim", SoundSwim }
        };

    }

    public void OnEnabled() { }

    public void Update()
    {
        isMoving = turtle.IsMoving;
        anim.SetBool("Swim", isMoving);
        if (!isMoving)
        {
            AudioClip clip = AudioManager.Instance.LibraryProvider.GetClip(AudioType.SFX, SFXKey.InGameTortoise, 0);
            var group = AudioManager.Instance.AudioGroups.OfType<SFXGroup>().FirstOrDefault();
            group.CustomPlayerControl(clip, 4);
        }
    }

    public void OnDisable() { }

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
