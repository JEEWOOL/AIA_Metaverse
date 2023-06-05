using JEEWOO.NET;
using FreeNet;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateMovement : MonoBehaviour
{
    CProcessPacket processPacket;

    private Animator animator;
    private CharacterController controller;
    private new Transform transform;


    CPlayerInfo playerInfo;

    // 일정 크기 이상의 변화일 때만 패킷을 전송하기 위해서(적당한 간격으로 패킷 전송하려고)
    Vector3 prePosition = Vector3.zero; // 이전 프레임의 위치정보
    Vector3 preRotator = Vector3.zero;  // 이전 프레임의 회전정보

    /*서버로 비교/전송할 Transform정보*/
    Vector3 nowMoveDir = Vector3.zero;   // 현재 controller.SimpleMove로 이동하는 벡터값 
    Vector3 nowPosition = Vector3.zero;  // 현재 프레임의 위치정보
    Vector3 nowRotator = Vector3.zero;  // 현재 프레임의 회전정보
    void Start()
    {
        processPacket = CProcessPacket.Instance;
        controller = GetComponent<CharacterController>();
        transform = GetComponent<Transform>();
        animator = GetComponent<Animator>();

        playerInfo = GetComponent<CPlayerInfo>();

        CProcessPacket.Instance.OnDispatchTransform += OnRecvTransform;
    }

    // Update is called once per frame
    void Update()
    {
        if(playerInfo.IS_MINE)
        {
            SendReqTransformPlayer();
        }
    }

    public void SendReqTransformPlayer()
    {
        nowPosition = transform.position;
        nowRotator = transform.eulerAngles;

        // 이동크기나 회전각도가 일정범위 미만일 때는 패킷 전송하지 않는다.
        if ((Vector3.Distance(nowPosition, prePosition) < 0.05f) &&
            (Mathf.Abs(nowRotator.y - preRotator.y) < 5.0f))
            return;

        CPacket msg = CPacket.create((short)PROTOCOL.TRANSFORM_PLAYER_REQ);

        msg.push(playerInfo.USER_ID);
        // controller.SimpleMove로 이동해야 하는 값
        msg.push(nowMoveDir.x);
        msg.push(nowMoveDir.y);
        msg.push(nowMoveDir.z);
        // transform.position
        msg.push(nowPosition.x);
        msg.push(nowPosition.y);
        msg.push(nowPosition.z);
        // transform.eulerAngles
        msg.push(nowRotator.x);
        msg.push(nowRotator.y);
        msg.push(nowRotator.z);
        // animator 변수
        msg.push(animator.GetInteger("aniStep"));

        CNetworkManager.Instance.send(msg);

        prePosition = nowPosition;
        preRotator = nowRotator;
    }

    void OnRecvTransform(string uid, Vector3 moveDir, Vector3 pos, Vector3 euler, int aniState)
    {
        // 직접 움직이는 플레이어가 아니고
        // USER_ID가 일치하는 플레이어에 이 Transform을 적용해라
        if (!playerInfo.IS_MINE && uid == playerInfo.USER_ID)
        {
            controller.enabled = false;
            //if (controller.enabled)
            //    controller.SimpleMove(moveDir);
            transform.position = pos;
            transform.eulerAngles = euler;
            controller.enabled = true;
            animator.SetInteger("aniStep", aniState);
            //animator.SetFloat("Forward", forward);
            //animator.SetFloat("Strafe", strafe);
        }
    }

    private void OnDestroy()
    {
        CProcessPacket.Instance.OnDispatchTransform -= OnRecvTransform;
    }
}
