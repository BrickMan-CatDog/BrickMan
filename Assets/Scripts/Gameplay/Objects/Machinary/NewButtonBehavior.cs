using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 새로운 버튼 동작 클래스
/// </summary>
public class NewButtonBehavior : MonoBehaviour
{
    [SerializeField] private GameObject ButtonOn;
    [SerializeField] private GameObject ButtonOff;
    public bool buttonState = false;
    
    private List<ButtonLinked> linkedObjects = new List<ButtonLinked>();
    private string[] validTags = { "Player", "CloneBox" };


    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("버튼 누름");

        foreach (var tag in validTags)
        {
            if (collision.CompareTag(tag))
            {
                SetButton(!buttonState);
            }
        }
    }

    /// <summary>
    /// 버튼과 연결된 객체 등록
    /// </summary>
    /// <param name="buttonLinked">연결할 객체</param>
    public void Link(ButtonLinked buttonLinked)
    {
        if (!linkedObjects.Contains(buttonLinked))
        {
            linkedObjects.Add(buttonLinked);
        }
    }

    /// <summary>
    /// 버튼 상태 설정
    /// </summary>
    /// <param name="targetState">설정할 상태</param>
    /// <param name="trigger">상태 변경 시 연결된 객체에 알릴지 여부</param>
    public void SetButton(bool targetState, bool trigger = true)
    {
        if (targetState != buttonState)
        {
            buttonState = targetState;
            ButtonOn.SetActive(targetState);
            ButtonOff.SetActive(!targetState);

            if (trigger)
            {
                foreach (var obj in linkedObjects)
                {
                    obj.ButtonPressed(targetState);
                }
            }
        }
    }
}
