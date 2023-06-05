using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject target; // 카메라가 따라다닐 대상
    public float rotationSpeed = 10f; // 회전 속도
    public float distance = 7f; // 대상과의 거리

    private bool isRotating = false; // 회전 중인지 여부를 나타내는 플래그


    private void Awake()
    {
        // target 오브젝트 초기화
        target = GameObject.FindWithTag("MineTrap");
    }

    private void Update()
    {
        if (isRotating)
        {
            // 카메라 회전
            float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
            float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed;

            transform.RotateAround(target.transform.position, Vector3.up, mouseX);
            transform.RotateAround(target.transform.position, -transform.right, mouseY);
        }
    }

    public void RotateCamera()
    {
        /*
        // 카메라 위치와 회전 초기화
        transform.position = target.transform.position;
        transform.rotation = target.transform.rotation;*/

        // 회전 가능한 상태로 설정
        isRotating = true;
    }

    public void StopRotateCamera()
    {
        // 회전 중지
        isRotating = false;
    }

    private void LateUpdate()
    {
        if (target != null)
        {
            // 대상 오브젝트 앞에 카메라 위치 설정
            Vector3 desiredPosition = target.transform.position - target.transform.forward * distance;
            transform.position = desiredPosition;
            transform.LookAt(target.transform.position);
        }
    }
}
