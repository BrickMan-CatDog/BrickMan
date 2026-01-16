using UnityEngine;
using static Constants;

public class StarBehavior : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(PLAYER_TAG))
        {
            Debug.Log("별 먹음");

            LevelManager.Instance.CollectStar();
            Destroy(gameObject);
        }
    }
}
