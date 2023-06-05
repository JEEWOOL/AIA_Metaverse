using FreeNet;
using JEEWOO.NET;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    public Gun currentGun;                  // 현재 장착된 총
    private AudioSource audioSource;        // 총기 발사 효과음
    private float currentFireRate;          // 연사 속도 계산
    private bool isReload = false;          // 재장전 상태확인 변수

    public Vector3 originPos;               // 원래 총 포지션
    private RaycastHit hitInfo;             // 레이케스트에 충돌한 정보를 받아옴
    public GameObject theCam;                  // 레이케스트를 카메라의 가운데 점으로 받기위해서 카메라 컴포넌트를 받아옴
    public GameObject hit_Effect;           // 피격이펙트
    private float gunAccuracy = 0.04f;      // 총발사시 튀는정도

    // 플레이어 수류탄 공격
    public GameObject firePos;
    public GameObject bombFactory;    
    public float throwPower = 25f;
    public int bombCount = 3;

    CPlayerInfo playerInfo;
    public HUD hud;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        originPos = Vector3.zero;
        playerInfo = GetComponent<CPlayerInfo>();
        CProcessPacket.Instance.OnDispatchFire += OnReceive;
    }

    private void Update()
    {
        if (playerInfo.IS_MINE)
        {
            GunFireRateCalc();
            TryFire();
            TryReload();
            Bomb();
        }
    }

    // 수류탄 공격
    void Bomb()
    {
        if (bombCount > 0)
        {            
            if (Input.GetMouseButtonDown(1))
            {
                bombCount--;
                GameObject bomb = Instantiate(bombFactory);
                bomb.transform.position = firePos.transform.position;

                Rigidbody rb = bomb.GetComponent<Rigidbody>();
                rb.AddForce(firePos.transform.forward * throwPower, ForceMode.Impulse);
            }
        }        
    }

    // 연사 속도 다시 계산
    void GunFireRateCalc()
    {
        if (currentFireRate > 0)
        {
            currentFireRate -= Time.deltaTime;
        }
    }
    // 발사 시도
    void TryFire()
    {
        if (Input.GetMouseButton(0) && currentFireRate <= 0 && !isReload)
        {
            Fire();
        }
    }
    // 발사 전 계산
    void Fire()
    {
        if (!isReload)
        {
            CPacket pack = CPacket.create((short)PROTOCOL.SHOT_PLAYER_REQ);
            pack.push(playerInfo.USER_ID);
            if (currentGun.currentBulletCount > 0)
            {
                pack.push_int16((short)1);
                Shoot(pack);
            }
            else
            {
                pack.push_int16((short)0);
                StartCoroutine(ReloadCoroutine());
            }
            CNetworkManager.Instance.send(pack);
        }
    }
    // 발사 후 계산
    void Shoot(CPacket pack)
    {
        currentGun.anim.SetTrigger("Attack");
        currentGun.currentBulletCount--;
        currentFireRate = currentGun.fireRate; // 연사속도 재계산
        PlaySE(currentGun.fire_Sound);
        currentGun.muzzleFlash1.Play();
        currentGun.muzzleFlash2.Play();
        Hit(pack);

        //StopAllCoroutines();
        // 총기 반동 코루틴
        //StartCoroutine(RetroActionCoroutine());        
    }
    void Shoot(Vector3 pos, Vector3 dir)
    {
        currentGun.anim.SetTrigger("Attack");
        currentGun.currentBulletCount--;
        currentFireRate = currentGun.fireRate; // 연사속도 재계산
        PlaySE(currentGun.fire_Sound);
        currentGun.muzzleFlash1.Play();
        currentGun.muzzleFlash2.Play();
        Hit(pos, dir);

        //StopAllCoroutines();
        // 총기 반동 코루틴
        //StartCoroutine(RetroActionCoroutine());        
    }

    // 총이 맞췄는지 확인
    private void Hit(CPacket pack)
    {
        Vector3 pos = theCam.transform.position;
        Vector3 dir = theCam.transform.forward + new Vector3(Random.Range(-gunAccuracy - currentGun.accuracy, gunAccuracy + currentGun.accuracy),
            Random.Range(-gunAccuracy - currentGun.accuracy, gunAccuracy + currentGun.accuracy), 0);
        pack.push(pos.x);
        pack.push(pos.y);
        pack.push(pos.z);
        pack.push(dir.x);
        pack.push(dir.y);
        pack.push(dir.z);
        if (Physics.Raycast(pos, dir , out hitInfo, currentGun.range))
        {
            StartCoroutine(Hit_Effect());
        }
    }
    private void Hit(Vector3 pos, Vector3 dir)
    {
        if (Physics.Raycast(pos, dir, out hitInfo, currentGun.range))
        {
            if(hitInfo.transform.gameObject.CompareTag("Player"))// == LayerMask.NameToLayer("Player"))
            {
                ShootingInfo info = hitInfo.transform.gameObject.GetComponent<ShootingInfo>();
                info.currHp -= currentGun.damage;
                if(info.currHp < 0)
                {
                    info.PlayerDie();
                }
            }

            StartCoroutine(Hit_Effect(hitInfo, dir));
        }
    }
    IEnumerator Hit_Effect(RaycastHit hitInfo, Vector3 dir)
    {
        var clone = Instantiate(hit_Effect, hitInfo.point, Quaternion.LookRotation(-dir.normalized));
        Destroy(clone, 2.0f);
        yield return new WaitForSeconds(0.2f);
        var clone1 = Instantiate(hit_Effect, hitInfo.point, Quaternion.LookRotation(-dir.normalized));
        Destroy(clone1, 2.0f);
    }
    IEnumerator Hit_Effect()
    {
        var clone = Instantiate(hit_Effect, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
        Destroy(clone, 2.0f);
        yield return new WaitForSeconds(0.2f);
        var clone1 = Instantiate(hit_Effect, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
        Destroy(clone1, 2.0f);
    }

    // 재장전 시도
    private void TryReload()
    {
        if (Input.GetKeyDown(KeyCode.R) && !isReload && currentGun.currentBulletCount < currentGun.reloadBulletCount)
        {
            StartCoroutine(ReloadCoroutine());
        }
    }
    // 재장전
    IEnumerator ReloadCoroutine()
    {
        if (currentGun.carryBulletCount > 0)
        {
            isReload = true;

            currentGun.anim.SetTrigger("ReRoad");

            currentGun.carryBulletCount += currentGun.currentBulletCount;
            currentGun.currentBulletCount = 0;

            yield return new WaitForSeconds(currentGun.reloadTime);

            if (currentGun.carryBulletCount >= currentGun.reloadBulletCount)
            {
                currentGun.currentBulletCount = currentGun.reloadBulletCount;
                currentGun.carryBulletCount -= currentGun.reloadBulletCount;
            }
            else
            {
                currentGun.currentBulletCount = currentGun.carryBulletCount;
                currentGun.carryBulletCount = 0;
            }

            isReload = false;
        }
        else
        {
            Debug.Log("총알이 없다!");
        }
    }

    //IEnumerator RetroActionCoroutine()
    //{
    //    Vector3 recoilBack = new Vector3(originPos.x, originPos.y, currentGun.retroActionForce);

    //    currentGun.transform.localPosition = originPos;

    //    // 총기반동
    //    while (currentGun.transform.localPosition.z <= currentGun.retroActionForce - 0.02f)
    //    {
    //        currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, recoilBack, 0.4f);
    //        yield return null;
    //    }

    //    while(currentGun.transform.localPosition != originPos)
    //    {
    //        currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, originPos, 0.1f);
    //        yield return null;
    //    }
    //}

    // 총기 발사 소리 재생
    void PlaySE(AudioClip _clip)
    {
        audioSource.clip = _clip;
        audioSource.Play();
    }

    void OnReceive(string user_id, short shot, Vector3 pos, Vector3 dir)
    {
        if (playerInfo.USER_ID == user_id)
        {
            switch (shot)
            {
                case 0:
                    StartCoroutine(ReloadCoroutine());
                    break;
                case 1:
                    Shoot(pos, dir);
                    break;
            }
        }
    }

    private void OnDestroy()
    {
        CProcessPacket.Instance.OnDispatchFire -= OnReceive;
    }
}
