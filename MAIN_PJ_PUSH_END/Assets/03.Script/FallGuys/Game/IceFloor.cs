using FreeNet;
using JEEWOO.NET;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class IceFloor : MonoBehaviour
{
    Collider coll;
    public Material[] mat = new Material[2];
    public int x, y, z;

    private void Start()
    {
        //coll = gameObject.GetComponent<BoxCollider>();
        //coll.enabled = false;
        //Invoke("ColliderEnable", 5f);
    }

    void OnCollisionEnter(Collision other)
    {
        if(other.collider.tag == "PLAYER")
        {
            ChangeMat(1);
            Invoke("IceDisable", 1f);
            Invoke("IceEnable", 5f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("PLAYER"))
        {
            CPacket pack = CPacket.create((short)PROTOCOL.ICE_BROKE_REQ);
            pack.push(x);
            pack.push(y);
            pack.push(z);
            CNetworkManager.Instance.send(pack);
            ChangeMat(1);
            Invoke("IceDisable", 1f);
            //Invoke("IceEnable", 5f);
        }
    }
    public void TriggerIncome()
    {
        ChangeMat(1);
        Invoke("IceDisable", 1f);
    }
    void IceEnable()
    {
        ChangeMat(0);
        this.gameObject.SetActive(true);
    }

    void IceDisable()
    {
        this.gameObject.SetActive(false);
    }

    public void ChangeMat(int i)
    {
        this.gameObject.GetComponent<MeshRenderer>().material = mat[i];
    }

    void ColliderEnable()
    {
        coll.enabled = true;
    }
}
