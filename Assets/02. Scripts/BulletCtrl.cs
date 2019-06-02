using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCtrl : MonoBehaviour
{
    public int bulletDamage = 1;
    public float bulletSpeed = 1000.0f;

    private Transform tr;
    private Rigidbody rigid;

    

    void Start()
    {
        tr = GetComponent<Transform>();
        rigid = GetComponent<Rigidbody>();
        rigid.AddForce(tr.forward * bulletSpeed);
    }
}
