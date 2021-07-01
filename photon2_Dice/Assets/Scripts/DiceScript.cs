using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceScript : MonoBehaviour
{
    public Rigidbody RB;
    public Transform[] Nums;    //주사위 면에 있는 오브젝트
    public int num; // 주사위 숫자

    public IEnumerator Roll()
    {
        yield return null;

        // 위에서 던지기 4높이
        // 회전값 랜덤
        // 주사위 회전 속력 랜덤으로
        transform.position = new Vector3(0, 4, 0);
        transform.localEulerAngles = new Vector3(Random.Range(-90f, 90f), Random.Range(-90f, 90f), Random.Range(-90f, 90f));
        RB.angularVelocity = Random.insideUnitSphere * Random.Range(-1000, 1000);

        // 주사위가 기울어져 있는것 대기 시간
        yield return new WaitForSeconds(3);

        // 속력이 0에 가까우면 스톱
        while(true)
        {
            yield return null;
            if (RB.velocity.sqrMagnitude < 0.001f) break;
        }

        for (int i = 0; i < Nums.Length; i++)
        {

            if(Nums[i].position.y > 2)
            {
                num = i + 1;
                break;
            }
        }
    }
}
