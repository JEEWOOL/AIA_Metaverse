using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage_Change : MonoBehaviour
{
    public string stageName;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Loading.LoadScene(stageName);
        }
    }
}
