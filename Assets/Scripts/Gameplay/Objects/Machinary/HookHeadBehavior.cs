using System.Collections;
using UnityEngine;
using static Constants;

public class HookHeadBehavior : MonoBehaviour
{
    private HookBehavior hookBehavior;

    void Start()
    {
        hookBehavior = transform.parent.GetComponent<HookBehavior>();
        GetComponent<HingeJoint2D>().anchor = new Vector2(0, -transform.localPosition.y);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag(PLAYER_TAG))
        {
            Debug.Log("갈고리 헤드 충돌 감지");
            hookBehavior.GetOnHook(collision);
            // GetComponent<Collider2D>().enabled = false;
        }
    }

    public void GetOff()
    {
        GetComponent<Collider2D>().enabled = false;
        StartCoroutine(WaitAndEnable(hookBehavior.disableDuration));
    }

    IEnumerator WaitAndEnable(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        GetComponent<Collider2D>().enabled = true;
        hookBehavior.isActivated = false;
    }
}
