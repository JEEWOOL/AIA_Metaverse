//using System;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor.PackageManager;
using UnityEngine;

public class MineTrap : MonoBehaviour
{
    public GameObject trapPrefab; // 함정 큐브 프리팹
    public Material trapMaterial; // 함정 큐브 머티리얼
    public Material safeMaterial; // 안전 머티리얼
    public Material normalMaterial; // 초기화 머티리얼

    private Transform[] cubeTransforms; // 16개의 큐브 배열 
    private Transform trapTransform; // 함정 큐브의 Transform
    private Transform safeTransform; // 클릭된 안전한 큐브의 Transform
    private bool isGameActive = true; // 게임 상태

    private GameManager gameManager; // GameManager 참조
    public Camera maincam;
    private int safecount = 0; // 안전큐브 클릭횟수

    private void Start()
    {
        gameManager = GameObject.FindObjectOfType<GameManager>(); // 게임매니저 참조
        // 16개의 큐브 배열을 초기화
        cubeTransforms = new Transform[16];

        // 자식 큐브들의 Transform을 배열에 저장
        for (int i = 0; i < transform.childCount; i++)
        {
            cubeTransforms[i] = transform.GetChild(i);
            
        }

        // 랜덤으로 하나의 큐브를 함정 큐브로 설정
        int trapIndex = Random.Range(0, 16);
        trapTransform = cubeTransforms[trapIndex];
    }

    private void Update()
    {
        if (isGameActive && Input.GetMouseButtonDown(0))
        {
            int layerMask = 1 << LayerMask.NameToLayer("Player");    // LayerMask
            layerMask = ~layerMask;    // 플레이어 레이캐스트 무시하기위해서

            RaycastHit hit;

            // Mathf.Infinity - Ray거리를 무제한으로 두었기때문에 후에 수정
            bool ray = Physics.Raycast(maincam.transform.position, maincam.transform.forward, out hit, Mathf.Infinity, layerMask);
                        
            if (ray)
            {
                GameObject clickedObject = hit.collider.gameObject;
                OnCubeClick(clickedObject);
            }
        }
    }

    public void OnCubeClick(GameObject cube)
    {
        maincam = gameManager.maincam.GetComponent<Camera>(); // 게임매니저에 할당된 maincam가져오기

        if (System.Array.IndexOf(cubeTransforms, cube.transform) != -1)
        {
            if (cube.transform == trapTransform)
            {
                // 함정 큐브를 클릭한 경우
                cube.GetComponent<Renderer>().material = trapMaterial; // 머티리얼 변경
                StartCoroutine(ResetGameAfterDelay(5f));
                Debug.Log($"최대기록 -{safecount}");
            }
            else
            {
                // 일반 큐브를 클릭한 경우
                cube.GetComponent<Renderer>().material = safeMaterial; // 머티리얼 변경                
                
                // 클릭된큐브(안전한큐브)가 아닐경우 카운트증가
                if (cube.transform != safeTransform)
                {
                    safecount++;
                }
                // 클릭된 큐브(안전한 큐브)로 바꿈
                safeTransform = cube.transform;
                Debug.Log($"safecount -{safecount}");                
            }
        }
    }

    private IEnumerator ResetGameAfterDelay(float delay)
    {
        isGameActive = false; // 게임 상태를 비활성화

        yield return new WaitForSeconds(delay);

        // 게임 재시작
        // 함정 큐브를 클릭했을 때
        for (int i = 0; i < cubeTransforms.Length; i++)
        {
            cubeTransforms[i].GetComponent<Renderer>().material = normalMaterial;

        }
        // 게임 상태를 다시 활성화
        // 랜덤으로 하나의 큐브를 함정 큐브로 설정
        int trapIndex = Random.Range(0, 16);
        trapTransform = cubeTransforms[trapIndex];
        safecount = 0; // 횟수 기록 초기화
        isGameActive = true;
    }
}
