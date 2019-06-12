using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMgr : MonoBehaviour
{
    //몬스터 생성위치 배열
    private Transform[] points;
    public GameObject monsterPrefabs;

    //몬스터 발생 주기
    public float createTime = 2.0f;

    //몬스터 최대 생성 개수
    public int maxMonsterCount = 10;

    //게임 종료 여부
    public bool isGameOver = false;

    //인스턴스
    public static GameMgr instance = null;

    void Awake()
    {
        instance = this;
    }
    
    void Start()
    {
        points = GameObject.Find("SpawnPoints").transform.GetChild(0).GetComponentsInChildren<Transform>();

        if (points.Length > 0)
        {
            StartCoroutine(this.CreateMonster());
            Debug.Log(points.Length);
        }
    }

    IEnumerator CreateMonster()
    {
        while (isGameOver == false)
        {
            int monsterCount = (int)GameObject.FindGameObjectsWithTag("MONSTER").Length;

            if (monsterCount < maxMonsterCount)
            {
                yield return new WaitForSeconds(createTime);

                int randIndex = Random.Range(1, points.Length);
                Instantiate(monsterPrefabs, points[randIndex].position, points[randIndex].rotation);
            }
            else
                yield return null;
        }
    }

}
