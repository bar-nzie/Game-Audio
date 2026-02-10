using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem.HID;

public class LazerBehaviour : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public LineRenderer telegraphLaser;
    public Transform boss;
    public GameObject player;
    public Health health;
    public float xOffset = 50f;
    public float zRange = 50f;
    private float sweepSpeed = 10f;
    private float damage = 10f;

    private float currentZ;
    private int direction = -1;
    private bool isFiring = false;

    private float telegraphTime = 1.5f;
    private float activeTime = 3f;
    private Color safeColor = Color.blue;
    private Color dangerColor = new Color(0.6f, 0f, 0.6f);


    // Start is called before the first frame update
    void Start()
    {
        currentZ = zRange;
        lineRenderer.enabled = false;
        telegraphLaser.enabled = false;
        lineRenderer.startWidth = 0.75f;
        telegraphLaser.startWidth = 0.75f;
        telegraphLaser.endWidth = 0.75f;
        lineRenderer.endWidth = 0.75f;
        player = GameObject.Find("Player");
        health = player.GetComponent<Health>();
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.K))
        //{
        //    StartLaser();
        //}
        if (!isFiring) return;

        currentZ += direction * sweepSpeed * Time.deltaTime;

        if (currentZ <= -50f)
        {
            isFiring = false;
            lineRenderer.enabled = false;
            currentZ = zRange;
            return;
        }
        //if (currentZ <= -zRange || currentZ >= zRange)
        //{
        //    direction *= -1;
        //}

        Vector3 startPos = boss.position + new Vector3(xOffset, -4, currentZ);
        Vector3 endPos = boss.position + new Vector3(-xOffset, -4, currentZ);

        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, endPos);

        Vector3 dir = (endPos - startPos).normalized;
        float dist = Vector3.Distance(startPos, endPos);

        RaycastHit[] hits;
        if (Physics.CapsuleCastAll(startPos, endPos, 0.75f, dir, dist).Any(h=> h.collider.CompareTag("Player")))
        {
            health.TakeDamage(200*Time.deltaTime);
        }
    }
    public void StartLaser()
    {
        isFiring = true;
        lineRenderer.enabled = true;
    }

    public void Activate(Vector3 start, Vector3 end)
    {
        StartCoroutine(LazerGrid(start, end));
    }

    private IEnumerator LazerGrid(Vector3 start, Vector3 end)
    {
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
        telegraphLaser.SetPosition(0, start);
        telegraphLaser.SetPosition(1, end);

        telegraphLaser.enabled = true;

        yield return new WaitForSeconds(telegraphTime);

        telegraphLaser.enabled = false;
        lineRenderer.enabled = true;
        Vector3 dir = (end - start).normalized;
        float dist = Vector3.Distance(start, end);

        float elapsed = 0f;
        while (elapsed < activeTime)
        {
            RaycastHit[] hits = Physics.CapsuleCastAll(start, end, 0.75f, dir, dist);
            foreach (var hit in hits)
            {
                if (hit.collider.CompareTag("Player"))
                {
                    health.TakeDamage(200 * Time.deltaTime);
                }
            }

            elapsed += Time.deltaTime;
            yield return null;
        }
        lineRenderer.enabled = false;
        yield return new WaitForSeconds(activeTime);

        lineRenderer.enabled = false;

    }
}
