using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public int curPos, myNum, money;
    public NetworkManager NM;
    public PlayerScript otherPlayer;

    public IEnumerator Move(int diceNum)
    {
        yield return null;

        int[] movePos = new int[diceNum];
        bool isZero = false;    // 시작점 들어왔는지 여부

        // 갈 길 배열 만들기
        for (int i = 0; i < movePos.Length; i++)
        {
            int plusNum = curPos + i + 1;
            if(plusNum > 31)
            {
                isZero = true;
                plusNum -= 32;
            }
            movePos[i] = plusNum;
        }

        //부드럽게 이동
        for (int i = 0; i < movePos.Length; i++)
        {
            Vector3 targetPos = NM.Pos[movePos[i]].position;    //가야할 방향

            while(true)
            {
                yield return null;
                transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime * 7);
                if (transform.position == targetPos) break;
            }
        }

        if (isZero)
        {
            money += 30;
            print(myNum + "이 30을 벌었습니다.");
        }

        curPos = movePos[movePos.Length - 1];
        NM.Pos[curPos].GetComponent<GroundScript>().TypeSwitch(this, otherPlayer);
            
    }
}
