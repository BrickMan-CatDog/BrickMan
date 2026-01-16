using UnityEngine;

public class DoorBehavior : MonoBehaviour
{
    [SerializeField] private GameObject DoorOpen;
    [SerializeField] private GameObject DoorClosed;
    private bool isOpen = false;
    private Collider2D doorCollider;

    private void Awake()
    {
        doorCollider = GetComponent<Collider2D>();
    }

    public void Open()
    {
        if (!isOpen)
        {
            Debug.Log("문 열림");

            isOpen = true;
            DoorOpen.SetActive(true);
            DoorClosed.SetActive(false);
            doorCollider.enabled = false; // 충돌 비활성화

            // 문 열리는 애니메이션 또는 동작 추가
        }
    }

    public void Close()
    {
        if (isOpen)
        {
            Debug.Log("문 닫힘");

            isOpen = false;
            DoorOpen.SetActive(false);
            DoorClosed.SetActive(true);
            doorCollider.enabled = true; // 충돌 활성화
            
            // 문 닫히는 애니메이션 또는 동작 추가
        }
    }
}
