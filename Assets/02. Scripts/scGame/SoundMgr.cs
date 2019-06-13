using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundMgr : MonoBehaviour
{
    public static SoundMgr instance = null;

    public float sfxVolumeSize = 1.0f;
    public bool isSfxMute = false;

    void Awake()
    {
        instance = this;
    }

    public void PlaySfx(Vector3 pos, AudioClip sfx)
    {
        if (isSfxMute)
            return;

        //사운드를 동적으로 생성
        GameObject soundObj = new GameObject("Sfx");

        //사운드 발생위치 지정
        soundObj.transform.position = pos;

        AudioSource audioSource = soundObj.AddComponent<AudioSource>();

        audioSource.clip = sfx;
        audioSource.maxDistance = 10.0f;
        audioSource.minDistance = 30.0f;

        //전체 볼륨 크기
        audioSource.volume = sfxVolumeSize;
        audioSource.Play();

        //사운드 길이만큼 시간이 흐룬 후 삭제
        Destroy(soundObj, sfx.length);
    }
}
