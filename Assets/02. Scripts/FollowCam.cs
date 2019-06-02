using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    public Transform targetTransform;
    public float dist = 10.0f;
    public float height = 3.0f;
    public float dampTrace = 20.0f;

    private Transform tr;
    void Start()
    {
        tr = this.GetComponent<Transform>();
    }


    /// <summary>
    /// Update 함수 호출 이후 한 번씩 호출됨
    /// 추적할 타깃의 이동이 종료된 이후에 카메라가 추적하기위해 LateUpdate 사용
    /// </summary>
    void LateUpdate()
    {
        //카메라의 위치를 추적대상의 dist변수만큼 뒤쪽으로 배치하고 height만큼 올림
        tr.position = Vector3.Lerp(
            tr.position,        //시작 위치
            targetTransform.position - (targetTransform.forward * dist) + (Vector3.up * height), //종료위치
            Time.deltaTime * dampTrace //보간 시간
            );
        tr.LookAt(targetTransform);
    }
}
