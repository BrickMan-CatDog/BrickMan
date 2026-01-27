using UnityEngine;
using static Constants;

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
            gameObject.layer = LayerMask.NameToLayer(GROUND_LAYER); // 충돌 레이어 변경
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
            gameObject.layer = LayerMask.NameToLayer("Default"); // 충돌 레이어 원래대로 변경
            // 문 닫히는 애니메이션 또는 동작 추가
        }
    }
}
