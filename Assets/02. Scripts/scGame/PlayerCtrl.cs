using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Anims
{
    public AnimationClip idle;
    public AnimationClip runForward;
    public AnimationClip runBacward;
    public AnimationClip runRight;
    public AnimationClip runLeft;
}
public class PlayerCtrl : MonoBehaviour
{
    private float h = 0.0f;
    private float v = 0.0f;
    private const int maxHp = 100;

    
    public float moveSpeed = 10.0f;
    public float rotSpeed = 5.0f;
    public float gunDelay = 0.05f;
    public int Hp = 100;
    public Anims anims;
    public Transform firePos;
    public GameObject bullet;
    public AudioClip gunFireSound;
    public MeshRenderer muzzleFlash;
    public delegate void PlayerDieHandler();
    public static event PlayerDieHandler OnPlayerDie;
    public Image imgHpBar;


    private Transform tr;
    private Animation animationComponent;
    private AudioSource gunFireAS;
    private bool isFireOk = true;

    void Start()
    {
        tr = GetComponent<Transform>();
        animationComponent = GetComponentInChildren<Animation>();
        gunFireAS = GetComponentInChildren<AudioSource>();
    }

    void Update()
    {
        Debug.DrawRay(this.firePos.position, firePos.forward * 10.0f, Color.green);

        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");

        tr.Translate((Vector3.forward * v + Vector3.right * h).normalized * moveSpeed * Time.deltaTime, Space.Self);
        tr.Rotate(Vector3.up * Time.deltaTime * rotSpeed * Input.GetAxis("Mouse X"));

        if (h >= 0.3f)
            animationComponent.CrossFade(anims.runRight.name, 0.3f);
        else if (h <= -0.3f)
            animationComponent.CrossFade(anims.runLeft.name, 0.3f);
        else if (v >= 0.3f)
            animationComponent.CrossFade(anims.runForward.name, 0.3f);
        else if (v <= -0.3f)
            animationComponent.CrossFade(anims.runBacward.name, 0.3f);
        else
            animationComponent.CrossFade(anims.idle.name, 0.3f);

        if (isFireOk == true && Input.GetButton("Fire1"))
        {
            //Instantiate(bullet, firePos.position, firePos.rotation); //이제 레이캐스팅을 사용하므로 안씀
            //gunFireAS.PlayOneShot(gunFireSound, 0.9f); //이제 단독으로 안씀 SoundMgr로 효과음 발생
            SoundMgr.instance.PlaySfx(firePos.position, gunFireSound);

           

            if (Physics.Raycast(firePos.position, firePos.forward, out RaycastHit hit, 10.0f))
            {
                switch (hit.collider.tag)
                {
                    case "MONSTER":
                        {
                            object[] _params = new object[2];
                            _params[0] = hit.point; //맞은 위치
                            _params[1] = 20;         //통에 입힐 데미지 값;

                            hit.collider.gameObject.SendMessage("OnDamage", _params, SendMessageOptions.DontRequireReceiver);
                        }
                        break;
                    case "BARREL":
                        {
                            object[] _params = new object[2];
                            _params[0] = hit.point; //맞은 위치
                            _params[1] = firePos.position ;         //레이의 입사각 계산을 위해 맞은점과 발사점 전달
                            hit.collider.gameObject.SendMessage("OnDamage", _params, SendMessageOptions.DontRequireReceiver);

                            
                        }
                        break;
                    default:
                        break;
                }
            }

            StartCoroutine(ShowMuzzleFlash());
            StartCoroutine(FireWaits());
            isFireOk = false;
        }
    }

    private IEnumerator FireWaits()
    {
        yield return new WaitForSeconds(0.1f);

        isFireOk = true;
    }

    private IEnumerator ShowMuzzleFlash()
    {
        muzzleFlash.gameObject.SetActive(true);
        muzzleFlash.transform.localScale = Vector3.one * Random.Range(1.0f, 2.0f);
        muzzleFlash.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));

        yield return new WaitForSeconds(UnityEngine.Random.Range(0.05f, 0.1f));
        muzzleFlash.gameObject.SetActive(false);
    }

    void OnTriggerEnter(Collider coll)
    {
        if (coll.tag == "MONSTER")
        {
            Hp -= 10;
            imgHpBar.fillAmount = (float)Hp / (float)maxHp;
            if (Hp <= 0)
            {
                /*
                 * SendMessage 방식으로 몬스터에게 생존자가 죽엇음을 알림
                GameObject[] monsterObjects = GameObject.FindGameObjectsWithTag("MONSTER");

                foreach (var mob in monsterObjects)
                {
                    mob.SendMessage("OnPlayerDie", SendMessageOptions.DontRequireReceiver);

                    //SendMessageOptions.DontRequireReceiver : 해당함수가 없더라도 메세지를 리턴 받지 않겠다.
                }
                */

                //이벤트 방식으로 알림
                
                OnPlayerDie();
                GameMgr.instance.isGameOver = true;
            }
        }
    }
}
