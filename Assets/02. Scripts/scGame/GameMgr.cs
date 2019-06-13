using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMgr : MonoBehaviour
{
    //몬스터 생성위치 배열
    private Transform[] points;
    public GameObject monsterPrefab;

    //몬스터 풀
    private List<GameObject> monsterPool = new List<GameObject>();

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
        }

        //몬스터 풀에 저장
        for (int i =0;i < maxMonsterCount; i++)
        {
            GameObject monsterIns = Instantiate(monsterPrefab);
            monsterIns.SetActive(false); //Active false로 두어서 실제 게임상에선 보이지 않도록한다.
            monsterIns.name = "monster_" + i;
            monsterPool.Add(monsterIns);
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


                //비활성 오브젝트 찾기
                GameObject inactiveMonsterObject = monsterPool.Find(x => x.activeSelf == false);

                if(inactiveMonsterObject != null)
                {
                    inactiveMonsterObject.SetActive(true);
                    inactiveMonsterObject.transform.position = points[randIndex].position;
                    inactiveMonsterObject.transform.rotation = points[randIndex].rotation;
                }
            }
            else
                yield return null;
        }
    }

}
