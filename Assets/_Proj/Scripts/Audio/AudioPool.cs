using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public enum SFXMode
{
    OutGame = 0,
    InGame
}

public class AudioPool
{
    private readonly Transform parent;
    private readonly AudioMixerGroup defaultGroup;
    private readonly Queue<AudioSource> pool = new Queue<AudioSource>();
    private readonly List<AudioSource> activePool = new List<AudioSource>();
    private readonly List<AudioSource> poolList = new List<AudioSource>();

    public AudioPool(Transform parent, AudioMixerGroup defaultGroup, int size)
    {
        this.parent = parent;
        this.defaultGroup = defaultGroup;
        //var poolParent = new GameObject("PoolParent");
        //poolParent.transform.SetParent(parent);

        for (int i = 0; i < size; i++)
        {
            var src = new GameObject($"PooledAudio_{i}").AddComponent<AudioSource>();
            src.gameObject.tag = "pooled";
            src.outputAudioMixerGroup = defaultGroup;
            src.transform.SetParent(parent);
            src.volume = 1f;
            src.pitch = 1f;
            src.spatialBlend = 1f;

            #region 사용자지정버전
            // src.rolloffMode = AudioRolloffMode.Custom;
            // AnimationCurve curve = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(50f, 0.877f), new Keyframe(60f, 0.59f), new Keyframe(80f, 0.34f), new Keyframe(128f, 0.125f), new Keyframe(200f, 0.002f));
            // //curve = AnimationCurve.EaseInOut(0f, 1f, 200f, 0f);
            // src.SetCustomCurve(AudioSourceCurveType.CustomRolloff, curve);
            // src.minDistance = 40f;
            // src.maxDistance = 200f;
            SetOutGameMode(src);
            #endregion

            #region 일반버전
            // src.rolloffMode = AudioRolloffMode.Logarithmic; // �ڿ������� ����
            // src.minDistance = 40f;  // float �� �̳��� �׻� �ִ� ����
            // src.maxDistance = 170f; // float �� �̻��� �� �鸲
            #endregion
            src.gameObject.SetActive(false);
            poolList.Add(src);
            pool.Enqueue(src);
        }
    }

    public AudioSource GetSource()
    {
        if (pool.Count == 0)
        {
            Debug.LogWarning("���� ����Կ�");
            var newPool = new GameObject("NewPooledAudio").AddComponent<AudioSource>();
            newPool.transform.SetParent(parent);
            newPool.tag = "newPooled";
            return newPool;
        }

        var src = pool.Dequeue();
        activePool.Add(src);
        src.gameObject.SetActive(true);
        return src;
    }

    public void ReturnSource(AudioSource src)
    {
        src.Stop();
        src.gameObject.SetActive(false);
        src.clip = null;
        activePool.Remove(src);
        pool.Enqueue(src);
    }

    private void SetInGameMode(AudioSource src)
    {
        src.rolloffMode = AudioRolloffMode.Logarithmic;
        src.minDistance = 2f;
        src.maxDistance = 100f;
    }

    private void SetOutGameMode(AudioSource src)
    {
        src.rolloffMode = AudioRolloffMode.Custom;
        AnimationCurve curve = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(50f, 0.877f), new Keyframe(60f, 0.59f), new Keyframe(80f, 0.34f), new Keyframe(128f, 0.125f), new Keyframe(200f, 0.002f));
        //curve = AnimationCurve.EaseInOut(0f, 1f, 200f, 0f);
        src.SetCustomCurve(AudioSourceCurveType.CustomRolloff, curve);
        src.minDistance = 40f;
        src.maxDistance = 200f;
    }

    // Ǯ�� ��� ���� �� �ݺ� ����ϴ� ���ϵ� Ǯ���� ������ ex) ȯ���� �߿��� �� �帣�� �Ҹ�
    public void PlayPool()
    {
        foreach (var src in activePool)
        {
            if (src != null && !src.isPlaying) src.Play();
        }
    }
    
    public void PausePool()
    {
        foreach (var src in activePool)
        {
            if (src != null && src.isPlaying) src.Pause();
        }
    }

    public void ResumePool()
    {
        foreach (var src in activePool)
        {
            if (src != null && !src.isPlaying) src.UnPause();
        }
    }

    public void StopPool()
    {
        foreach (var src in activePool)
        {
            if (src != null && src.isPlaying) 
            {
                src.DOFade(0, 1f);
                src.Stop();
            }
        }
    }

    public void ResetPool(float volumeValue)
    {
        foreach (var src in activePool)
        {
            if (src != null)
            {
                if (src.isPlaying) src.Stop();
                src.loop = false;
                src.volume = volumeValue;
                src.pitch = 1f;
                src.clip = null;
            }
        }
        foreach (var gObj in pool)
        {
            if (gObj.CompareTag("newPooled"))
            {
                GameObject gO;
                gO = gObj.gameObject;
                UnityEngine.GameObject.Destroy(gO);
            }
        }
    }

    public void SettingPool(float volumeValue, SFXMode sfxMode)
    {
        foreach (var src in poolList)
        {
            if (src != null)
            {
                src.volume = volumeValue;

                switch (sfxMode)
                {
                    case SFXMode.InGame:
                    SetInGameMode(src);
                    break;
                    case SFXMode.OutGame:
                    SetOutGameMode(src);
                    break;
                }
            }
        }
    }
    public void SetPoolVolumeHalf()
    {
        foreach (var src in activePool)
        {
            if (src != null) 
            {
                if (DOTween.IsTweening(src, true))
                {
                    src.DOKill();
                }
                src.DOFade(0.3f, 0.5f);
            }
        }
    }
    public void SetPoolVolumeNormal()
    {
        foreach (var src in activePool)
        {
            if (src != null) 
            {
                if (DOTween.IsTweening(src, true))
                {
                    src.DOKill();
                }
                src.DOFade(1, 0.5f);
            }
        }
    }
    public void SetPoolVolumeZero()
    {
        foreach (var src in activePool)
        {
            if (src != null) 
            {
                if (DOTween.IsTweening(src, true))
                {
                    src.DOKill();
                }
                src.DOFade(0, 0.5f);
            }
        }
    }

    /// <summary>
    /// 특정 클립을 재생 중인 Pool 안에 상태를 제어하는 메서드
    /// int mode : 0 = 재생, 1 = 멈춤, 2 = 재개, 3 = 멈추고 재생 위치를 처음으로, 4 = 멈추고 클립 빼기, 5 = 볼륨 조절
    /// mode 4, 5는 float volumeValue 파라미터를 넣을 수 있음
    /// </summary>
    /// <param name="clip"></param>
    /// <param name="mode"></param>
    /// <param name="volumeValue"></param>
    public void CustomPoolSourceInPool(AudioClip clip, int mode, float? volumeValue = null)
    {
        foreach (var src in activePool)
        {
            if (src.clip == clip)
            {
                switch (mode)
                {
                    case 0: src.Play(); break;
                    case 1: src.Pause(); break;
                    case 2: src.UnPause();  break;
                    case 3: src.Stop(); break;
                    case 4: 
                    src.DOFade(0, 0.3f).OnComplete(() => 
                    {
                        src.Stop();
                        src.clip = null; 
                        if (volumeValue != null)
                        {
                            src.volume = (float)volumeValue;
                        }
                        else
                        {
                            src.volume = 1f;
                        }
                    });
                    
                    break;
                    case 5:
                    if (volumeValue != null)
                    {
                        src.volume = (float)volumeValue;
                    }
                    break;
                }
            }
        }
    }

    public IEnumerator ReturnAfterDelay(AudioSource src, float delay)
    {
        yield return new WaitForSeconds(delay);
        ReturnSource(src);
    }
    
}
