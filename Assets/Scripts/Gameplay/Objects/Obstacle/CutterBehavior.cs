using System.Collections;
using UnityEngine;
using static Constants;

public class CutterBehavior : MonoBehaviour
{
    public bool isMove = true;
    public Transform targetPoint; // 이동할 목적지 (오브젝트)
    public float moveSpeed = 3f;  // 이동 속도
    public float waitTime = 1f;   // 대기 시간


    private Vector3 startPos;
    private Vector3 endPos;

    void Start()
    {
        startPos = transform.position;
        endPos = targetPoint.position;

        if (isMove)
        {
            StartCoroutine(MoveRoutine());
        }
    }

    public void toggleMovement()
    {
        isMove = !isMove;

        if (isMove)
        {
            StartCoroutine(MoveRoutine());
        }
        else
        {
            StopAllCoroutines();
        }
    }

    IEnumerator MoveRoutine()
    {
        while (true)
        {
            yield return StartCoroutine(MoveToTarget(endPos));
            yield return new WaitForSeconds(waitTime);
            yield return StartCoroutine(MoveToTarget(startPos));
            yield return new WaitForSeconds(waitTime);
        }
    }

    // 실제 이동을 담당하는 하위 코루틴
    IEnumerator MoveToTarget(Vector3 target)
    {
        // 목표에 도달할 때까지 반복
        while (Vector3.Distance(transform.position, target) > 0.01f)
        {
            // 현재 위치에서 목표 위치로 speed만큼 이동
            transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
            
            yield return null; 
        }

        // 오차 보정
        transform.position = target;
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag(PLAYER_TAG))
        {
            Debug.Log("절단기에 닿음");
            Debug.Log("죽음");

            // 절단기에 닿았을 때 처리
            // collision.collider.gameObject.GetComponent<PlayerController>().Death();
            
        }
    }
}
