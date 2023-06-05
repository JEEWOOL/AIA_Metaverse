using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RmoveHard : MonoBehaviour
{
    Transform tr;

    float rotationSpeed = 50.0f;
    float moveSpeed = 5.0f;

    int randomRotate;
    bool isRotate = true;
    int count;

    Vector3 startPos;
    Collider cubeCollider;
    bool isRespawn;


    void Start()
    {
        tr = GetComponent<Transform>();
        startPos = tr.position;
        cubeCollider = this.GetComponent<Collider>();
    }

    private void Update()
    {
        if (!isRespawn)
        {
            if (isRotate)
                StartCoroutine(coRandRotate());
            StartCoroutine(coRandMove());
        }
        else
            tr.position = startPos;

    }

    IEnumerator coRandRotate()
    {
        if (count < 1)
        {
            count++;
            randomRotate = Random.Range(-5, 5);
            //Debug.Log("Rand rot = " + randomRotate);
        }
        tr.Rotate(Vector3.up * rotationSpeed * Time.deltaTime * randomRotate);
        yield return new WaitForSeconds(1.0f);
        isRotate = false;
        count = 0;
        yield return new WaitForSeconds(1.0f);
        isRotate = true;

    }

    IEnumerator coRandMove()
    {
        tr.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        yield return new WaitForSeconds(1.0f);

    }

    // 큐브 안전구역 갔을 시 리스폰 로직
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("SAFEZONE"))
        {
            isRespawn = true;
            cubeCollider.enabled = false;
            Invoke("CubeOperate", 2.0f);
        }
    }

    void CubeOperate()
    {
        cubeCollider.enabled = true;
        isRespawn = false;
    }

    // 벽에 부딪혔을 때 로직
    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("WALL"))
        {
            tr.Rotate(0, 180, 0);
        }
    }
}
