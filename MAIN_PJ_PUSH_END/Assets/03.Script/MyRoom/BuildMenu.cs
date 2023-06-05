using FreeNet;
using JEEWOO.NET;
using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using Unity.VisualScripting;

[System.Serializable]
public class Build
{
    public string buildName;
    public short buildItemNum;
    public GameObject go_Prefab;
    public GameObject go_PreviewPrefab;
}

[Serializable]
public struct BuildedObj
{
    public short itemNum;
    public Vector3 postion;
    public Quaternion rotation;
}

[Serializable]
public class BuildMenu : MonoBehaviour
{
    private static BuildMenu _instance = null;

    public static BuildMenu Instance
    {
        get
        {
            return _instance;
        }
    }
    private bool isActivated = false;
    private bool isPreviewActivated = false;

    public GameObject go_BaseUI;

    // 건물 배치
    public Build[] build_Bed;
    public GameObject go_PreView;
    private GameObject go_Prefab;
    private short selNum;
    private List<BuildedObj> buildedList = new List<BuildedObj>();
    private Quaternion buildRot = Quaternion.identity;
    public Transform player;

    private RaycastHit hitInfo;
    public LayerMask layerMask;
    public float range;

    private void Awake()
    {
        _instance = this;
        go_BaseUI.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && !isPreviewActivated)
        {
            Window();
        }
        if (isPreviewActivated)
        {
            PreviewPositionUpdate();
            RotUpdate();
        }
        if (Input.GetMouseButtonDown(0))
        {
            MakeBuild();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cancel();
        }
    }

    private void RotUpdate()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            buildRot = Quaternion.Euler(buildRot.eulerAngles.x, buildRot.eulerAngles.y - 1.5f, buildRot.eulerAngles.z);
        }
        if (Input.GetKey(KeyCode.E))
        {
            buildRot = Quaternion.Euler(buildRot.eulerAngles.x, buildRot.eulerAngles.y + 1.5f, buildRot.eulerAngles.z);
        }
    }

    void MakeBuild()
    {
        if (isPreviewActivated && go_PreView.GetComponentInChildren<PreviewObject>().isBuildable())
        {
            Instantiate(go_Prefab, hitInfo.point, buildRot);
            Destroy(go_PreView);
            BuildedObj buildedObj;
            buildedObj.rotation = buildRot;
            buildedObj.postion = hitInfo.point;
            buildedObj.itemNum = selNum;
            buildedList.Add(buildedObj);
            isPreviewActivated = false;
            isActivated = false;
            go_PreView = null;
            go_Prefab = null;
        }
    }

    void PreviewPositionUpdate()
    {
        if (Physics.Raycast(player.position, player.forward, out hitInfo, range, layerMask))
        {
            if (hitInfo.transform != null)
            {
                Vector3 _location = hitInfo.point;
                go_PreView.transform.position = _location;
                go_PreView.transform.rotation = buildRot;
            }
        }
    }

    void Cancel()
    {
        if (isPreviewActivated)
        {
            Destroy(go_PreView);
        }
        isActivated = false;
        isPreviewActivated = false;
        go_PreView = null;
        go_Prefab = null;

        go_BaseUI.SetActive(false);
    }

    public void SlotClick(int _slotNumber)
    {
        go_PreView = Instantiate(build_Bed[_slotNumber].go_PreviewPrefab, player.position + player.forward, buildRot);
        go_Prefab = build_Bed[_slotNumber].go_Prefab;
        selNum = build_Bed[_slotNumber].buildItemNum;
        isPreviewActivated = true;
        go_BaseUI.SetActive(false);
    }

    void Window()
    {
        if (!isActivated)
        {
            OpenWindow();
        }
        else
        {
            CloseWindow();
        }
    }

    private void OpenWindow()
    {
        isActivated = true;
        go_BaseUI.SetActive(true);
    }

    private void CloseWindow()
    {
        isActivated = false;
        go_BaseUI.SetActive(false);
    }

    public void SaveButtonClick()
    {
        string blist = JsonConvert.SerializeObject(buildedList);
        CPacket pack = CPacket.create((short)PROTOCOL.MYROOM_SAVE_REQ);
        pack.push(CProcessPacket.Instance.USER_ID);
        pack.push(blist);
        CNetworkManager.Instance.send(pack);
    }

    public void LoadObjects(string data)
    {
        buildedList = JsonConvert.DeserializeObject<List<BuildedObj>>(data);
        if (buildedList == null)
            return;
        for (int i = 0; i < buildedList.Count; i++)
        {
            Instantiate(build_Bed[buildedList[i].itemNum].go_Prefab, buildedList[i].postion, buildedList[i].rotation);
        }
    }
}
