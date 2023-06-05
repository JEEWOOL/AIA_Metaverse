using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInterFaceManager : MonoBehaviour
{
    public Inventory inventory;
    public GameObject go_ExitPanel;    

    private void Start()
    {
        go_ExitPanel.SetActive(false);
    }

    public void InventoryBtn()
    {
        Inventory.inventoryActivated = !Inventory.inventoryActivated;

        if (Inventory.inventoryActivated)
        {
            inventory.OpenInventory();
        }
        else
        {
            inventory.CloseInventory();
        }
    }

    public void MainExitBtn()
    {
        go_ExitPanel.SetActive(true);
    }
    public void YesBtn()
    {
        Loading.LoadScene("Login");
    }
    public void NoBtn()
    {
        go_ExitPanel.SetActive(false);
    }
}
