using System;
using UnityEngine;
using static Constants;

public class SpringBehavior : MonoBehaviour
{
    [SerializeField] private float jumpHeight = 2f;  // 높이가 N배가 되려면 힘은 sqrt(N)배 필요
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(PLAYER_TAG))
        {
            Debug.Log("용수철 닿음");
            collision.GetComponent<PlayerController>().jumpMultiplier *= Mathf.Sqrt(jumpHeight);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(PLAYER_TAG))
        {
            Debug.Log("용수철 떠남");
            collision.GetComponent<PlayerController>().jumpMultiplier /= Mathf.Sqrt(jumpHeight);
        }
    }
}
