using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbSpawner : MonoBehaviour
{
    public GameObject Orb;
    // Start is called before the first frame update
    void Start()
    {
        Instantiate(Orb, transform.position, Quaternion.identity);
        Destroy(gameObject, 3f);
    }

    
}
