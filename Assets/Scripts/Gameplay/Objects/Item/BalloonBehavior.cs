using System.Collections;
using UnityEngine;
using static Constants;

public class BalloonBehavior : MonoBehaviour
{
    private Collider2D balloonCollider;
    private GameObject player = null;
    [SerializeField] private float availableTime = 1.5f;
    [SerializeField] private float power = 0.07f;
    [SerializeField] private float slowRate = 5f;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        balloonCollider = gameObject.GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(PLAYER_TAG))
        {
            Debug.Log("풍선 먹음");
            balloonCollider.enabled = false;

            player = collision.gameObject;
            transform.SetParent(player.transform);
            transform.localPosition = new Vector3(0, 1f, 0);

            player.GetComponent<PlayerController>().GetBalloon(power, slowRate);
            StartCoroutine("WaitForPop");
        }
    }

    IEnumerator WaitForPop()
    {
        yield return new WaitForSeconds(availableTime);

        if (player != null)
        {
            Debug.Log("풍선 터짐");
            player.GetComponent<PlayerController>().PopBalloon(slowRate);
        }
        Destroy(gameObject);
    }

}
