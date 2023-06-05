using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabManager : MonoBehaviour
{
    public GameObject go_Bed;
    public GameObject go_Chair;
    public GameObject go_Desk;

    public void Bed()
    {
        go_Chair.SetActive(false);
        go_Desk.SetActive(false);
        go_Bed.SetActive(true);
    }
    public void Chair()
    {
        go_Chair.SetActive(true);
        go_Desk.SetActive(false);
        go_Bed.SetActive(false);
    }
    public void Desk()
    {
        go_Chair.SetActive(false);
        go_Desk.SetActive(true);
        go_Bed.SetActive(false);
    }
}
