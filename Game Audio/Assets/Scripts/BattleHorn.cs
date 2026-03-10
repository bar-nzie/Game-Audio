using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using Unity.VisualScripting;

public class BattleHorn : MonoBehaviour
{
    public string hornRef;

    private EventInstance horn;
    private GameObject player;

    public float rayDistance = 2f;
    public LayerMask floorLayer;

    private float timer;
    private float cooldown;

    void Start()
    {
        horn = RuntimeManager.CreateInstance(hornRef);
        player = GameObject.FindGameObjectWithTag("Player");
        //horn.start();
        horn.setParameterByName("FloorLevel", 0f);
        cooldown = 10;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        Ray ray = new Ray(transform.position + Vector3.up * 0.5f, Vector3.down);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, rayDistance, floorLayer))
        {
            horn.setParameterByName("FloorLevel", 0f);
        }
        else
        {
            horn.setParameterByName("FloorLevel", 1f);
            float dist = Vector3.Distance(transform.position, player.transform.position);
            dist /= 40;
            horn.setParameterByName("Distance", dist);
            if (timer >= cooldown)
            {
                timer = 0;
                cooldown = Random.Range(7, 13);
                horn.start();
            }
        }
    }
}
