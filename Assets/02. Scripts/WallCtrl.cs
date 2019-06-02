using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCtrl : MonoBehaviour
{
    public GameObject particle;

    void Start()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "BULLET")
        {
            GameObject sparkInstance = Instantiate(particle, other.transform);
            Destroy(sparkInstance, particle.GetComponent<ParticleSystem>().main.duration + 0.2f);
            Destroy(other.gameObject);
        }
    }

}
