using Unity.VisualScripting;
using UnityEngine;
using static Constants;

public class HookBehavior : MonoBehaviour
{
    private GameObject hookHead;  // 갈고리 헤드는 무조건 첫 번째 자식으로 고정
    private GameObject initialParent = null;
    private GameObject player = null;
    private Rigidbody2D rb;


    public bool isActivated = false;
    public float power = 0.5f; //갈고리 이동 속도
    public Vector2 releaseMultiplier = new Vector2(2f, 1.5f); //  갈고리에서 하차 시 속도 배수
    public Vector3 movement; // 좌우 움직임 결정
    public float disableDuration = 1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        hookHead = transform.GetChild(0).gameObject;
        rb = hookHead.GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (isActivated)
        {
            //플레이어가 갈고리에 탑승해 있을 때만 움직임 적용
            rb.AddRelativeForce(movement * power);
        }
    }

    public void GetOnHook(Collision2D collision)
    {
        if (!isActivated)
        {
            Debug.Log("갈고리 탑승");
            player = collision.collider.gameObject;
            player.GetComponent<PlayerController>().hookedHook = this;
            player.GetComponent<PlayerController>().ChangeState(PlayerController.PlayerState.OnHook);
            player.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
            player.GetComponent<Rigidbody2D>().simulated = false;
            initialParent = player.transform.parent?.gameObject;
            player.transform.SetParent(hookHead.transform);
            player.transform.localPosition = Vector2.zero;

            isActivated = true;
        }
    }

    public void GetOffHook()
    {
        if (isActivated && player != null)
        {
            Debug.Log("갈고리 하차");
            movement = Vector3.zero;
            player.transform.rotation = Quaternion.identity;
            player.transform.SetParent(initialParent?.transform);
            initialParent = null;
            player.GetComponent<PlayerController>().hookedHook = null;
            player.GetComponent<PlayerController>().ChangeState(PlayerController.PlayerState.Idle);
            player.GetComponent<Rigidbody2D>().simulated = true;
            player.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(rb.linearVelocity.x * releaseMultiplier.x, rb.linearVelocity.y * releaseMultiplier.y);
            player = null;

            hookHead.GetComponent<HookHeadBehavior>().GetOff();
        }
    }


}
