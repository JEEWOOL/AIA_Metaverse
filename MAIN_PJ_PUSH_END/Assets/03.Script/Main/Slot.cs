using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Vector3 originPos;

    public Item item;           // 획득한 아이템
    public int itemCount;       // 획득한 아이템의 갯수
    public Image itemImage;     // 아이템 이미지

    public Text text_Count;
    public GameObject go_CountImage;
    public SlotTooltip theSlot;

    private void Start()
    {
        theSlot = FindObjectOfType<SlotTooltip>();
        originPos = transform.position;
    }

    // 이미지 투명도 조절
    private void SetColor(float _alpha)
    {
        Color color = itemImage.color;
        color.a = _alpha;
        itemImage.color = color;
    }
    // 아이템 획득
    public void AddItem(Item _item, int _count = 1)
    {
        item = _item;
        itemCount = _count;
        itemImage.sprite = item.itemImage;

        if (item.itemType == Item.ItemType.ingredient)
        {
            go_CountImage.SetActive(true);
            text_Count.text = itemCount.ToString();
        }

        SetColor(1);
    }
    // 아이템 갯수 조정
    public void SetSlotCount(int _count)
    {
        itemCount += _count;
        text_Count.text = itemCount.ToString();

        if(itemCount <= 0)
        {
            ClearSlot();
        }
    }
    // 슬롯 초기화
    public void ClearSlot()
    {
        item = null;
        itemCount = 0;
        itemImage.sprite = null;
        SetColor(0);
        text_Count.text = "0";
        go_CountImage.SetActive(false);        
    }

    // 마우스 슬롯 인 / 아웃 이벤트
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(item != null)
        {
            theSlot.ShowToolTip(item, transform.position);
        }        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        theSlot.HideToolTip();
    }
}