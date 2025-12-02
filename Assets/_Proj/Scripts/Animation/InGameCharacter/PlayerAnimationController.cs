using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField] AudioListener audioListener;
    private Camera cam;
    public Animator anim;
    public PlayerMovement move;
    public PlayerPush push;
    private WaitForSeconds startDelay;
    private WaitForSeconds nextDelay;
    //private Coroutine currentCoroutine;
    //private AudioSource src;

    private void Awake()
    {
        startDelay = new WaitForSeconds(UnityEngine.Random.Range(2f, 3f));
        nextDelay = new WaitForSeconds(10f);
        cam = Camera.main;
        if (audioListener == null) audioListener = GetComponent<AudioListener>();
        if (!audioListener.enabled) audioListener.enabled = true;
        var camAudioListener = cam.GetComponent<AudioListener>();
        camAudioListener.enabled = false;
    }

    private void OnEnable()
    {
        ETCEvent.OnCocoInteractSoundInGame += PlayCocoInteractSound;
        //if (!audioListener.enabled) audioListener.enabled = true;
        //var camAudioListener = cam.GetComponent<AudioListener>();
        //if (camAudioListener.enabled) camAudioListener.enabled = false;
    }

    private void Update()
    {
        bool isRunning = move.isRunning;
        // 코코두기 인게임에서 조이스틱 입력값 없을 시 대기 소리 재생. // 일단 보류
        // if (isRunning && currentCoroutine != null)
        // {
        //     AudioClip clip = AudioManager.Instance.LibraryProvider.GetClip(AudioType.SFX, SFXKey.InGameCocodoogy, 0);
        //     var group = AudioManager.Instance.AudioGroups.OfType<SFXGroup>().FirstOrDefault();
        //     group.CustomPlayerControl(clip, 4);
        //     StopCoroutine(currentCoroutine);
        //     currentCoroutine = null;
        // }
        // else if (!isRunning && currentCoroutine == null)
        // {
        //     currentCoroutine = StartCoroutine(PlayCocoBreathingSound());
        // }

        bool isPushing = push.isPushing;
        //float speed = move.rb.linearVelocity.magnitude;

        anim.SetBool("Push", isPushing);
        anim.SetBool("Run", isRunning && !isPushing);
        //anim.SetFloat("Speed", speed);
    }

    private void OnDisable()
    {
        ETCEvent.OnCocoInteractSoundInGame -= PlayCocoInteractSound;
        //if (audioListener.enabled) audioListener.enabled = false;
        //var camAudioListener = cam.GetComponent<AudioListener>();
        //if (!camAudioListener.enabled) camAudioListener.enabled = true;
    }

    private IEnumerator PlayCocoBreathingSound()
    {
        while (true)
        {
            yield return startDelay;
            AudioEvents.Raise(SFXKey.InGameCocodoogy, 0, loop: false, pooled: true, pos: transform.position);
            AudioClip clip = AudioManager.Instance.LibraryProvider.GetClip(AudioType.SFX, SFXKey.InGameCocodoogy, 0);
            yield return clip.length;
            yield return nextDelay;
        }
    }
    private void PlayCocoInteractSound()
    {
        int index = UnityEngine.Random.Range(1, 5);
        AudioEvents.Raise(SFXKey.InGameCocodoogy, index);
    }
    
}

