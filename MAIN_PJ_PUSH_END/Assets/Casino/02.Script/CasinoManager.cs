using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CasinoManager : MonoBehaviour
{
    public GameObject player;

    private void Awake()
    {
        Transform startPoint = GameObject.Find("StartPos").GetComponent<Transform>();
        GameObject.Instantiate(player, startPoint.position, startPoint.rotation);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
