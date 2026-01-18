using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;
using Unity.Burst.Intrinsics;

/// <summary>
/// 레벨이 아닌 게임의 전체 상태(로비, 인게임, 일시정지) 관리
/// <para>게임 전체 전역 싱글톤</para>
/// </summary>
public class GameManager : Singleton<GameManager>
{
    public enum GameState
    {
        Main, WorldSelection, LevelSelection, Level
    }

    public GameState CurrentState { get; private set; } = GameState.Main;
    // public List<Tuple<int, bool>> LevelClearStatus = new List<Tuple<int, bool>>();

    private void ChangeState(GameState newState)
    {
        CurrentState = newState;

        // 상태 변경에 따른 로직

        Debug.Log($"Game State changed to: {CurrentState}");
    }

    // 레벨 클리어 상태 설정
    public void SetLevelClearStatus(int levelIndex, bool isCleared)
    {
        // LevelClearStatus[levelIndex] = Tuple.Create(levelIndex, isCleared);
    }

    public void GotoMainMenu()
    {
        ChangeState(GameState.Main);
        // 메인 메뉴로 전환하는 로직 추가
        SceneManager.LoadScene("MainMenu");
    }

    public void GotoWorldSelection()
    {
        ChangeState(GameState.WorldSelection);
        // 월드 선택 화면으로 전환하는 로직 추가
        SceneManager.LoadScene("WorldSelection");
    }

    public void GotoLevelSelection()
    {
        ChangeState(GameState.LevelSelection);
        // 레벨 선택 화면으로 전환하는 로직 추가
        SceneManager.LoadScene("LevelSelection");
    }

    public void GotoLevel()
    {
        ChangeState(GameState.Level);
        // 레벨로 전환하는 로직 추가
        // SceneManager.LoadScene("Level1");
    }

    public void ExitLevel()
    {
        ChangeState(GameState.LevelSelection);
        // 레벨 마친 후 레벨 선택 화면으로 전환하는 로직 추가

        SceneManager.LoadScene("LevelSelection");
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
