using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityRandom = UnityEngine.Random;

public class BarrelCtrl : MonoBehaviour
{
    public GameObject explosionEffect;
    public Texture[] barrelTextures;

    private Transform tr;
    private int hitCount = 0;
    private const int maxHitCount = 3;

    void Start()
    {
        tr = GetComponent<Transform>();
        var mesh_renderer = GetComponentInChildren<MeshRenderer>();
        mesh_renderer.material.mainTexture  = barrelTextures[UnityRandom.Range(0, barrelTextures.Length)];
    }

    void OnTriggerEnter(Collider coll)
    {
        if (coll.tag == "BULLET")
        {
            Destroy(coll);

            if (++hitCount >= maxHitCount)
            {
                ExplosionEffect(coll.transform.position);

               
            }
        }
    }

    private void ExplosionEffect(Vector3 explodedPos)
    {
        GameObject explosionEffectInstance = Instantiate(explosionEffect, explodedPos, Quaternion.identity);
        Destroy(explosionEffectInstance, 1.0f);

        Collider[] colls = Physics.OverlapSphere(tr.transform.position, 10.0f);

        for (int i = 0; i < colls.Length; i++)
        {
            Rigidbody colliderRigid = colls[i].GetComponent<Rigidbody>();
            if (colliderRigid != null)
            {
                colliderRigid.mass = 1.0f;
                colliderRigid.AddExplosionForce(1000.0f, tr.position, 10.0f, 300.0f);
                //폭발력, 원점, 반경, 위로 솟구치는 힘
            }
        }

        Destroy(gameObject, 5.0f);
    }

    void OnDamage(object[] _params)
    {
        Vector3 hitPos = (Vector3)_params[0];
        Vector3 firePos = (Vector3)_params[1];

        Vector3 incomeVector = (hitPos - firePos).normalized; //입사벡터의 정규화
        GetComponent<Rigidbody>().AddForceAtPosition(incomeVector * 1000.0f, hitPos);
        
        if (++hitCount >= maxHitCount)
        {
            ExplosionEffect((Vector3)_params[0]);
        }
    }

}
