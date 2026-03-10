using UnityEngine;
using FMODUnity;

public class FogGate : MonoBehaviour
{
    public string fogGateEvent;

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        RuntimeManager.PlayOneShot(fogGateEvent, transform.position);
        hasTriggered = true;
    }
}
