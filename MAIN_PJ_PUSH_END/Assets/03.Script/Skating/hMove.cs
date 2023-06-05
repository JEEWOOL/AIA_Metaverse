using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hMove : MonoBehaviour
{
    Vector3 pos;
    float delta = 2.0f;
    float speed = 3.0f;
    Transform tr;

    Vector3 startPos;
    Collider cubeCollider;
    bool isRespawn;

    void Start()
    {
        pos = transform.position;
        tr = GetComponent<Transform>();
        startPos = tr.position;
        cubeCollider = this.GetComponent<Collider>();
    }

    void Update()
    {
        if (!isRespawn)
        {
            Vector3 v = pos;
            v.x += delta * Mathf.Sin(Time.time * speed);
            transform.position = v;
        }
        else
            tr.position = startPos;

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
}
