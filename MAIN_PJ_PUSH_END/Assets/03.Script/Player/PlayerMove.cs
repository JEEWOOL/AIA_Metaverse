using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    float hAxis;
    float vAxis;
    bool rDown;    
    bool isJumping = false;
    float gravity = -20f;
    float yVelocity = 0;
    float jumpPower = 7f;
    float playerMoveSpeed = 5;
    public float rotSpeed = 200;
    float mx = 0;    

    CharacterController cc;
    Animator anim;

    private void Awake()
    {
        cc = gameObject.GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        GetInput();
        Move();
    }

    private void LateUpdate()
    {
        float mouse_X = Input.GetAxisRaw("Mouse X");        

        mx += mouse_X * rotSpeed * Time.deltaTime;        

        transform.eulerAngles = new Vector3(0, mx, 0);
    }

    void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        rDown = Input.GetKey(KeyCode.LeftShift);        
    }

    void Move()
    {
        Vector3 dir = new Vector3(hAxis, 0, vAxis).normalized;

        anim.SetBool("isWalk_F", dir != Vector3.zero);
        anim.SetBool("isRun", rDown);

        dir = Camera.main.transform.TransformDirection(dir);
        
        if (Input.GetButtonDown("Jump") && !isJumping)
        {
            anim.SetTrigger("isJump");
            yVelocity = jumpPower;
            isJumping = false;
        }

        yVelocity += gravity * Time.deltaTime;
        dir.y = yVelocity;

        cc.Move(dir * playerMoveSpeed * (rDown ? 1.5f : 1f) * Time.deltaTime);
    }
}
