using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoginManager : MonoBehaviour
{
    public InputField id;
    public InputField password;
    public Text notify;

    private void Start()
    {
        notify.text = "";
        id.Select();
    }

    private void Update()
    {
        if (id.isFocused == true)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                password.Select();
            }
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            CheckUserData();
        }
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            CheckUserData();
        }
    }

    public void SaveUserData()
    {
        if (!CheckInput(id.text, password.text))
        {
            return;
        }

        if (!PlayerPrefs.HasKey(id.text))
        {
            PlayerPrefs.SetString(id.text, password.text);
            notify.text = "아이디 생성이 완료되었습니다.";
        }
        else
        {
            notify.text = "이미 존재하는 아이디입니다.";
        }
    }

    public void CheckUserData()
    {
        if (!CheckInput(id.text, password.text))
        {
            return;
        }

        string pass = PlayerPrefs.GetString(id.text);

        if (password.text == pass)
        {
            Loading.LoadScene("Main_Protype");
            //SceneManager.LoadScene(2);
        }
        else
        {
            notify.text = "아이디 / 비밀번호가 일치하지 않습니다.";
        }
    }

    bool CheckInput(string id, string pwd)
    {
        if (id == "" || pwd == "")
        {
            notify.text = "아이디 / 비밀번호를 확인해주세요.";
            return false;
        }
        else
        {
            return true;
        }
    }
}
