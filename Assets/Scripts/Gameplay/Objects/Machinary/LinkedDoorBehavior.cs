using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Constants;

public class LinkedDoorBehavior : ButtonLinked
{
    private List<Transform> doors = new List<Transform>();
    private Rigidbody2D doorRB;

    private void Awake()
    {
        doorRB = GetComponent<Rigidbody2D>();
        Init_LinkedButtons();
        foreach (Transform child in transform)
        {
            doors.Add(child);
        }
    }

    public override void Activate()
    {
        Debug.Log("문 열림");
        foreach (var door in doors)
        {
            door.GetChild(0).gameObject.SetActive(true); // DoorOpen
            door.GetChild(1).gameObject.SetActive(false); // DoorClosed
        }
        doorRB.simulated = false; // 충돌 비활성화
        gameObject.layer = LayerMask.NameToLayer("Default"); // 충돌 레이어 변경
        // 문 열리는 애니메이션 또는 동작 추가
    }

    public override void Deactivate()
    {
        Debug.Log("문 닫힘");
        foreach (var door in doors)
        {
            door.GetChild(0).gameObject.SetActive(false); // DoorOpen
            door.GetChild(1).gameObject.SetActive(true); // DoorClosed
        }
        doorRB.simulated = true; // 충돌 활성화
        gameObject.layer = LayerMask.NameToLayer(GROUND_LAYER); // 충돌 레이어 원래대로 변경
        // 문 닫히는 애니메이션 또는 동작 추가
    }
}
