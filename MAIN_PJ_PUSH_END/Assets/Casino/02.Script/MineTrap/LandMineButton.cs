using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandMineButton : MonoBehaviour
{
   // public MineTrap minetrap; // 참조를 가져올 변수

    private bool onTriggerStay = false;
    private bool isClick = false;

    private int time = 5;
    

    private void Update()
    {
        if (onTriggerStay)
        {
            if (!isClick)
            {
                if (Input.GetKeyDown("e"))
                {
                
                    isClick = true;
                    //MineTrap
                    StartCoroutine(coTimer());
                }               
            }
        }

    }

    // 버튼 5초 간격으로 눌리게끔
    IEnumerator coTimer()
    {
        yield return new WaitForSeconds(1.0f);
        //Debug.Log(time);
        if (time > 0)
        {
            time -= 1;
            StartCoroutine(coTimer());
        }
        else
        {
            time = 5;
            isClick = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            onTriggerStay = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            onTriggerStay = false;
        }
    }
}
