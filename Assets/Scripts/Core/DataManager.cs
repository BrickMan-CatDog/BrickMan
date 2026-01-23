using UnityEngine;

/// <summary>
/// 맵 데이터, 유저 설정, 유저 진행 상황 관리
/// <para>게임 전체 전역 PersistentSingleton</para>
/// </summary>
public class DataManager : PersistentSingleton<DataManager>
{
    protected override void Awake()
    {
        base.Awake();

        // 땜빵용 코드
        SoundManager.Instance.SetMasterVolume(1f);
        SoundManager.Instance.SetBGMVolume(1f);
        SoundManager.Instance.SetSFXVolume(1f);
    }
}
