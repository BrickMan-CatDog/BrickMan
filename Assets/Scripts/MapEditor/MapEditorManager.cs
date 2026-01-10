using UnityEngine;

/// <summary>
/// 맵 에디터의 상태 및 기능 관리
/// <para>맵 에디터 씬에서만 유지되는 싱글톤</para>
/// </summary>
public class MapEditorManager : Singleton<MapEditorManager>
{
    protected override bool DontDestroy => false; // 씬 전환 시 파괴됨
}
