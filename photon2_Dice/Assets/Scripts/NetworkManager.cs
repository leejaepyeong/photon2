using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    [Header("DisconnectPanel")]
    public GameObject DisconnectPanel;
    public InputField NicknameInput;

    [Header("RoomPanel")]
    public GameObject RoomPanel;
    public GameObject InitGameBtn, RollBtn;
    public Text[] NicknameTxts;
    public GameObject[] ArrowImages;

    [Header("Board")]
    public DiceScript diceScript;
    public Transform[] Pos;
    public PlayerScript[] Players;  // 각 플레이엄
    public Text[] MoneyTexts;   // 플레이어 소유금액
    public Text LogText;

    public int myNum, turn;
    const int Size = 2;
    PhotonView PV;


    private void Start()
    {
        //pc상태에서는 해당 해상도로 실행
#if(!UNITY_ANDROID)
        Screen.SetResolution(960, 540, false);
#endif

        PV = photonView;

    }

    public void Connect()
    {
        PhotonNetwork.LocalPlayer.NickName = NicknameInput.text;
        PhotonNetwork.ConnectUsingSettings();   // 서버에 접속
    }

    // 마스터 서버
    public override void OnConnectedToMaster()
    {
        //2명만 접속 가능
        PhotonNetwork.JoinOrCreateRoom("MyRoom",new RoomOptions { MaxPlayers = 2}, null);
        
    }


    void ShowPanel(GameObject CurPanel)
    {
        DisconnectPanel.SetActive(false);
        RoomPanel.SetActive(false);

        CurPanel.SetActive(true);
    }

    bool master()
    {
        return PhotonNetwork.LocalPlayer.IsMasterClient;
    }

    public override void OnJoinedRoom()
    {
        ShowPanel(RoomPanel);

        if(master())
        {
            InitGameBtn.SetActive(true);
        }
    }

    public void InitGame()
    {
        // 현재 방의 플레이어 수 Check
        if (PhotonNetwork.CurrentRoom.PlayerCount != 2) return;

        RollBtn.SetActive(true);
        InitGameBtn.SetActive(false);
        PV.RPC("InitGameRPC", RpcTarget.AllViaServer);
    }

    // 아이디 입력
    [PunRPC]
    void InitGameRPC()
    {
        for (int i = 0; i < 2; i++)
        {
            NicknameTxts[i].text = PhotonNetwork.PlayerList[i].NickName;

            // 각 네트워크의 매니저 구분 위해  로컬 플레이어한테 myNum 입력
            if (PhotonNetwork.PlayerList[i] == PhotonNetwork.LocalPlayer)
                myNum = i;
        }
    }


    // 주사위 던지기
    public void Roll()
    {
        PV.RPC("RollRPC", RpcTarget.MasterClient);
    }

    [PunRPC]
    void RollRPC()
    {
        StartCoroutine(RollCo());
    }

    [PunRPC]
    void EndRollRPC(int money0, int money1)
    {
        turn = turn == 0 ? 1 : 0;

        for (int i = 0; i < 2; i++)
        {
            ArrowImages[i].SetActive(i == turn);
        }

        RollBtn.SetActive(myNum == turn);

        MoneyTexts[0].text = money0.ToString();
        MoneyTexts[1].text = money1.ToString();

        if (money0 <= 0 || money1 >= 300) LogText.text = NicknameTxts[1].text + "이 승리하셨습니다";
        else if(money1 <= 0 || money0 >= 300) LogText.text = NicknameTxts[0].text + "이 승리하셨습니다";
    }

    IEnumerator RollCo()
    {

        //방장만 함수 호출
        // 해당 함수를 다 돌아야 이후로 넘어감
        yield return StartCoroutine(diceScript.Roll());
        yield return StartCoroutine(Players[turn].Move(diceScript.num));
        yield return new WaitForSeconds(0.2f);

        PV.RPC("EndRollRPC", RpcTarget.AllViaServer, Players[0].money, Players[1].money);

    }
}
