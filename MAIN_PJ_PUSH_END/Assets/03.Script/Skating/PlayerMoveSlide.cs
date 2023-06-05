using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveSlide : MonoBehaviour
{
    enum P_States
    {
        walk_f = 1,
        walk_b = 2,
        run = 3,
        jump = 4,
        idle = 5
    }

    float hAxis;
    float vAxis;
    bool rDown;
    bool isJumping;
    float gravity = -10;
    float yVelocity = 0;
    float jumpPower = 3f;
    float playerMoveSpeed = 5;
    public float rotSpeed = 200;
    float mx = 0;

    bool isSliding = false;
    Vector3 moveDirection = Vector3.zero;
    float slideForce = 5f;
    float friction = 0.3f;
    float slidingSpeed = 2f;

    private CharacterController cc;
    private Animator anim;


    bool vAxisEnable = true;
    bool hAxisEnable = true;

    bool isHit = false;

    CPlayerInfo playerInfo;

    private void Start()
    {
        playerInfo = GetComponent<CPlayerInfo>();
        cc = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (playerInfo.IS_MINE)
        {
            if (hAxisEnable)
                hAxis = Input.GetAxis("Horizontal");
            if (vAxisEnable)
                vAxis = Input.GetAxis("Vertical");
            Move();
            if (hAxis == 0 && vAxis == 0 && !Input.GetButtonDown("Jump"))
                anim.SetInteger("aniStep", 0);
        }
    }

    private void LateUpdate()
    {
        if (playerInfo.IS_MINE)
        {
            float mouse_X = Input.GetAxisRaw("Mouse X");

            mx += mouse_X * rotSpeed * Time.deltaTime;

            transform.eulerAngles = new Vector3(0, mx, 0);
        }
    }

    private void Move()
    {
        anim.SetInteger("aniStep", 1);
        Vector3 dir = new Vector3(hAxis, 0, vAxis).normalized;


        anim.SetBool("isWalk_F", dir != Vector3.zero);
        anim.SetBool("isRun", rDown);

        dir = Camera.main.transform.TransformDirection(dir);

        if (cc.collisionFlags == CollisionFlags.Below)
        {
            if (isJumping)
            {
                isJumping = false;
                yVelocity = 0;
            }
        }
        if (Input.GetButtonDown("Jump") && !isJumping)
        {
            anim.SetInteger("aniStep", 2);
            //anim.SetBool("isJump", !isJumping);            
            yVelocity = jumpPower;
            isJumping = true;
        }
        yVelocity += gravity * Time.deltaTime;
        dir.y = yVelocity;
        
        if (isSliding)
        {                     
            moveDirection += dir * slideForce * Time.deltaTime;
            
            moveDirection = Vector3.Lerp(moveDirection, Vector3.zero, friction * Time.deltaTime);
                
            cc.Move(moveDirection * slidingSpeed * Time.deltaTime);

            if (cc.isGrounded)
            {                
                isJumping = false;
                yVelocity = 0;
            }
            else
            {
                yVelocity += gravity * Time.deltaTime;
                moveDirection.y = yVelocity;
            }
        }
        else
        {
            cc.Move(dir * playerMoveSpeed * (rDown ? 1.5f : 1f) * Time.deltaTime);
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {

        if (hit.collider.CompareTag("SlideArea"))
        {
            isSliding = true;
            vAxisEnable = false;
            hAxisEnable = false;
            if (!isHit)
                vAxis = 1f;
        }
        else
        {
            isSliding = false;            
            moveDirection = Vector3.zero;
            vAxisEnable = true;
            hAxisEnable = true;
        }
        
        if (hit.collider.CompareTag("OBSTACLE"))
        {
            isHit = true;
            hAxis = 0;
            vAxis = 0;
            Invoke("PlayerMoveEnable", 3.0f);
        }
    }

    void PlayerMoveEnable()
    {
        isHit = false;
    }
}
