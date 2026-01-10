using UnityEngine;
using static Constants;

public class StarBehavior : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(PLAYER_TAG))
        {
            Debug.Log("별 먹음");

            // 별 먹을 때 처리

            Destroy(gameObject);
        }
    }
}
