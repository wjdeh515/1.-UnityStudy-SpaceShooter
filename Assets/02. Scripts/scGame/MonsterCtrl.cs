using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityRandom = UnityEngine.Random;



public class MonsterCtrl : MonoBehaviour
{
    public enum MonsterState
    {
        Idle,
        Walk,
        Attack,
        Hit,
        Die
    }
    //추적 사정거리 이거리 사이에 플레이어가 들어오면 추적을 시작함
    public float traceDist = 10.0f;

    //공격 사정거리 이거리 사이에 플레이어가 들어오면 공격을 함
    public float attackDist = 2.0f;

    //몬스터 Hp
    public int monsterHp = 20;
    public const int maxMonsterHp = 20;

    public MonsterState monsterState = MonsterState.Idle;

    public Transform playerTr;
    public GameObject bloodEffect;
    public GameObject bloodDecal;

    private GameUICtrl gameUiManager;
    private Transform monsterTr;
    private NavMeshAgent nvAgent;
    private Animator animator;
    private bool isDie = false;

    void Awake()
    {
        monsterTr = GetComponent<Transform>();
        playerTr = GameObject.FindWithTag("Player").GetComponent<Transform>();
        gameUiManager = GameObject.Find("GameUI").GetComponent<GameUICtrl>();
        nvAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// OnEnable 함수는 객체가 활성화 될 때마다 호출된다.
    /// 몬스터 오브젝트 풀에서 꺼낸후 SetActive(true)로 주기때문에바로 아래 함수 실행가능
    /// </summary>
    void OnEnable()
    {
        PlayerCtrl.OnPlayerDie += OnPlayerDie;

        StartCoroutine(CheckMonsterState());
        StartCoroutine(ExecuteMonsterAction());
    }

    void OnDisable()
    {
        PlayerCtrl.OnPlayerDie -= OnPlayerDie;
    }

    void Start()
    {

    }

    

    private IEnumerator ExecuteMonsterAction()
    {
        while (isDie == false)
        {
            yield return new WaitForSeconds(0.2f);

            float dist = Vector3.Distance(playerTr.position, monsterTr.position);

            if (dist <= attackDist)
                monsterState = MonsterState.Attack;
            else if (dist <= traceDist)
                monsterState = MonsterState.Walk;
            else
                monsterState = MonsterState.Idle;
        }

    }

    private IEnumerator CheckMonsterState()
    {
        while (isDie == false)
        {
            switch (monsterState)
            {
                case MonsterState.Idle:
                    animator.SetBool("isTrace", false);
                    nvAgent.isStopped = true;
                    break;
                case MonsterState.Walk:
                    nvAgent.destination = playerTr.position;
                    nvAgent.isStopped = false;

                    animator.SetBool("isTrace", true);
                    animator.SetBool("isAttack", false);
                    
                    break;
                case MonsterState.Attack:
                    nvAgent.isStopped = true;
                    animator.SetBool("isAttack", true);
                    break;
            }

            yield return null;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "BULLET")
        {
            //총알 제거
            Destroy(other.gameObject);
            ShowHitEffect(other.transform.position);
            //총알 데미지 만큼 hp 차감
            monsterHp -= other.GetComponent<BulletCtrl>().bulletDamage;

            //몬스터 사망처리
            if (monsterHp <= 0)
            {
                MonsterDie();
            }
        }
    }

    private void ShowHitEffect(Vector3 hitPosition)
    {
        //goHit 애니메이션 실행 트리거인 isHit 트리거 작동
        animator.SetTrigger("isHit");

        //총알을 맞아 몬스터 맞은 부위에 혈흔 생성
        GameObject bloodEffectInstance = Instantiate(bloodEffect, hitPosition, Quaternion.identity);
        Destroy(bloodEffectInstance, 2.0f);


        //총알을 맞은 몬스터 발바닥아래 데칼 혈흔 생성
        Vector3 bloodDecalPos = hitPosition;
        bloodDecalPos.y = 0.05f;

        Quaternion decalRot = Quaternion.Euler(90.0f, 0.0f, UnityRandom.Range(0, 360));
        GameObject bloodDecalInstance = Instantiate(bloodDecal, bloodDecalPos, decalRot);
        bloodDecalInstance.transform.localScale = Vector3.one * UnityRandom.Range(1.5f, 3.5f);

        Destroy(bloodDecalInstance, 5.0f);
    }

    private void MonsterDie()
    {
        //사망 후 태그를 변경 하여 생성시 죽은 몬스터는 제외하고 생성가능
        this.tag = "Untagged";

        StopAllCoroutines();

        isDie = true;
        monsterState = MonsterState.Die;
        nvAgent.isStopped = true;
        animator.SetTrigger("isMonsterDie");


        //콜라이더를 비활성화 함으로써 죽고난 뒤에 총알이 더이상 충돌하지 않도록 함
        GetComponentInChildren<CapsuleCollider>().enabled = false;
        foreach (Collider coll in GetComponentsInChildren<SphereCollider>())
            coll.enabled = false;

        //플레이어 점수 획득
        gameUiManager.PlusScore(50);

        //몬스터 풀에 객체 환원
        StartCoroutine(this.ReduceMonsterObject());
    }

    /// <summary>
    /// 몬스터를 GameMgr의 몬스터 오브젝트풀에 다시 환원 // 재삽입의 개념이 아닌 값을 초기상태로 만들어주는걸 의미함
    /// </summary>
    private IEnumerator ReduceMonsterObject()
    {
        yield return new WaitForSeconds(5.0f);

        monsterState = MonsterState.Idle;
        gameObject.SetActive(false);
        tag = "MONSTER";
        monsterHp = maxMonsterHp;
        isDie = false;

        foreach (Collider col in GetComponentsInChildren<SphereCollider>())
            col.enabled = true;

        GetComponentInChildren<CapsuleCollider>().enabled = true;
    }

    /// <summary>
    /// Player가 죽을시 PlayerCtrl 클래스에서 SendMessage을 사용하여 호출하는  함수
    /// </summary>
    void OnPlayerDie()
    {
        StopAllCoroutines();
        nvAgent.isStopped = true;
        animator.SetTrigger("isPlayerDie");
    }

    void OnDamage(object[] _params)
    {
        ShowHitEffect((Vector3)_params[0]);

        monsterHp -= (int)_params[1];

        //몬스터 사망처리
        if (monsterHp <= 0)
        {
            MonsterDie();
        }
    }
}
