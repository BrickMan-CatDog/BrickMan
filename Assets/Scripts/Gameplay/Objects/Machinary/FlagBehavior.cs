using System.Collections;
using static Constants;
using Unity.Cinemachine;
using UnityEngine;

public class FlagBehavior : MonoBehaviour
{
    Collider2D flagCollider;

    public GameObject flag_enablePrefab;

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
            Destroy(gameObject);

            // flag_enable로 교체
            Instantiate(flag_enablePrefab, transform.position, Quaternion.identity);
        }
    }
}
