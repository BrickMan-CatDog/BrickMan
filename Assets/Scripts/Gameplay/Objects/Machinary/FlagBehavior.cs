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
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(PLAYER_TAG))
        {
            Debug.Log("깃발 닿음");

            LevelManager.Instance.SetSpawnPoint(transform.position);
            flagCollider.enabled = false;
        }
    }
}
