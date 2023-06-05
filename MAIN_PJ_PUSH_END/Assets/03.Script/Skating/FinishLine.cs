using FreeNet;
using JEEWOO.NET;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FinishLine : MonoBehaviour
{
    public Transform rayPos;

    private RaycastHit hit;
    int count = 0;
    bool isFinish = false;

    GameObject gameoverPanel;
    Button restartBtn;

    // Start is called before the first frame update
    void Awake()
    {
        gameoverPanel = GameObject.Find("Canvas/Panel");
        gameoverPanel.SetActive(false);
        //restartBtn = gameoverPanel.GetComponentInChildren<Button>();
        //restartBtn.onClick.AddListener(() => OnRestartClick());
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(rayPos.position, rayPos.forward * 30.0f, Color.green);

        if (Physics.Raycast(rayPos.position, rayPos.forward, out hit, 30.0f))
        {
            if (hit.collider.CompareTag("Player"))
            {
                if (!isFinish)
                {
                    isFinish = true;
                    count++;
                    Debug.Log($"Count = {count}");

                    CPacket pack = CPacket.create((short)PROTOCOL.GAME_END_REQ);
                    pack.push(hit.collider.GetComponent<CPlayerInfo>().USER_ID);

                    CNetworkManager.Instance.send(pack);
                    //GameOverUIEnable();
                }
            }
        }
    }

    private void GameOverUIEnable()
    {
        gameoverPanel.SetActive(true);
        Cursor.visible = true;
        //Cursor.lockState = CursorLockMode.Confined;
    }
}
