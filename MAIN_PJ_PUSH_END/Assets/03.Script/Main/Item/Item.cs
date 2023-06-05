using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "New Item/item")]
public class Item : ScriptableObject
{
    // 아이템 이름
    public string itemName;
    // 아이템 설명
    [TextArea]
    public string itemDescription;
    // 아이템 종류
    public ItemType itemType;
    // 아이템 이미지
    public Sprite itemImage;
    // 아이템의 프리팹
    public GameObject itemPrefab;

    public enum ItemType
    {
        ingredient
    }
}
