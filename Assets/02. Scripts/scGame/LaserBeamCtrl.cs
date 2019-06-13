using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityRandom = UnityEngine.Random;

public class LaserBeamCtrl : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private Transform tr;



    void Start()
    {
        tr = this.GetComponent<Transform>();
        lineRenderer = this.GetComponent<LineRenderer>();
        lineRenderer.startWidth = 0.3f;
        lineRenderer.endWidth = 0.01f;
        lineRenderer.enabled = false;
        lineRenderer.useWorldSpace = false;
    }

    // Update is called once per frame
    void Update()
    {
        //광선 미리 생성
        Ray ray = new Ray(tr.position + (Vector3.up * 0.02f), tr.forward);

        if (Input.GetMouseButtonDown(0))
        {
            //첫번째 점의 위치 설정 / InverseTransformPoint -> 월드 좌표계에서 로컬좌표계값을 가져옴 / ray.origin 값이 월드값이니 그렇다
            lineRenderer.SetPosition(0, tr.InverseTransformPoint(ray.origin));

            //어떤 물체에 광선이 맞았을 때의 위치를 LineRenderer의 끝점으로 설정
            if (Physics.Raycast(ray, out RaycastHit hit, 100.0f))
                lineRenderer.SetPosition(1, tr.InverseTransformPoint(hit.point));
            else
                lineRenderer.SetPosition(1, tr.InverseTransformPoint(ray.GetPoint(100.0f)));

            StartCoroutine(this.ShowLaserBeam());
        }
    }

    private IEnumerator ShowLaserBeam()
    {
        lineRenderer.enabled = true;
        yield return new WaitForSeconds(UnityRandom.Range(0.081f, 0.09f));
        lineRenderer.enabled = false;
    }
}
