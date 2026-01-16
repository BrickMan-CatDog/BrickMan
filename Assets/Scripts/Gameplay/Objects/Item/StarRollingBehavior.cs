using UnityEngine;
using static Constants;

public class StarRollingBehavior : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag(PLAYER_TAG))
        {
            Debug.Log("별 먹음");

            LevelManager.Instance.CollectStar();
            Destroy(gameObject);
        }
    }
}
