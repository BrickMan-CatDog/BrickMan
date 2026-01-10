using UnityEngine;
using UnityEngine.InputSystem; // [핵심 1] 네임스페이스 추가

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class TestFollowMouse : MonoBehaviour
{
    [Header("Settings")]
    public float moveSpeed = 10f; // 이동 속도

    private Rigidbody2D rb;
    private Camera mainCamera;
    private Vector2 targetWorldPosition;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;

        // [중요] 중력 끄기 (마우스 따라다니는데 떨어지면 안 되므로)
        rb.gravityScale = 0f;
        
        // [중요] 물리 충돌 시 덜덜거림 방지 (부드러운 이동)
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        
        // [중요] 빠른 이동 시 벽 뚫기 방지
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        
        // [중요] 충돌 시 회전 방지 (오뚜기 설정)
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    void Update()
    {
        // 1. 마우스 입력 감지 (Input System 방식)
        if (Mouse.current == null) return; // 마우스가 연결 안 된 경우 예외 처리

        // [핵심 2] 스크린 좌표 가져오기 (Vector2 반환)
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();

        // 2. 스크린 좌표 -> 월드 좌표 변환
        // 카메라의 깊이(z)를 상쇄하기 위해 Z값을 설정
        Vector3 screenPosWithDepth = new Vector3(mouseScreenPos.x, mouseScreenPos.y, -mainCamera.transform.position.z);
        targetWorldPosition = mainCamera.ScreenToWorldPoint(screenPosWithDepth);
    }

    void FixedUpdate()
    {
        // 3. 물리 이동 처리 (벽 충돌 계산)
        // 현재 위치에서 목표 위치로 moveSpeed만큼 이동
        Vector2 newPosition = Vector2.MoveTowards(rb.position, targetWorldPosition, moveSpeed * Time.fixedDeltaTime);
        
        // [핵심 3] transform.position 대신 Rigidbody를 움직여야 벽을 안 뚫음
        rb.MovePosition(newPosition);
    }
}