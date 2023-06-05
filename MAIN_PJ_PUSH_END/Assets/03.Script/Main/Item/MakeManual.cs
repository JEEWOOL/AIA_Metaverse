using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Make
{
    public string MakeName;
    public GameObject makePrefab;
    public GameObject previewPrefab;
}

public class MakeManual : MonoBehaviour
{
    private bool isMenual = false;
    private bool isPreview = false;

    public GameObject baseUI;

    public Make[] make_bed;

    private GameObject lookPreview;
    private GameObject makePreviewPrefab;

    public Transform playerPos;

    private RaycastHit hitInfo;
    public LayerMask layerMask;
    public float range;

    private void Start()
    {
        playerPos = GameObject.Find("MakeOBJpos").transform;
    }

    public void Slot(int _slotNumber)
    {
        lookPreview = Instantiate(make_bed[_slotNumber].previewPrefab, playerPos.position + playerPos.forward, Quaternion.identity);
        makePreviewPrefab = make_bed[_slotNumber].makePrefab;
        isPreview = true;
        baseUI.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && !isPreview)
        {
            ManualWindow();
        }
        if (isPreview)
        {
            PreviewPos();
        }
        if (Input.GetButtonDown("Fire1"))
        {
            Build();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cancellation();
        }
    }

    private void Build()
    {
        if (isPreview)
        {
            Instantiate(makePreviewPrefab, hitInfo.point, Quaternion.identity);
            Destroy(lookPreview);
            isMenual = false;
            isPreview = false;
            lookPreview = null;
            makePreviewPrefab = null;

            baseUI.SetActive(false);
        }
    }

    private void PreviewPos()
    {
        if(Physics.Raycast(playerPos.position, playerPos.forward, out hitInfo, range, layerMask))
        {
            if(hitInfo.transform != null)
            {
                Vector3 pos = hitInfo.point;
                lookPreview.transform.position = pos;
            }
        }
    }

    private void Cancellation()
    {
        if (!isPreview)
        {
            Destroy(lookPreview);
        }
        isPreview = false;
        isMenual = false;
        lookPreview = null;

        baseUI.SetActive(false);
    }

    private void ManualWindow()
    {
        if (!isMenual)
        {
            OpenMenual();
        }
        else
        {
            CloseMenual();
        }
    }

    private void OpenMenual()
    {
        isMenual = true;
        baseUI.SetActive(true);
    }

    private void CloseMenual()
    {
        isMenual = false;
        baseUI.SetActive(false);
    }    
}