using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPC : MonoBehaviour
{
    public GameObject uInterface;
    public GameObject roomUI;
    public Text npcText;
    public BoxCollider npcBoxcol;

    private void Start()
    {
        roomUI.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && other.GetComponent<CPlayerInfo>().IS_MINE) {
            roomUI.gameObject.SetActive(true);
            uInterface.SetActive(false);
            npcBoxcol.GetComponent<NPC>().GetComponent<BoxCollider>().enabled = false;
            Time.timeScale = 0;
        }
    }

    public void PlayListExitBtn()
    {        
        roomUI.gameObject.SetActive(false);
        uInterface.SetActive(true);
        Time.timeScale = 1;
        StartCoroutine(NpcBoxCol());
    }

    IEnumerator NpcBoxCol()
    {
        yield return new WaitForSeconds(5f);
        npcBoxcol.GetComponent<NPC>().GetComponent<BoxCollider>().enabled = true;
    }
}
