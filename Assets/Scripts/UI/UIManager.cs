using UnityEngine;

/// <summary>
/// Views의 UI 출력 / 숨김 등 관리 담당
/// <para>게임 전체 전역 싱글톤</para>
/// </summary>
public class UIManager : Singleton<UIManager>
{


/*
    public Transform canvasTrm; //캔버스의 위치

    void Awake()
    {
        canvasTrm = GameObject.Find("Canvas").transform;
    }
*/

//간단하게 만들긴 했는데 수정 필요하면 해야할듯
    public void ShowUI(GameObject ui)
    {
        ui.SetActive(true);
    }

    public void HideUI(GameObject ui)
    {
        ui.SetActive(false);
    }
}
