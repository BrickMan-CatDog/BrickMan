using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 버튼과 연결된 객체의 추상 클래스
/// </summary>
public abstract class ButtonLinked : MonoBehaviour
{
    [SerializeField] protected List<NewButtonBehavior> linkedButtons;

    /// <summary>
    /// 모든 연결된 버튼이 켜져 있어야 활성화되는지 여부
    /// <para> true면 AND 게이트처럼 모두 켜져 있어야 활성화</para>
    /// <para> false면 모든 버튼이 서로 링크된 듯 한번에 작동함, 기본값</para>
    /// </summary>
    [SerializeField] protected bool mustAllOn = false;
    public bool isActive = false;

    /// <summary>
    /// 버튼과 연결된 버튼들을 초기화하는 메서드
    /// </summary>
    public void Init_LinkedButtons()
    {
        foreach (var button in linkedButtons)
        {
            button.Link(this);
        }

        if (isActive)
        {
            Activate();
        }
        else
        {
            Deactivate();
        }
    }

    /// <summary>
    /// 버튼 눌림 신호를 받을 때 호출되는 메서드
    /// <para> 다른 버튼 변화 처리 후 활성화/비활성화 메서드 호출 </para>
    /// </summary>
    /// <param name="state">버튼의 현재 상태</param>
    public void ButtonPressed(bool state)
    {
        if (state)
        {
            if (mustAllOn)
            {
                foreach (var button in linkedButtons)
                {
                    if (!button.buttonState)
                    {
                        return;
                    }
                }
                Activate();
                isActive = true;
            }
            else
            {
                foreach (var button in linkedButtons)
                {
                    button.SetButton(true, false);
                }
                Activate();
                isActive = true;
            }
        }

        else
        {
            if (mustAllOn)
            {
                Deactivate();
                isActive = false;
            }
            else
            {
                foreach (var button in linkedButtons)
                {
                    button.SetButton(false, false);
                }
                Deactivate();
                isActive = false;
            }
        }
    }

    /// <summary>
    /// 활성화될 때 호출되는 메서드
    /// </summary>
    public abstract void Activate();

    /// <summary>
    /// 비활성화될 때 호출되는 메서드
    /// </summary>
    public abstract void Deactivate();
}