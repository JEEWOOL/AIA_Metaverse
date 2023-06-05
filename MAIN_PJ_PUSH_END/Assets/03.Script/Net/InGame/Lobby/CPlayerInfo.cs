using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPlayerInfo : MonoBehaviour
{
    public string USER_ID { get; set; }
    public string USER_NAME { get; set; }
    public int room_idx;


    // true¸é Local Player
    public bool IS_MINE { get; set; }
}
