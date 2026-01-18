using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    PlayerController playerController;
    Animator animator;
    SpriteRenderer spriteRenderer;
    Rigidbody2D rb;

    bool _a; // playerState 변화 감지

    void Awake()
    {
        // PlayerController에서 Player의 state 가져오기 위함.
        playerController = GetComponent<PlayerController>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        if (playerController == null)
            Debug.LogError("PlayerController 없음");

        if (animator == null)
            Debug.LogError("Animator 없음");

        animator.SetInteger("State", (int)playerController.currentstate);

    }


    void OnEnable()
    {
        Debug.Log("🟢 PlayerAnimator OnEnable");
        playerController.OnStateChanged += HandleStateChanged;
    }

    void OnDisable()
    {
        playerController.OnStateChanged -= HandleStateChanged;
    }


    void HandleStateChanged(PlayerController.PlayerState state)
    {
        switch (state)
        {
            case PlayerController.PlayerState.Idle:
                animator.SetInteger("State", 0); // Idle
                break;

            case PlayerController.PlayerState.Move:
                animator.SetInteger("State", 1); // Move
                ChangeMoveDir();
                break;

            case PlayerController.PlayerState.Leviating:
                animator.SetInteger("State", 2); // Leviating
                break;

            case PlayerController.PlayerState.Flip:
                animator.SetInteger("State", 3); // Flip
                break;
            case PlayerController.PlayerState.Death:
                animator.SetInteger("State", 4);
                break;
        }
    }


    private void ChangeMoveDir()
    {
        float moveX = playerController.movement.x;

        if (moveX < 0)
        {
            spriteRenderer.flipX = true;
            animator.SetBool("isWalking", true);
        }
        else if (moveX > 0)
        {
            spriteRenderer.flipX = false;
            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }
    }
}


// void Update()
// {
//     // PlayerController에서 MoveX 가져오기 (idle과 move 구분하는 용도)
//     int nowState = (int)playerController.currentstate;

//     switch (nowState)
//     {
//         case 0:
//             AnimationFlip();
//             break;

//         case 1:
//             animator.SetInteger("State", (int)playerController.currentstate);
//             break;

//         case 2:
//             animator.SetInteger("State", (int)playerController.currentstate);
//             break;

//         case 3:
//             animator.SetInteger("State", (int)playerController.currentstate);
//             break;


//     }
// }