using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionController : MonoBehaviour
{
    public float range;                         // 습득 가능한 최대 거리
    public bool pickupActivated = false;        // 습득 가능할 시 true;

    private RaycastHit hitInfo;                 // 충돌체 정보 저장
    public LayerMask layerMask;                // 아이템 레이어에만 반응하도록 레이어 마스크 설정

    public Text actionText;
    public Inventory theInventory;

    private CPlayerInfo playerInfo;

    private void Start()
    {
        playerInfo = GetComponentInParent<CPlayerInfo>();
    }

    private void Update()
    {
        if (playerInfo.IS_MINE)
        {
            CheckItem();
            TryAction();
        }
    }
    void TryAction()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            CheckItem();
            CanPickUp();
        }
    }
    void CanPickUp()
    {
        if (pickupActivated)
        {
            if(hitInfo.transform != null)
            {
                Debug.Log(hitInfo.transform.GetComponent<ItemPickUp>().item.itemName +
            " 획득 ");
                theInventory.AcquireItem(hitInfo.transform.GetComponent<ItemPickUp>().item);
                Destroy(hitInfo.transform.gameObject);
                InfoDisappear();
            }
        }
    }
    void CheckItem()
    {
        if(Physics.Raycast(transform.position, transform.forward, out hitInfo, range, layerMask))
        {
            if(hitInfo.transform.tag == "Item")
            {
                ItemInfoAppear();
            }
        }
        else
        {
            InfoDisappear();
        }
    }
    void ItemInfoAppear()
    {
        pickupActivated = true;
        actionText.gameObject.SetActive(true);
        actionText.text = hitInfo.transform.GetComponent<ItemPickUp>().item.itemName + 
            " 획득 " + "<color=red>" + "(G)" + "</color>";
    }
    void InfoDisappear()
    {
        pickupActivated = false;
        if(actionText != null)
            actionText.gameObject.SetActive(false);
    }
}
