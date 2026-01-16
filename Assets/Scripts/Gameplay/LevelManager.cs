using UnityEngine;
using System.Collections;
using static Constants;
using Unity.Cinemachine;

/// <summary>
/// 레벨의 상태(준비, 진행 중, 완료, 실패) 관리
/// <para>레벨 안에서만 유지되는 싱글톤</para>
/// </summary>
public class LevelManager : Singleton<LevelManager>
{
    protected override bool DontDestroy => false; // 씬 전환 시 파괴됨
    public enum LevelState
    {
        Ready, Play, Pause, Clear, Fail
    }

    public GameObject playerPrefab; // 프로젝트 창의 Player 프리팹 할당
    public Vector3 startPosition;
    public int requiredStars;

    private PlayerController playerController;  // 현재 플레이어 컨트롤러 참조
    private Vector3 spawnPosition;
    private int collectedStars = 0;
    private LevelState currentLevelState = LevelState.Ready;

    /// <summary>
    /// 레벨 초기화
    /// </summary>
    public void Init_Level()
    {
        spawnPosition = startPosition;

        // 플레이어 생성
        RequestRespawn();

        collectedStars = 0;
        currentLevelState = LevelState.Play;

        // UI 초기화
    }

    /// <summary>
    /// 임시 테스트 용, 레벨 씬을 곧바로 실행하지 않는 환경에서는 쓰지 않음
    /// </summary>
    void Awake()
    {
        Init_Level();
    }

    /// <summary>
    /// 플레이어 리스폰 요청
    /// <para>1초 대기 후 스폰 지점에 새 플레이어 생성</para>
    /// </summary>
    public void RequestRespawn()
    {
        StartCoroutine(RespawnRoutine());
    }

    IEnumerator RespawnRoutine()
    {
        // 1초 대기
        yield return new WaitForSeconds(1.0f);

        // 새 플레이어 생성
        GameObject newPlayer = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
        newPlayer.tag = PLAYER_TAG;
        playerController = newPlayer.GetComponent<PlayerController>();
        Debug.Log("Player Respawned");

        //카메라 로직
        var vcam = GameObject.FindAnyObjectByType<CinemachineCamera>();
        if (vcam != null)
        {
            // 플레이어가 리스폰 하면 새로운 플레이어를 따라가도록
            vcam.Target.TrackingTarget = newPlayer.transform; 
        }
    }

    /// <summary>
    /// 레벨 일시정지 요청
    /// </summary>
    public void RequestPause()
    {
        // 일시정지 처리
        currentLevelState = LevelState.Pause;
        // 일시정지 UI 표시
    }

    /// <summary>
    /// 새 스폰 지점 설정
    /// </summary>
    /// <param name="newSpawnPoint">새로운 스폰 지점 위치</param>
    public void SetSpawnPoint(Vector3 newSpawnPoint)
    {
        Debug.Log($"New Spawn Point Set: {newSpawnPoint}");

        spawnPosition = newSpawnPoint;
    }

    /// <summary>
    /// 별 수집 처리, 레벨 완료 체크
    /// </summary>
    public void CollectStar() {
        Debug.Log($"Stars Collected: {collectedStars}/{requiredStars}");

        collectedStars++;
        if (collectedStars >= requiredStars) {
            Debug.Log("Level Complete!");

            // 레벨 완료 처리
            currentLevelState = LevelState.Clear;
            // 레벨 클리어 UI 표시
            // GameManager에 레벨 완료 기록
        }
    }

    // 추가적인 게임 관리 기능들
}
