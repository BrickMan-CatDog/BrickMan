using UnityEditor.UI;
using UnityEngine;
using static Constants;

public class FlagBehavior : MonoBehaviour
{
    Collider2D flagCollider;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        flagCollider = gameObject.GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(PLAYER_TAG))
        {
            Debug.Log("깃발 닿음");

            // 리스폰 포지션 설정 처리
            
            flagCollider.enabled = false;
        }
    }
}
