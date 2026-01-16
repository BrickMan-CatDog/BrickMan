using UnityEngine;

/// <summary>
/// View의 추상 기본 클래스
/// <para>UI의 한 화면 단위</para>
/// </summary>
public abstract class View : MonoBehaviour
{
    /// <summary>
    /// View 표시
    /// </summary>
    public virtual void Show()
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// View 숨김
    /// </summary>
    public virtual void Hide()
    {
        gameObject.SetActive(false);
    }
}