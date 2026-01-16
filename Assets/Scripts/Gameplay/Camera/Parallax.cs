using UnityEngine;

public class Parallax : MonoBehaviour
{
    private float length, startPosX, startPosY;
    public GameObject cam;           // 메인 카메라 참조
    public float parallaxEffect;     // 움직임 배율 (0: 고정, 1: 카메라와 동일 속도)
    //1에 가까워질 수록 더 멀리있는 배경

    void Start()
    {
        
        startPosX = transform.position.x;
        // 배경 이미지의 가로 길이, 배경 루프 용
        length = GetComponent<SpriteRenderer>().bounds.size.x;

        startPosY = transform.position.y;
    }

    void FixedUpdate()
    {
        // 카메라가 움직인 거리 계산
        //루프 타이밍 기준점
        /// parallaxEffect = 0.2이면 배경은 카메라에 비해 매우 느리게 이동하고 있다는 뜻
        /// position = 배경의 실제 위치
        /// Position = startPos + (cam * parallaxEffect)
        /// 여기서 카메라($cam$)가 움직일 때, 우리가 보는 카메라와 배경 사이의 상대적인 거리
        /// (카메라 위치) - (배경 위치) = cam - (startPos + cam * parallaxEffect)
        /// = cam * (1 - parallaxEffect) - startPos
        /// temp - startPos > Length 면 
        /// 어짜피 배경은 두개 설치할 것이기 때문에 카메라가 하나의 배경을 벗어나더라도 어색한 공간이 나오지 x
        float temp = (cam.transform.position.x * (1 - parallaxEffect));

        //배경을 움직이는 거리
        float distX = (cam.transform.position.x * parallaxEffect);
        float distY = (cam.transform.position.y * parallaxEffect * 0.5f);
        //parallaxeffect가 1이면 카메라의 움직임만큼 움직이게 됨-> 카메라 시점에서는 안움직임
        // 배경 위치 업데이트
        transform.position = new Vector3(startPosX + distX, startPosY + distY, transform.position.z);

        // 무한 배경 루프 (배경이 화면 끝에 도달하면 위치 재설정)
        if (temp > startPosX + length) startPosX += length; //위치 재설정
        else if (temp < startPosX - length) startPosX -= length; //위치 재설정
    }
}