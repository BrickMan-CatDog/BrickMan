using UnityEngine;
using System.Collections;

/// <summary>
/// 게임의 전체 상태(로비, 인게임, 일시정지) 관리
/// <para>게임 전체 전역 싱글톤</para>
/// </summary>
public class GameManager : Singleton<GameManager>
{
    PlayerController playercontroller;
    
    public enum GameState
    {
        Main, WorldSelection, LevelSelection, Level, MapEditor
    }

    public GameState CurrentState { get; private set; } = GameState.Main;

    public GameObject playerPrefab; // 프로젝트 창의 Player 프리팹 할당
    public Vector3 spawnPosition;

    public void ChangeState(GameState newState)
    {
        CurrentState = newState;

        // 상태 변경에 따른 로직

        Debug.Log($"Game State changed to: {CurrentState}");
    }

    //PlayerController에서 사용
    //플레이어가 사망했을때 호출됨
    public void RequestRespawn() {

        StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine() {
        // 1초 대기
        yield return new WaitForSeconds(1.0f);

        // 새 플레이어 생성
        GameObject newPlayer = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
        newPlayer.tag = "Player"; 
        
        Debug.Log("Player Respawned");
    }

    // 추가적인 게임 관리 기능들
}
