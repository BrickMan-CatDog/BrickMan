using UnityEngine;
using static Constants;

public class DelTileBehavior : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("오브젝트 삭제");

        if (collision.collider.CompareTag(PLAYER_TAG))
        {
            Debug.Log("갈고리 헤드 충돌 감지");
            PlayerController playerController = collision.collider.GetComponent<PlayerController>();
            playerController.ChangeState(PlayerController.PlayerState.Death);
            LevelManager.Instance.RequestRespawn();
            Destroy(playerController.gameObject);
            // GetComponent<Collider2D>().enabled = false;
        }
        Destroy(collision.collider.gameObject);
    }
}
