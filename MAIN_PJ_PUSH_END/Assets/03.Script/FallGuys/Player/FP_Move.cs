using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FP_Move : MonoBehaviour
{
    // 캐릭터 이동
    public float moveSpeed = 7f;
    float h, v;
    bool rDown;

    // 캐릭터 점프
    CharacterController cc;
    float gravity = -5f;
    float yVelocity = 0;
    public float jumpPower = 6f;
    public bool isJumping = false;
    Animator anim;

    // 카메라
    public float lookSensitivity;
    public float cameraRotationLimit;
    [SerializeField]
    private float currentCameraRotationX = 0;
    public GameObject theCamera;

    // 플레이어 좌우    
    public float rotSpeed = 200f;
    float mx = 0;

    CPlayerInfo playerInfo;


    private void Awake()
    {
        cc = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();        
    }
    private void Start()
    {
        playerInfo = GetComponent<CPlayerInfo>();
        cc.enabled = false;
        Invoke("ControllerEnable", 1f);
    }
    private void Update()
    {
        GameObject.Find("BoxTrigger").GetComponent<Renderer>().enabled = false;

        if (playerInfo.IS_MINE)
        {
            if (cc.enabled)
            {
                GetInput();
                PMove();
                CameraRotation();
                CharacterRotation();
            }
        }
        
    }

    void CharacterRotation()
    {
        float mouse_X = Input.GetAxis("Mouse X");
        mx += mouse_X * rotSpeed * Time.deltaTime;
        transform.eulerAngles = new Vector3(0, mx, 0);
    }

    void CameraRotation()
    {
        float _xRotation = Input.GetAxisRaw("Mouse Y");
        float _cameraRotationX = _xRotation * lookSensitivity;
        currentCameraRotationX += _cameraRotationX;
        currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);

        theCamera.transform.localEulerAngles = new Vector3(-currentCameraRotationX, 0f, 0f);
    }

    void GetInput()
    {
        h = Input.GetAxisRaw("Horizontal");
        v = Input.GetAxisRaw("Vertical");
        rDown = Input.GetKey(KeyCode.LeftShift);        
    }

    void PMove()
    {
        Vector3 dir = new Vector3(h, 0, v);
        dir = dir.normalized;
        dir = Camera.main.transform.TransformDirection(dir);

        anim.SetBool("isWalk_F", dir != Vector3.zero);
        anim.SetBool("isRun", rDown);

        if (Input.GetButtonDown("Jump") && !isJumping)
        {
            anim.SetTrigger("isJump");
            yVelocity = jumpPower;
            StartCoroutine(JumpMotion());
        }

        yVelocity += gravity * Time.deltaTime;
        dir.y = yVelocity;

        cc.Move(dir * moveSpeed * (rDown ? 1.5f : 1f) * Time.deltaTime);
    }
    IEnumerator JumpMotion()
    {
        isJumping = true;
        yield return new WaitForSeconds(0.7f);
        isJumping = false;
    }

    void ControllerEnable()
    {
        cc.enabled = true;
    }
}
