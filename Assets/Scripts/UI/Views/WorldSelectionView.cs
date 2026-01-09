using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class WorldSelectionView : MonoBehaviour, IDragHandler, IEndDragHandler
{
    const int WORLD_COUNT = 4;

    public ScrollRect scrollRect;
    public RectTransform contentTrans;
    public RectTransform slotTrans;
    public HorizontalLayoutGroup hlg;

    public float snapSpeed = 2f;
    float snapTime = 0;
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
            snapTime += Time.deltaTime;
            float t = snapTime * snapSpeed;
            contentTrans.localPosition = Vector3.Lerp(contentTrans.localPosition, targetPos, t);
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
        worldNum = Mathf.RoundToInt(-contentTrans.localPosition.x / (hlg.spacing + slotTrans.rect.width));
        if(worldNum < 0) worldNum = 0;
        if(worldNum > WORLD_COUNT-1) worldNum = WORLD_COUNT-1;
        //Debug.Log(worldNum);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        targetPos = new Vector3(-(worldNum*(hlg.spacing + slotTrans.rect.width)), contentTrans.localPosition.y, contentTrans.localPosition.z);
        snapTime = 0;
        isSnapping = true;
    }
}
