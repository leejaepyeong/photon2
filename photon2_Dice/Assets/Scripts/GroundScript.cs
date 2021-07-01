using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class GroundScript : MonoBehaviourPun
{
    // 땅 분류 시작점 / 그냥 땅 / 황금키
    public enum GroundType {GROUND, START, GOLDKEY }
    public GroundType groundType;
    public int price, owner;    //  땅의 가격 , 주인넘버 (기본은 -1)

    PhotonView PV;
    TextMesh PriceText;
    GameObject[] Cubes; //  플레이어가 구매했는지 보이는 큐브

    int[] goldKeyMoneys = new int[6] { -20, -10, 0, 10, 20, 30 };


    private void Start()
    {
        PV = photonView;

        if (groundType == GroundType.GROUND)
        {
            PriceText = GetComponentInChildren<TextMesh>();
            Cubes = new GameObject[2] { transform.GetChild(0).gameObject, transform.GetChild(1).gameObject };

            
        }

        
    }

   


    public void TypeSwitch(PlayerScript curPlayer, PlayerScript otherPlayer)
    {
        // 그냥 땅
        if(groundType == GroundType.GROUND)
        {
            GroundOwner(curPlayer, otherPlayer);
            PV.RPC("AddPriceRPC", RpcTarget.AllViaServer);
        }
        // 골드키
        else if (groundType == GroundType.GOLDKEY)
        {
            int addmoney = goldKeyMoneys[Random.Range(0, goldKeyMoneys.Length)];
            curPlayer.money += addmoney;
            print(curPlayer.myNum + "이" + addmoney + "을 얻었다");
        }
    }

    void GroundOwner(PlayerScript curPlayer, PlayerScript otherPlayer)
    {
        int myNum = curPlayer.myNum;

        // 빈땅
        if(owner == -1)
        {
            curPlayer.money -= price;
            print(myNum + "이" + price + "을 잃었다");

            // 땅이 자기것이 
            owner = myNum;
            PV.RPC("CubeRPC",RpcTarget.AllViaServer, myNum);
        }

        // 다른 플레이어 
        else if(owner != myNum)
        {
            curPlayer.money -= price;
            otherPlayer.money += price;
            print(myNum + "이" + price + "을 잃었다");
            print(otherPlayer.myNum + "이" + price + "을 얻었다");
        }
    }

    [PunRPC]
    void CubeRPC(int myNum)
    {
        Cubes[myNum].SetActive(true);
    }

    [PunRPC]
    void AddPriceRPC()
    {
        price += 10;
        PriceText.text = price.ToString();
    }
}
