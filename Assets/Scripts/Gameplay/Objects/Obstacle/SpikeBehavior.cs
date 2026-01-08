using UnityEngine;
using static Constants;

public class SpikeBehavior : MonoBehaviour
{
    void Start()
    {
        
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag(PLAYER_TAG))
        {
            Debug.Log("가시에 닿음");
            Debug.Log("죽음");

            // 가시에 닿았을 때 처리
            // collision.collider.gameObject.GetComponent<PlayerController>().Death();

        }
    }
}
