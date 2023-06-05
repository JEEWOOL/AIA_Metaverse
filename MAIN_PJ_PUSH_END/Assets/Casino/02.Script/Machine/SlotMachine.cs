using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FreeNet;
using JEEWOO.NET;

public class SlotMachine : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField inputBetAmount;      // 배팅 금액 필드
    [SerializeField]
    private Image imageBetAmount;               // 배팅 금액 필드 (색상 변경용)
    [SerializeField]
    private TextMeshProUGUI textCredits;        // 플레이어 소지 금액
    [SerializeField]
    private TextMeshProUGUI textFirstReel;      // 첫 번째 릴 숫자
    [SerializeField]
    private TextMeshProUGUI textSecondReel;     // 두 번째 릴 숫자
    [SerializeField]
    private TextMeshProUGUI textThirdReel;      // 세 번째 릴 숫자
    [SerializeField]
    private TextMeshProUGUI textResult;         // 실행 결과

    private float spinDuration = 0.2f;          // 릴 굴리기 지속 시간 (0.2초);
    private float elapsedTime = 0;              // 숫자 선택 지연시간 (릴이 실제 돌아가는 것처럼)
    private bool isStartSpin = false;           // 이 값이 true이면 릴 굴리기 시작
    

    //static public int credits = 100;                // 플레이어 소지 금액


    // 릴의 상태(false : 릴을 굴리는 중)
    private bool isFirstReelSpinned = false;
    private bool isSecondtReelSpinned = false;
    private bool isThirdReelSpinned = false;
    
    // 릴의 결과 값(0 or 1)
    private int firstReelResult = 0;
    private int secondReelResult = 0;
    private int thirdReelResult = 0;

    public GameObject crossHair;
    //GameObject slotMachineUI;   // 슬롯머신 UI
    private bool onTriggerStay = false;

    private void Awake()
    {
        
    }

    private void Update()
    {
        if (onTriggerStay)
        {
            if (Input.GetKeyDown("e"))
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                textCredits.text = $"Credits : {PlayListManager.gold}";
                GameManager.slotMachineUI.SetActive(true);
                crossHair.SetActive(false);
            }
            if (Input.GetKey("escape"))
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                GameManager.slotMachineUI.SetActive(false);
                crossHair.SetActive(true);
            }
        }

        if ( !isStartSpin ) return;

        elapsedTime += Time.deltaTime;
        // 릴 안에 들어가는 0, 1 중 랜덤 값
        int random_spinResult = Random.Range(0,2);
        
        if( !isFirstReelSpinned )
        {
            firstReelResult = random_spinResult;
            if( elapsedTime >= spinDuration)
            {
                isFirstReelSpinned = true;
                elapsedTime = 0;
            }
        }
        else if( !isSecondtReelSpinned )
        {
            secondReelResult = random_spinResult;
            if( elapsedTime >= spinDuration)
            {
                isSecondtReelSpinned = true;
                elapsedTime = 0;
            }
        }
        else if( !isThirdReelSpinned )
        {
            thirdReelResult = random_spinResult;
            if( elapsedTime >= spinDuration)
            {
                isStartSpin = false;
                elapsedTime = 0;
                isFirstReelSpinned = false;
                isSecondtReelSpinned = false;
                isThirdReelSpinned = false;

                // 숫자 비교 후 승리/패배 여부 설정
                CheckBet();
            }
        }
        
        // 0 -> O, 1 -> X 
        if(firstReelResult == 0)
            textFirstReel.text = "O";
        else
            textFirstReel.text = "X";

        if(secondReelResult == 0)
            textSecondReel.text = "O";
        else
            textSecondReel.text = "X";

        if(thirdReelResult == 0)
            textThirdReel.text = "O";
        else
            textThirdReel.text = "X";

        /*
        textFirstReel.text  = firstReelResult.ToString("D1");
        textSecondReel.text  = secondReelResult.ToString("D1");
        textThirdReel.text  = thirdReelResult.ToString("D1");
        */

        
    }

    // 레버 버튼 눌렀을 때
    public void OnClickPull()
    {
        // 필드의 색상과 입력 정보가 바뀌어 있을 수도 있으니 초기화
        OnMessage(Color.white, string.Empty);

        // 필드에 값을 입력하지 않았을 시 에러
        if(inputBetAmount.text.Trim().Equals(""))
        {
            OnMessage(Color.red, "Please Fill Bet Amount");
            return;
        }
        // 문자열 데이터를 숫자로 변경해 parse 변수에 저장
        int parse = int.Parse(inputBetAmount.text);
        
        // 소지금 - 배팅금액이 0 이상이면
        if(PlayListManager.gold - parse >= 0)
        {
            CPacket pack = CPacket.create((short)PROTOCOL.UPDATE_MONEY_REQ);
            pack.push(CProcessPacket.Instance.USER_ID);
            pack.push((short)0);
            pack.push(parse);
            CNetworkManager.Instance.send(pack);

            // 배팅 금액 차감하고
            PlayListManager.gold -= parse;
            textCredits.text = $"Credits : {PlayListManager.gold}";

            // 릴 굴리기
            isStartSpin = true;
        }
        else
        {
            OnMessage(Color.red, "Not enough money");
        }

    }

    private void CheckBet()
    {
        // 세 개의 숫자가 같으면(같은 문양이면)
        if(firstReelResult == secondReelResult && secondReelResult == thirdReelResult)
        {
            // 배팅 금액의 10배를 총 credit에 추가
            int result = int.Parse(inputBetAmount.text) * 10;

            CPacket pack = CPacket.create((short)PROTOCOL.UPDATE_MONEY_REQ);
            pack.push(CProcessPacket.Instance.USER_ID);
            pack.push((short)1);
            pack.push(result);
            CNetworkManager.Instance.send(pack);
            PlayListManager.gold += result;

            textCredits.text = $"Credits : {PlayListManager.gold}";

            textResult.text = "YOU WIN!";
        }
        else
        {
            textResult.text = "YOU LOSE!";
        }
    }

    private void OnMessage(Color color, string msg)
    {
        imageBetAmount.color = color;
        textResult.text = msg;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            onTriggerStay = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            onTriggerStay = false;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            GameManager.slotMachineUI.SetActive(false);
            crossHair.SetActive(true);
        }
    }
}
