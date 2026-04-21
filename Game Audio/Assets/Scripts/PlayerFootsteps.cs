using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class PlayerFootstep : MonoBehaviour
{

    public EventReference footstepSound;
    public EventReference footstepSound2;

    public float rayDistance = 2f;

    public void PlayerFootsteps()
    {
        Debug.DrawRay(transform.position + Vector3.up * 0.5f, Vector3.down * rayDistance, Color.red);

        
        RaycastHit hit;
        
        if(Physics.Raycast(transform.position + Vector3.up * 0.5f, Vector3.down, out hit, rayDistance))
        {
            if (hit.collider.CompareTag("Metal"))
            {
                RuntimeManager.PlayOneShot(footstepSound2, transform.position);    
            }
            else{
                RuntimeManager.PlayOneShot(footstepSound, transform.position);
            }
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
