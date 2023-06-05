using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterConrtoller_t : MonoBehaviour
{
    public Transform characterBody;
    public Transform cameraArm;

    Animator anim;

    public float currentSpeed = 5f;
    public float lDownSpeed = 8f;
    public float charSpeed;
    bool lDown;

    private void Start()
    {
        anim = characterBody.GetComponent<Animator>();
        charSpeed = currentSpeed;
    }

    private void Update()
    {
        LookAround();
        Move();
    }

    void GetInput()
    {
        lDown = Input.GetKey(KeyCode.LeftShift);
    }

    private void Move()
    {
        Vector2 moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        bool isMove = moveInput.magnitude != 0 && !lDown;
        anim.SetBool("isWalk", isMove);
        if (isMove)
        {
            Vector3 lookForward = new Vector3(cameraArm.forward.x, 0f, cameraArm.forward.z).normalized;
            Vector3 lookRight = new Vector3(cameraArm.right.x, 0f, cameraArm.right.z).normalized;
            Vector3 moveDir = lookForward * moveInput.y + lookRight * moveInput.x;

            characterBody.forward = lookForward;            

            if (lDown)
            {
                charSpeed = lDownSpeed;
            }
            else
            {
                charSpeed = currentSpeed;
            }
            transform.position += moveDir * Time.deltaTime * charSpeed;
        }
    }

    private void LookAround()
    {
        Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        Vector3 camAngle = cameraArm.rotation.eulerAngles;
        float x = camAngle.x - mouseDelta.y;

        if(x < 180f)
        {
            x = Mathf.Clamp(x, -1f, 70f);
        }
        else
        {
            x = Mathf.Clamp(x, 355f, 361f);
        }

        cameraArm.rotation = Quaternion.Euler(x, camAngle.y + mouseDelta.x, camAngle.z);
    }
}
