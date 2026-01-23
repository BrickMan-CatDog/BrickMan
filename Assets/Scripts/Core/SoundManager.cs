using UnityEngine;
using UnityEngine.Audio;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

/// <summary>
/// 사운드 효과 및 배경 음악 관리
/// <para>게임 전체 전역 PersistentSingleton</para>
/// </summary>
public class SoundManager : PersistentSingleton<SoundManager>
{
    [Header("Audio Mixer Groups")]
    [SerializeField] private AudioMixer audioMixer;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip[] bgmClips;
    [SerializeField] private AudioClip[] sfxClips;

    [Header("Pool Settings")]
    [SerializeField] private int initialPoolSize = 10;
    [SerializeField] private bool canGrowPool = true;

    [Header("Volume (0~1, Linear)")]
    [Range(0f, 1f)][SerializeField] private float masterVolume;
    [Range(0f, 1f)][SerializeField] private float bgmVolume;
    [Range(0f, 1f)][SerializeField] private float sfxVolume;

    [SerializeField] private string masterParam;
    [SerializeField] private string bgmParam;
    [SerializeField] private string sfxParam;

    [Header("3D SFX Settings")]
    [SerializeField] private bool use3DForSFX = true;           // Positioned/Tracking일 때만 3D로
    [Range(0f, 5f)][SerializeField] private float sfxDoppler = 0f;
    [Range(0f, 360f)][SerializeField] private float sfxSpread = 0f;
    [SerializeField] private float sfxMinDistance = 2f;         // 이 거리 이내는 거의 원볼륨
    [SerializeField] private float sfxMaxDistance = 25f;        // 이 거리 넘어가면 거의 무음
    [SerializeField] private AudioRolloffMode sfxRolloff = AudioRolloffMode.Logarithmic;
    // (선택) 커스텀 감쇠 곡선이 필요하면 AnimationCurve를 추가하고 rolloff를 Custom으로 바꿔서 SetCustomCurve 사용
    [SerializeField] private AnimationCurve customRolloff = AnimationCurve.Linear(0, 1, 1, 0);

    public bool canPlay = true;


    private Dictionary<string, AudioClip> bgmDict = new(); // BGM 클립 딕셔너리
    private Dictionary<string, AudioClip> sfxDict = new(); // SFX 클립 딕셔너리

    private Stack<SoundPlayer> pool = new(); // 사운드 플레이어 풀
    private readonly List<SoundPlayer> activeSfx = new(); // 현재 재생 중인 SFX(루프/원샷 포함)
    private List<SoundPlayer> loopedPlayers = new(); // 현재 루프 중인 사운드 플레이어 목록

    protected override void Awake()
    {
        base.Awake();

        // audioDict 초기화
        foreach (var clip in bgmClips)
        {
            if (clip != null && !bgmDict.ContainsKey(clip.name))
            {
                bgmDict[clip.name] = clip;
            }
        }
        foreach (var clip in sfxClips)
        {
            if (clip != null && !sfxDict.ContainsKey(clip.name))
            {
                sfxDict[clip.name] = clip;
            }
        }
        // 사운드 플레이어 풀 초기화
        for (int i = 0; i < initialPoolSize; i++)
        {
            var player = CreatePlayer();
            pool.Push(player);
        }
    }

    private void Start()
    {
        ApplyVolumes();

    }

    // 에디터 inspector에서 값 변경시 볼륨 적용 -> X.얘가 아래 에러를 발생시킴
    //- 사운드 줄인 상태에서 다른 씬으로 이동 시, 설정 창은 그대로지만 사운드가 다시 기존 크기로 돌아감
    //private void OnValidate()
    //{
    //    if (audioMixer != null) ApplyVolumes();
    //}

    public void SetMasterVolume(float v) { masterVolume = Mathf.Clamp01(v); ApplyVolumes(); }
    public void SetBGMVolume(float v) { bgmVolume = Mathf.Clamp01(v); ApplyVolumes(); }
    public void SetSFXVolume(float v) { sfxVolume = Mathf.Clamp01(v); ApplyVolumes(); }

    private void ApplyVolumes()
    {
        SetMixer(masterParam, masterVolume);
        SetMixer(bgmParam, bgmVolume);
        SetMixer(sfxParam, sfxVolume);
    }

    // 0~1 선형값을 dB로 변환해 Mixer에 세팅
    private void SetMixer(string param, float linear)
    {
        float dB = (linear > 0f) ? Mathf.Log10(linear) * 20f : -80f;
        audioMixer.SetFloat(param, dB);
    }

    private SoundPlayer CreatePlayer() // 사운드 플레이어 생성
    {
        var player = new GameObject("SoundPlayer");
        player.transform.SetParent(transform);
        var source = player.AddComponent<AudioSource>();
        return new SoundPlayer(source, this);
    }

    private AudioClip GetClip(string name, bool isBGM) // 클립 이름으로 오디오 클립 가져오기
    {
        if (isBGM)
        {
            if (bgmDict.TryGetValue(name, out var clip))
            {
                return clip;
            }
        }
        else
        {
            if (sfxDict.TryGetValue(name, out var clip))
            {
                return clip;
            }
        }
        Debug.LogWarning($"SoundManager: Clip '{name}' not found.");
        return null;
    }

    private AudioSource bgmSource;

    public void PlayBGM(string name, float volume = 1f) // BGM 재생
    {
        var clip = GetClip(name, true);
        if (clip == null) return;

        if (bgmSource == null)
        {
            var obj = new GameObject("BGMSource");
            obj.transform.SetParent(transform);
            bgmSource = obj.AddComponent<AudioSource>();
            bgmSource.loop = true;
            bgmSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("BGM")[0];
        }

        if (bgmSource.clip == clip && bgmSource.isPlaying) return;

        bgmSource.clip = clip;
        bgmSource.volume = volume;
        bgmSource.Play();
    }

    public void StopBGM() // BGM 정지
    {
        if (bgmSource != null && bgmSource.isPlaying)
        {
            bgmSource.Stop();
        }
    }

    public void PauseBGM() // BGM 일시정지
    {
        if (bgmSource != null && bgmSource.isPlaying)
        {
            bgmSource.Pause();
        }
    }

    public void ResumeBGM() // BGM 재개
    {
        if (bgmSource != null && !bgmSource.isPlaying)
        {
            bgmSource.UnPause();
        }
    }

    public void PlaySFX(string name, float volume = 1f, float pitch = 1f, Action onFinished = null) // SFX 재생
       => PlayInternal(name, volume, pitch, PlayMode.Simple, null, Vector3.zero, onFinished);

    // 위치 기반
    public void PlaySFXAt(string name, Vector3 pos, float volume = 1f, float pitch = 1f, Action onFinished = null) // 위치 기반 SFX 재생
        => PlayInternal(name, volume, pitch, PlayMode.Positioned, null, pos, onFinished);

    // 추적 재생
    public void PlaySFXTracking(string name, Transform target, float volume = 1f, float pitch = 1f, Action onFinished = null) // 추적 재생 SFX
        => PlayInternal(name, volume, pitch, PlayMode.Tracking, target, Vector3.zero, onFinished);

    public void StopAllSFX()
    {
        // 루프 중인 SFX 정지
        StopAllLoops();

        // 풀에 들어있지 않고 재생 중인 SFX 정지
        foreach (Transform child in transform)
        {
            var source = child.GetComponent<AudioSource>();
            if (source != null && source.isPlaying)
            {
                source.Stop();
            }
        }

        // 추적 리스트 정리
        ReturnAllActiveSfx();
    }

    public void StopSFX(string clipName)
    {
        // 루프 중인 것부터 정지
        for (int i = loopedPlayers.Count - 1; i >= 0; i--)
        {
            var player = loopedPlayers[i];
            if (player.ClipName == clipName)
            {
                StopAndRecycle(player, removeFromLoopList: true);
            }
        }

        // 원샷/기타 SFX 정지
        for (int i = activeSfx.Count - 1; i >= 0; i--)
        {
            var player = activeSfx[i];
            if (player.ClipName == clipName)
            {
                StopAndRecycle(player);
            }
        }
    }


    public void PlayLoop(string name, Transform target = null, float volume = 1f, float pitch = 1f) // 루프 재생
    {
        var clip = GetClip(name, false);
        if (clip == null) return;

        var player = pool.Count > 0 ? pool.Pop() : canGrowPool ? CreatePlayer() : null;
        if (player == null) return;

        player.Play(clip, volume, pitch, target == null ? PlayMode.Simple : PlayMode.Tracking, target, Vector3.zero, loop: true);
        activeSfx.Add(player);
        loopedPlayers.Add(player);
    }

    public void StopLoop(string clipName) // 루프 중인 사운드 정지
    {
        for (int i = loopedPlayers.Count - 1; i >= 0; i--)
        {
            var player = loopedPlayers[i];
            if (player.ClipName == clipName)
            {
                StopAndRecycle(player, removeFromLoopList: true);
                return;
            }
        }
        Debug.LogWarning($"SoundManager: Loop sound '{clipName}' not found.");
    }

    public void StopAllLoops() // 모든 루프 중인 사운드 정지
    {
        foreach (var player in loopedPlayers)
        {
            StopAndRecycle(player, removeFromLoopList: true);
        }
        loopedPlayers.Clear();
    }

    private void PlayInternal(string name, float volume, float pitch, PlayMode mode, Transform target, Vector3 position, Action onFinished) //  사운드 재생 내부 메서드
    {
        if (!canPlay) return;
        var clip = GetClip(name, false);
        if (clip == null) return;

        var player = pool.Count > 0 ? pool.Pop() : canGrowPool ? CreatePlayer() : null;
        if (player == null) return;

        activeSfx.Add(player);
        player.Play(clip, volume, pitch, mode, target, position, loop: false, () =>
        {
            onFinished?.Invoke();
            RecyclePlayer(player);
        });
    }

    private void StopAndRecycle(SoundPlayer player, bool removeFromLoopList = false)
    {
        player.StopImmediate();
        activeSfx.Remove(player);
        if (removeFromLoopList)
        {
            loopedPlayers.Remove(player);
        }
        pool.Push(player);
    }

    private void RecyclePlayer(SoundPlayer player)
    {
        activeSfx.Remove(player);
        loopedPlayers.Remove(player);
        pool.Push(player);
    }

    private void ReturnAllActiveSfx()
    {
        for (int i = activeSfx.Count - 1; i >= 0; i--)
        {
            var player = activeSfx[i];
            player.StopImmediate();
            pool.Push(player);
        }
        activeSfx.Clear();
        loopedPlayers.Clear();
    }

    private enum PlayMode // 사운드 재생 모드
    {
        Simple,
        Positioned,
        Tracking
    }

    private class SoundPlayer //사운드 플레이어 클래스
    {
        public string ClipName { get; private set; }
        private readonly AudioSource audioSource;
        private readonly Transform transform;
        private MonoBehaviour host;
        private Coroutine currentCoroutine;
        public SoundPlayer(AudioSource src, MonoBehaviour host)
        {
            this.audioSource = src;
            this.transform = src.transform;
            this.host = host;
        }



        // SoundManager.SoundPlayer.Play(...) 내부 교체/추가
        public void Play(AudioClip clip, float volume, float pitch, PlayMode mode, Transform target, Vector3 position, bool loop, Action onFinished = null)
        {
            if (clip == null) return;

            ClipName = clip.name;
            audioSource.clip = clip;
            audioSource.volume = volume;
            audioSource.pitch = pitch;
            audioSource.loop = loop;
            audioSource.outputAudioMixerGroup = SoundManager.Instance.audioMixer.FindMatchingGroups("SFX")[0];

            bool use3D = (mode != PlayMode.Simple) && SoundManager.Instance.use3DForSFX;
            audioSource.spatialBlend = use3D ? 1f : 0f;           // 1 = 완전 3D, 0 = 완전 2D
            audioSource.dopplerLevel = SoundManager.Instance.sfxDoppler;
            audioSource.spread = SoundManager.Instance.sfxSpread;
            audioSource.rolloffMode = SoundManager.Instance.sfxRolloff;
            audioSource.minDistance = SoundManager.Instance.sfxMinDistance;
            audioSource.maxDistance = SoundManager.Instance.sfxMaxDistance;

            if (SoundManager.Instance.sfxRolloff == AudioRolloffMode.Custom)
            {
                audioSource.SetCustomCurve(AudioSourceCurveType.CustomRolloff, SoundManager.Instance.customRolloff);
            }

            switch (mode)
            {
                case PlayMode.Simple:
                    // 위치 무관
                    if (currentCoroutine != null) host.StopCoroutine(currentCoroutine);
                    currentCoroutine = host.StartCoroutine(DelayCoroutine(clip.length / pitch, onFinished));
                    break;

                case PlayMode.Positioned:
                    // 고정 위치 3D 재생
                    transform.position = position;
                    if (currentCoroutine != null) host.StopCoroutine(currentCoroutine);
                    currentCoroutine = host.StartCoroutine(DelayCoroutine(clip.length / pitch, onFinished));
                    break;

                case PlayMode.Tracking:
                    // 타겟을 따라다니며 3D 재생
                    if (currentCoroutine != null) host.StopCoroutine(currentCoroutine);
                    currentCoroutine = host.StartCoroutine(TrackCoroutine(clip.length / pitch, target, onFinished));
                    break;
            }

            audioSource.Play();
        }

        public void StopImmediate() // 즉시 정지 메서드
        {
            if (currentCoroutine != null)
                host.StopCoroutine(currentCoroutine);
            audioSource.Stop();
        }

        private IEnumerator PlayCoroutine(float duration, Action callback) //사운드 재생 코루틴
        {
            currentCoroutine = host.StartCoroutine(DelayCoroutine(duration, callback));
            yield return currentCoroutine;
        }

        private IEnumerator TrackCoroutine(float duration, Transform target, Action callback) // 추적 재생 코루틴
        {
            float time = 0f;
            currentCoroutine = host.StartCoroutine(DelayCoroutine(duration, callback));
            while (time < duration)
            {
                if (target != null)
                {
                    transform.position = target.position;
                }
                time += Time.deltaTime;
                yield return null;
            }
        }

        private IEnumerator DelayCoroutine(float duration, Action callback) // 딜레이 후 콜백 실행 코루틴
        {
            yield return new WaitForSeconds(duration);
            callback?.Invoke();
        }
    }
}