using System;
using UnityEngine;

/// <summary>
/// 플레이어의 이동 및 상호작용 관리
/// </summary>
public class PlayerController : MonoBehaviour
{
    InputHandler inputhandler;
    Rigidbody2D rb;
    [SerializeField] private float leviatingLockTime = 0.15f; // Leviating>Move로 바로 전환되지 않도록 대비

    public Vector3 movement; //좌우 움직임 결정
    public Vector3 rotatemovement; //회전 움직임 결정

    public LayerMask groundlayer;  //레이어를 구분하기 위한 변수,inspector창에서 Ground레이어,box레이어 삽입
    // public LayerMask walllayer; //밀리지 않는 레이어 삽입 Ground레이어 삽입
    public GameObject boxprefab; //inspector창에서 clonebox 프리펩 삽입
    public GameObject player; //inspector창에서 player 프리펩 삽입(지금은 테스트용 플레이어)근데 이거 필요한..가?


    public float moveForce = 30f;
    public float maxSpeed = 3f; //최대 속도
    public float power = 5f;
    public float rotateSpeed = 100f;


    private float airtime = 0f; //체공시간 측정용 변수
    private Vector3 MyPos; //플레이어의 현재 위치
    public bool isGround = true; //땅에 닿고 있는 판정인지의 여부
    private float leviatingEnterTime;


    //플레이어 상태
    public enum PlayerState
    {
        Idle,
        Move,
        Leviating,
        Flip,
        Death
    }

    public PlayerState currentstate { get; private set; } = PlayerState.Idle;

    // 상태변화 이벤트 (애니메이션 호출에 사용)
    public System.Action<PlayerState> OnStateChanged;

    //상태변화 함수
    public void ChangeState(PlayerState newState)
    {
        currentstate = newState;
        Debug.Log($"Game State changed to : {currentstate}");

        if (newState == PlayerState.Leviating)
        {
            leviatingEnterTime = Time.time;
        }
        OnStateChanged?.Invoke(currentstate);
    }



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    void Awake()
    {
        inputhandler = GameObject.Find("InputHandler").GetComponent<InputHandler>();
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;  //idle 상태일 때 player 회전 차단
    }

    // Update is called once per frame
    //InputHandler 연결, update마다 move()실행
    void FixedUpdate()
    {
        if (currentstate != PlayerState.Flip)
        { //Flip 상태에서 회전했을 때 땅에 닿으면 idle로 바뀌는 문제 수정
            CheckGround(); //땅에 닿는 판정인지 확인
        }

        //공중에 떠 있는 시간 계산(jump와 flip 동시 실행 방지)
        if (currentstate == PlayerState.Leviating)
        {
            airtime += Time.deltaTime;
        }
        else
        {
            airtime = 0f;
        }

        //플레이어 상태에 따른 동작
        switch (currentstate)
        {

            case PlayerState.Idle:
                Move();
                break;
            case PlayerState.Move:
                Move();
                break;
            case PlayerState.Leviating:
                Move();
                break;

            case PlayerState.Flip:
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = 0f;
                Rotate();
                break;

            case PlayerState.Death:
                break;

        }

        //flip 상태일 때 플레이어 위치 고정
        if (currentstate == PlayerState.Flip)
        {
            transform.position = MyPos; //플레이어 위치 고정
        }
        else
        {
            MyPos = transform.position; //플레이어 위치 추적
        }

        //flip 상태일 때 플레이어 회전하다가 충돌이 일어났을 때 물리적 회전 속도 제한
        if (currentstate == PlayerState.Flip)
        {
            GetComponent<Rigidbody2D>().angularVelocity = 0f;
        }

        //leviating 상태일 때(점프 이외의 방식으로 체공할 때)

    }


    //jump 함수, Leviating으로 상태 변화
    public void Jump(float power)
    {
        GetComponent<Rigidbody2D>().AddForce(new Vector3(0, power, 0), ForceMode2D.Impulse);
        ChangeState(PlayerState.Leviating);
    }

    // Leviating > Move 상태 변화 조건 추가
    bool CanChangeFromLeviating()
    {
        if (currentstate != PlayerState.Leviating)
            return true;

        return Time.time - leviatingEnterTime > leviatingLockTime;
    }

    //move 함수
    private void Move()
    {
        float moveInput = movement.x;
        if (!isGround)
        { ChangeState(PlayerState.Leviating); }
        // move + jump => PlayerState = Leviating

        if (currentstate == PlayerState.Leviating)
        {
            if (Mathf.Abs(moveInput) > 0)
            {
                rb.AddForce(new Vector2(moveInput * moveForce, 0f), ForceMode2D.Force);
            }
        }
        // only move => PlayerState = Move
        else if (Mathf.Abs(moveInput) > 0)
        {
            rb.AddForce(new Vector2(moveInput * moveForce, 0f), ForceMode2D.Force);
            ChangeState(PlayerState.Move);
            if (Mathf.Abs(moveInput) > 0)
            {
                rb.AddForce(new Vector2(moveInput * moveForce, 0f), ForceMode2D.Force);
                ChangeState(PlayerState.Move);

                // if (CanChangeFromLeviating())
                // {
                //     ChangeState(PlayerState.Move);
                // }
            }
        }
        else if (Mathf.Abs(moveInput) == 0)
        {
            ChangeState(PlayerState.Idle);
        }

        // 최대 속도 제한 (속도가 너무 무한정 빨라지는 것 방지)
        if (Mathf.Abs(rb.linearVelocity.x) > maxSpeed)
        {
            float limitedX = Mathf.Clamp(rb.linearVelocity.x, -maxSpeed, maxSpeed);
            rb.linearVelocity = new Vector2(limitedX, rb.linearVelocity.y);
        }

    }

    //flip 함수, Flip으로 상태 변화
    public void Flip()
    {
        //체공시간 너무 적으면 flip 변환 불가(jump와 flip 동시 실행방지)
        if (airtime < 0.2f) return;
        //회전 중에 flip상태가 걸리면 플레이어의 회전 속도 제한
        if (rb != null)
        {
            rb.angularVelocity = 0f;
        }
        ChangeState(PlayerState.Flip);
    }

    //rotate 함수
    private void Rotate()
    {
        float RotateAmount = -rotatemovement.x * rotateSpeed * Time.deltaTime;
        transform.Rotate(0, 0, RotateAmount);
    }

    //MakeBrick 함수
    public void MakeBrickAndRespawn()
    {
        if (boxprefab != null)
        {
            Instantiate(boxprefab, transform.position, transform.rotation); //박스 프리펩
        }
        Destroy(gameObject); //플레이어 제거
        Respawn(); //리스폰
    }

    //respawn 함수, Idle로 상태 변화
    //LevelManager에 리스폰 요청
    private void Respawn()
    {
        LevelManager.Instance.RequestRespawn();
        ChangeState(PlayerState.Idle);
    }

    //플레이어가 가시에 닿을 시 플레이어제거, Death로 상태 변화, 리스폰(플레이어 제거보다 먼저호출)
    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.CompareTag("Spike"))
        {
            ChangeState(PlayerState.Death);
            Respawn();
            Destroy(gameObject);
        }
    }

    //플레이어가 땅에서 떨어지면 Leviating으로 상태 변화(이 함수는 필요없나?)
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            ChangeState(PlayerState.Leviating);
            isGround = false;
        }
    }

    //땅에 닿는 판정인지 확인

    private void CheckGround()
    {
        Collider2D col = GetComponent<Collider2D>();
        Vector2 boxSize = new Vector2(col.bounds.size.x, 0.1f);

        // 박스를 쏠 위치 (발바닥 지점) 및 거리
        Vector2 castOrigin = (Vector2)transform.position + Vector2.down * (col.bounds.extents.y);
        float castDistance = 0.1f;

        // 아래 방향으로 박스를 쏴서 바닥 레이어와 충돌하는지 확인
        RaycastHit2D hit = Physics2D.BoxCast(castOrigin, boxSize, 0f, Vector2.down, castDistance, groundlayer);

        if (hit.collider != null)
        {
            isGround = true;
            //땅에 닿으면 Idle로 상태변경
            if (currentstate == PlayerState.Leviating)
            {
                ChangeState(PlayerState.Idle);
            }
        }
        else
        {
            //땅에서 떨어지면 Leviating으로 상태변경
            isGround = false;
            if (currentstate == PlayerState.Idle)
            {
                ChangeState(PlayerState.Leviating);
            }
        }
    }


    // float distance = DistanceToFoot()+0.1f;
    // RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector3.down, distance, groundlayer);

    // if (hit.collider != null) {

    //     isGround = true;
    //     //땅에 닿으면 Idle로 상태변경
    //     if (currentstate == PlayerState.Leviating) { 
    //         ChangeState(PlayerState.Idle);
    //     }
    // }
    // else {
    //     //땅에서 떨어지면 Leviating으로 상태변경
    //     isGround = false;
    //     if (currentstate == PlayerState.Idle) { 
    //         ChangeState(PlayerState.Leviating);
    //     }
    // }

    // // 에디터 뷰에서 레이저가 나가는지 확인용
    // Debug.DrawRay(transform.position, Vector3.down * distance, Color.red);
    //Raycast 사용하여 player의 수직 아래로 레이어 발사, 물체에 닿으면 땅에 닿는다 판정
    //닿은 물체의 Layer가 Ground일 때만 판정, 판정하려는 물체의 Layer 확인!!

    // 중심점에서 발바닥까지의 거리
    float DistanceToFoot()
    {
        Collider2D col = GetComponent<Collider2D>();

        float distance = col.bounds.extents.y;
        return distance;

    }


}

