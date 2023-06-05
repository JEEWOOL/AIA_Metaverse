using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour
{
    public int hp;
    public float destroyTime;

    public CapsuleCollider col;
    public GameObject go_tree;
    public GameObject go_broken_tree;
    public GameObject go_effect;
    public GameObject go_tree_item;

    public int count;

    public AudioSource audioSource;
    public AudioClip effect_Sound1;
    public AudioClip effect_Sound2;

    public void Mining()
    {
        audioSource.clip = effect_Sound1;
        audioSource.Play();

        var clone = Instantiate(go_effect, col.bounds.center, Quaternion.identity);
        Destroy(clone, destroyTime);

        hp--;
        if (hp <= 0)
        {
            Destruction();
        }
    }

    void Destruction()
    {
        audioSource.clip = effect_Sound2;
        audioSource.Play();

        col.enabled = false;

        for (int i = 0; i <= count; i++)
        {
            Instantiate(go_tree_item, go_tree.transform.position + new Vector3(0, 1, 0), Quaternion.identity);
        }

        Destroy(go_tree);        
    }
}
