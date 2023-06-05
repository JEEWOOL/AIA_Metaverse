using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollow : MonoBehaviour
{
    public Transform playerTarget;

    private void LateUpdate()
    {
        transform.position = GameObject.Find("CamPos").transform.position;
        transform.rotation = GameObject.Find("CamPos").transform.rotation;
    }
}
