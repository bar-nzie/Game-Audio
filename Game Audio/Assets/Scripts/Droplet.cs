using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using Unity.VisualScripting;

public class Droplet : MonoBehaviour
{
    public string dropletRef;

    private EventInstance droplet;
    private GameObject player;

    void Start()
    {
        droplet = RuntimeManager.CreateInstance(dropletRef);
        player = GameObject.FindGameObjectWithTag("Player");
    }

     private void OnTriggerEnter(Collider other)
    {
        float dist = Vector3.Distance(transform.position, player.transform.position);
        //Debug.Log(dist);
        dist /= 30;
        //Debug.Log(dist);
        droplet.setParameterByName("Distance", dist);
        droplet.start();
        Destroy(gameObject);
    }


}
