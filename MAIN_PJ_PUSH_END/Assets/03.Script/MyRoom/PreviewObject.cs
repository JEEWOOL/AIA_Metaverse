using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PreviewObject : MonoBehaviour
{
    private List<Collider> colliderList = new List<Collider>();

    public int layerGround;
    private const int IGNORE_RAYCAST_LAYER = 2;

    public Material green;
    public Material red;

    private void Update()
    {
        ChangeColor();
    }

    void ChangeColor()
    {
        if (colliderList.Count > 0)
        {            
            SetColor(red);
        }
        else
        {            
            SetColor(green);
        }
    }
    void SetColor(Material mat)
    {
        //foreach (Transform obj in this.transform)
        //{
        //    var newMaterials = new Material[obj.GetComponent<Renderer>().materials.Length];

        //    for (int i = 0; i < newMaterials.Length; i++)
        //    {
        //        newMaterials[i] = mat;
        //    }

        //    obj.GetComponent<Renderer>().materials = newMaterials;
        //}
        foreach(Transform obj in this.GetComponentsInChildren<Transform>())
        {
            Renderer rend = obj.GetComponent<Renderer>();
            if (rend == null)
                continue;
            Material[] newMat = new Material[rend.materials.Length];

            for (int i = 0; i < newMat.Length; i++)
            {
                newMat[i] = mat;
            }
            rend.GetComponent<Renderer>().materials = newMat;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer != layerGround && other.gameObject.layer != IGNORE_RAYCAST_LAYER)
        {
            colliderList.Add(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer != layerGround && other.gameObject.layer != IGNORE_RAYCAST_LAYER)
        {
            colliderList.Remove(other);
        }
    }

    public bool isBuildable()
    {
        return colliderList.Count == 0;
    }
}