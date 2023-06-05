using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class FlowerSpawn : MonoBehaviour
{
    public GameObject flowerPrefab; // 생성할 게임오브젝트 프리팹

    private Transform[] cubeTransforms; // 16개 큐브의 Transform 배열


    private void Start()
    {
        // 자식 오브젝트들의 Transform을 배열에 저장
        cubeTransforms = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            cubeTransforms[i] = transform.GetChild(i);
        }
    }

    public void RandomObjects()
    {
        int flowerCount = 0;
        // 각 큐브에 대해 반복
        foreach (Transform cubeTransform in cubeTransforms)
        {
            // 이미 생성된 프리팹이 있는지 확인
            if (cubeTransform.childCount == 0)
            {
                // 랜덤한 확률로 생성 여부 결정
                float randomValue = Random.value;

                if (randomValue <= 0.5f)
                {
                    // 1부터 5개의 큐브에서 하나의 프리팹 생성
                    if (cubeTransform.GetSiblingIndex() < 5)
                    {
                        CreatePrefab(cubeTransform);

                    }
                }
                else if (randomValue <= 0.75f)
                {
                    // 6부터 10개의 큐브에서 하나의 프리팹 생성
                    if (cubeTransform.GetSiblingIndex() < 10)
                    {
                        CreatePrefab(cubeTransform);
                    }
                }
                else if (randomValue <= 0.9f)
                {
                    // 11부터 14개의 큐브에서 하나의 프리팹 생성
                    if (cubeTransform.GetSiblingIndex() < 14)
                    {
                        CreatePrefab(cubeTransform);
                    }
                }
                else
                {
                    // 15부터 16개의 큐브에서 하나의 프리팹 생성
                    if (cubeTransform.GetSiblingIndex() >= 14)
                    {
                        CreatePrefab(cubeTransform);
                    }
                }
            }
            flowerCount += cubeTransform.childCount;
        }

        // 생성된 꽃 개수
        Debug.Log($"flower count : {flowerCount}");

        // 나온 개수당 보상 반영(수정 필요)
        if (flowerCount < 8) // 절반 미만일 때
            PlayListManager.gold -= flowerCount;
        else if (flowerCount > 8)
            PlayListManager.gold += (flowerCount - 8);
        Debug.Log($"credits = {PlayListManager.gold}");
    }

    private void CreatePrefab(Transform cubeTransform)
    {
        // 큐브 위에 하나의 동일한 프리팹을 생성
        GameObject newObject = Instantiate(flowerPrefab, cubeTransform.position + new Vector3(0f, 1f, 0f), Quaternion.identity);

        // 생성된 게임 오브젝트를 FlowerSpawn의 자식으로 설정
        newObject.transform.SetParent(cubeTransform);

        Destroy(newObject, 5f);
    }
}