using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// UI에서 받은 입력을 LevelManager와 PlayerController에 전달
/// </summary>
public class InputHandler : MonoBehaviour
{
    PlayerController playercontroller; //플레이어와 연결용 변수

    //move 입력받고 Flip상태가 아닐때만 값 넘겨주기
    public void OnMove(InputAction.CallbackContext context) { 
	    if(playercontroller.currentstate != PlayerController.PlayerState.Flip) {
            Debug.Log("move to" + context.ReadValue<Vector3>());
            playercontroller.movement = context.ReadValue<Vector3>();
        }
    }

    //jump 입력받고 땅에 닿았을 때만 jump 실행
    public void OnJump(InputAction.CallbackContext context) { 
        if (context.performed) {
            if(playercontroller.isGround) {
                Debug.Log("Jump");
                playercontroller.Jump(playercontroller.power);
            }
        }
    }
    //Flip 입력받기
    public void OnFlip(InputAction.CallbackContext context) {
        //flip 입력 받기 전에 Leviating 상태인지 확인
        if(playercontroller.currentstate == PlayerController.PlayerState.Leviating) {
            if(context.performed) {    
                playercontroller.Flip();
            }
        }
    }

    //Rotate 입력받기
    public void OnRotate(InputAction.CallbackContext context) {
        if(context.performed) {
            if(playercontroller.currentstate == PlayerController.PlayerState.Flip) {
                Debug.Log("Rotate");
                playercontroller.rotatemovement = context.ReadValue<Vector3>();
            }
        }
        // 버튼에서 손을 뗐을 때 (Canceled)
        else if (context.canceled) {
            if(playercontroller.currentstate == PlayerController.PlayerState.Flip) {
                Debug.Log("Rotate Stop");
                playercontroller.rotatemovement = Vector3.zero; // 회전 값 초기화
            }
        }
    }

    //MakeBrick 입력받기
    public void OnMakeBrick(InputAction.CallbackContext context) {
        if(context.performed) {
            //Flip 상태일 때만 MakeBrick 가능
            if(playercontroller.currentstate == PlayerController.PlayerState.Flip) {
                Debug.Log("MakeBrick");
                playercontroller.MakeBrickAndRespawn();
            }
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Player 찾고 연결
        playercontroller = GameObject.FindWithTag("Player").GetComponent<PlayerController>(); 
    }

    // Update is called once per frame
    void Update()
    {
        //player가 리스폰되어도 player 찾고 연결
        if (playercontroller == null) { 
            GameObject playerObj = GameObject.FindWithTag("Player");

            if (playerObj != null) {
                playercontroller = playerObj.GetComponent<PlayerController>();
            }
        }

    }

    //idle  땅 접지 - move, jump(spacebar)
    //leviating 공중, - move flip(spacebar)
    //flip  flip 상태, rotate, new prefab
    //사망 respawn
}

