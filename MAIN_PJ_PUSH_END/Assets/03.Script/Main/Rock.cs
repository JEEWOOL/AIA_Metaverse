using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour
{
    public int hp;                  // 바위체력
    public float destoryTime;       // 바위파편 소멸시간

    public SphereCollider col;      // 구체 콜라이더
    public GameObject go_rock;      // 일반바위
    public GameObject go_debris;    // 깨진바위
    public GameObject go_effect;    // 채굴 임펙트
    public GameObject go_rock_item; // 돌맹이 아이템

    public int count;              // 아이템 등장횟수

    public AudioSource audioSource;
    public AudioClip effect_Sound1;
    public AudioClip effect_Sound2;

    public void Mining()
    {
        audioSource.clip = effect_Sound1;
        audioSource.Play();
        var clone = Instantiate(go_effect, col.bounds.center, Quaternion.identity);
        Destroy(clone, destoryTime);

        hp--;
        if(hp <= 0)
        {
            Destruction();
        }
    }
    void Destruction()
    {
        audioSource.clip = effect_Sound2;
        audioSource.Play();

        col.enabled = false;

        for(int i = 0; i <= count; i++)
        {
            Instantiate(go_rock_item, go_rock.transform.position + new Vector3(0, 1, 0), Quaternion.identity);
        }        

        Destroy(go_rock);

        go_debris.SetActive(true);
        Destroy(go_debris, destoryTime);
    }
}
