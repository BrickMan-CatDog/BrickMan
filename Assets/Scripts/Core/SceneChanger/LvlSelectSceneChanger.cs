using UnityEngine;

public class LvlSelectSceneChanger : BaseSceneChanger
{
    //WorldSelection으로 돌아가기(0번)
    public void OnClickedExit()
    {
        ChangeScene(nextScenes[0]);
    }

    //worldNum에 해당하는 World로 씬 바꾸기(1번부터 시작)
    public void OnClickedLvl(int lvlNum)
    {
        ChangeScene(nextScenes[lvlNum]);
    }
}
