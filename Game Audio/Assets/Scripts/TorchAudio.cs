using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class TorchAudio : MonoBehaviour
{
    public string fireCracklingRef;

    private EventInstance torches;
    private GameObject player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        torches = RuntimeManager.CreateInstance(fireCracklingRef);
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public void OnTriggerEnter(Collider other)
    {
        float dist = Vector3.Distance(transform.position, player.transform.position);
        //Debug.Log("Torch Distance from Player:" + dist);
        dist /= 30;
        //Debug.Log("Torch Distance from Player:" + dist);
        torches.setParameterByName("TorchDistance", dist);
        torches.start();
    }

}
