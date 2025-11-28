using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AudioEvents
{
    // Enum(BGMKey, SFXKey ���), int(�ش� Ű�� Ŭ�� �ε���, -1�̸� �������), float(fadeIn : BGM, Cutscene ����), float(fadeOut : BGM, Cutscene ����), bool(loop �Ҹ� �ݺ� ����), bool(������ƮǮ�� : SFX, Ambient ����), Vector3?(��ġ : SFX, Ambient ���� ���� ������ �ش� ��ġ���� ���(3d) ������ �׳� 2d���)
    public static event Action<Enum, int, float, float, bool, bool, Vector3?> OnPlayAudio;
    public static event Action<AudioType, string> OnPlayDialogue;
    public static event Action<AudioClip> OnPlayStageBGM;
    
    // 일반적으로 사용하는 오디오 재생 메서드
    public static void Raise(Enum key, int index = -1, float fadeIn = 0, float fadeOut = 0, bool loop = false, bool pooled = false, Vector3? pos = null)
    {
        OnPlayAudio?.Invoke(key, index, fadeIn, fadeOut, loop, pooled, pos);
    }
    // 로비에서 동물 소리 갈래
    public static void Raise(AnimalType type, int index = -1, bool loop = false, bool pooled = false, Vector3? pos = null)
    {
        switch (type)
        {
            case AnimalType.cow:
            OnPlayAudio?.Invoke(SFXKey.OutGameBuffalo, index, 0, 0, loop, pooled, pos);
            break;
            case AnimalType.horse:
            OnPlayAudio?.Invoke(SFXKey.OutGameDonkey, index, 0, 0, loop, pooled, pos);
            break;
            case AnimalType.pig:
            OnPlayAudio?.Invoke(SFXKey.OutGameHog, index, 0, 0, loop, pooled, pos);
            break;
            case AnimalType.bird:
            OnPlayAudio?.Invoke(SFXKey.OutGameRooster, index, 0, 0, loop, pooled, pos);
            break;
            case AnimalType.turtle:
            OnPlayAudio?.Invoke(SFXKey.OutGameTortoise, index, 0, 0, loop, pooled, pos);
            break;
        }
    }

    // Resources로 재생하는 다이얼로그 전용 재생
    public static void RaiseDialogueSound(AudioType type, string audioFileName)
    {
        OnPlayDialogue?.Invoke(type, audioFileName);
    }

    // 각 스테이지 BGM 재생 메서드
    public static void RaiseStageBGM(AudioClip clip)
    {
        OnPlayStageBGM?.Invoke(clip);
    }
}
