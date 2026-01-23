using UnityEngine;

public class LvlSceneChanger : BaseSceneChanger
{
    //0: 다음 레벨
    //1: 레벨 선택 화면

    //레벨 나가기
    public void OnClickedQuit()
    {
        Time.timeScale = 1f;
        
        ChangeScene(nextScenes[1]);
    }
}
