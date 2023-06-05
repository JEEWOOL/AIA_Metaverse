using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlotTooltip : MonoBehaviour
{
    public GameObject go_Base;
    public Text item_Name;
    public Text item_Desc;

    public void ShowToolTip(Item _item, Vector3 _pos)
    {
        go_Base.SetActive(true);
        _pos += new Vector3(go_Base.GetComponent<RectTransform>().rect.width * 0.5f, -go_Base.GetComponent<RectTransform>().rect.height * 0.5f, 0f);
        go_Base.transform.position = _pos;

        item_Name.text = _item.itemName;
        item_Desc.text = _item.itemDescription;
    }

    public void HideToolTip()
    {
        go_Base.SetActive(false);
    }
}
