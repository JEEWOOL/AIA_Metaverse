using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public string gunName;          // 총이름
    public float range;             // 사정거리
    public float accuracy;          // 정확도
    public float fireRate;          // 연사속도
    public float reloadTime;        // 재장선 속도

    public int damage;              // 총 데미지

    public int reloadBulletCount;   // 총알 재장선 갯수
    public int currentBulletCount;  // 현재 탄창에 남아있는 총알갯수
    public int maxBulletCount;      // 최대 소유 가능 총알 갯수
    public int carryBulletCount;    // 현재 소유중인 총알 갯수

    public float retroActionForce;  // 반동 강도    

    public Vector3 fineSightOriginPos;
    public Animator anim;
    public ParticleSystem muzzleFlash1;
    public ParticleSystem muzzleFlash2;

    public AudioClip fire_Sound;
}
