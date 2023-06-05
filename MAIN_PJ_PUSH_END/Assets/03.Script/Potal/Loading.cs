using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using JetBrains.Annotations;
using UnityEngine.Video;

public class Loading : MonoBehaviour
{
    static string nextScene;

    public Slider progressbar;
    public Text loadtext;

    public VideoPlayer videoP;
    public VideoClip[] GameLoadingVideo;

    public AudioSource audioSource;
    public AudioClip[] audioClip;

    private void Start()
    {
        switch (nextScene)
        {
            case "FallGuys_ProtoType": // 아이스브레이크
                videoP.clip = GameLoadingVideo[0];
                audioSource.clip = audioClip[0];
                audioSource.Play();
                break;

            case "Shooting_ProtoType": // 슈팅게임
                videoP.clip = GameLoadingVideo[1];
                audioSource.clip = audioClip[1];
                audioSource.Play();
                break;

            case "Skating_ProtoType": // 아이스스케이팅
                videoP.clip = GameLoadingVideo[2];
                audioSource.clip = audioClip[2];
                audioSource.Play();
                break;

            case "Casino_ProtoType": // 카지노
                videoP.clip = GameLoadingVideo[3];
                audioSource.clip = audioClip[3];
                audioSource.Play();
                break;

            //case "Skating_ProtoType": // 미술관
            //    videoP.clip = GameLoadingVideo[0];
            //    audioSource.clip = audioClip[4];
            //    audioSource.Play();
            //    break;

            case "MyRoom_ProtoType": // 마이룸
                videoP.clip = GameLoadingVideo[5];
                audioSource.clip = audioClip[5];
                audioSource.Play();
                break;

            default:
                videoP.clip = GameLoadingVideo[6];
                audioSource.clip = audioClip[6];
                audioSource.Play();
                break;

        }

        StartCoroutine(LoadScene());
    }

    public static void LoadScene(string sceneName)
    {
        nextScene = sceneName;
        SceneManager.LoadScene("Loading");
    }

    IEnumerator LoadScene()
    {
        yield return null;

        AsyncOperation operation = SceneManager.LoadSceneAsync(nextScene);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            yield return null;

            if (progressbar.value < 1f)
            {
                progressbar.value = Mathf.MoveTowards(progressbar.value, 1f, Time.deltaTime);
            }
            else
            {
                loadtext.text = "Press SpaceBar";
            }
            if (Input.GetKeyDown(KeyCode.Space) && progressbar.value >= 1f && operation.progress >= 0.9f)
            {
                operation.allowSceneActivation = true;
            }
        }
    }
}
