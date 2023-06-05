using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    GameObject chatCanvas;  // NPC 대화 UI
    GameObject slotMachineUI;   // 슬롯머신 UI


    private void Awake()
    {
        chatCanvas = GameObject.Find("Canvas - NPCChat");
        chatCanvas.SetActive(false);
        slotMachineUI = GameObject.Find("Canvas - SlotMachine");
        slotMachineUI.SetActive(false);
    }
    
    private void OnTriggerStay(Collider other)
    {
        
        if(other.tag == "NPC")
        {
            if(Input.GetKey("e"))
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                chatCanvas.SetActive(true);
            }
            if(Input.GetKey("escape"))
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                chatCanvas.SetActive(false);
            }
        }
        if(other.tag == "SLOTMACHINE")
        {
            if(Input.GetKey("e"))
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                slotMachineUI.SetActive(true);
            }
            if(Input.GetKey("escape"))
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                slotMachineUI.SetActive(false);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "NPC")
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            chatCanvas.SetActive(false);
        }

        if(other.tag == "SLOTMACHINE")
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            chatCanvas.SetActive(false);
        }
    }
}
