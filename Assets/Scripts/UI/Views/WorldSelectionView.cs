using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class WorldSelectionView : MonoBehaviour, IDragHandler, IEndDragHandler
{
    //월드 개수 상수
    const int WORLD_COUNT = 1;

    public ScrollRect scrollRect;               //WorldScrollView 넣기
    public RectTransform contentTrans;          //Content 넣기
    public RectTransform slotTrans;             //Worldslot 하나 넣기
    public HorizontalLayoutGroup hlg;           //Content 넣기

    public float snapTime = 1f;
    float snapTimeBase = 0;

    int slotCount = WORLD_COUNT + 1;    //slot 개수 = world 수 + comingsoon

    //현재 선택된 world 번호(0~)
    int worldNum = 0;

    bool isSnapping;
    Vector3 targetPos = Vector3.zero;

    void Start()
    {
        isSnapping = true;
    }

    void Update()
    {
        if (isSnapping)
        {
            //목표 위치로 이동
            snapTimeBase += Time.deltaTime;
            float t = snapTimeBase / snapTime;
            contentTrans.localPosition = Vector3.Lerp(contentTrans.localPosition, targetPos, t);
            //(snapTime)초 후 목표위치에 고정
            if (t >= 1f)
            {
                scrollRect.velocity = Vector2.zero;
                contentTrans.localPosition = targetPos;
                isSnapping = false;
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        isSnapping = false;
        scrollRect.velocity = Vector2.zero;

        //드래그한 위치에 따라 worldNum 계산
        //(-위치.x / (slot 사이 길이 + slot 길이)) 반올림
        worldNum = Mathf.RoundToInt(-contentTrans.localPosition.x / (hlg.spacing + slotTrans.rect.width));
        if(worldNum < 0) worldNum = 0;
        if(worldNum > slotCount-1) worldNum = slotCount-1;
        //Debug.Log(worldNum);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //worldNum에 해당하는 월드의 위치로  목표위치 설정
        targetPos = new Vector3(-(worldNum*(hlg.spacing + slotTrans.rect.width)), contentTrans.localPosition.y, contentTrans.localPosition.z);
        snapTimeBase = 0;
        isSnapping = true;
    }
}
