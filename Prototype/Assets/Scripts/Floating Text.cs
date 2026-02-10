using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FloatingText : MonoBehaviour
{
    public GameObject floatingTextPrefab;
    public GameObject player;
    public GameObject placeHolder;
    public GameObject camPos;
    public GameObject benching;
    public Image progressBar;
    public GameObject bench;
    Vector3 originalPos;
    public Playermovement playerMovement;
    bool isBenching;
    float progress;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        floatingTextPrefab.SetActive(false);
        benching.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

        if (Vector3.Distance(transform.position, player.transform.position) <= 5 && !isBenching)
        {
            floatingTextPrefab.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E))
            {
                isBenching = true;
                originalPos = bench.transform.position;
                floatingTextPrefab.SetActive(false);
            }
        }
        else
        {
            floatingTextPrefab.SetActive(false);

        }

        if (isBenching)
        {
            benching.SetActive(true);
            if (Input.GetKeyDown(KeyCode.Q))
            {
                benching.SetActive(false);
                isBenching = false;
                bench.transform.position = originalPos;
            }
            camPos.transform.position = placeHolder.transform.position;
            camPos.transform.rotation = placeHolder.transform.rotation;
            if (Input.GetKeyDown(KeyCode.M))
            {
                progress += 5f;
                bench.transform.position = new Vector3(bench.transform.position.x, bench.transform.position.y + 0.01f, bench.transform.position.z);
            }
            if (progress > 0f)
            {
                progress -= 20f * Time.deltaTime;
                bench.transform.position = new Vector3(bench.transform.position.x, bench.transform.position.y - (0.04f * Time.deltaTime), bench.transform.position.z);
            }
            if (progress >= 100f)
            {
                progress = 0f;
                bench.transform.position = originalPos;
                isBenching = false;
                benching.SetActive(false);
            }
        }
        progressBar.fillAmount = progress / 100f;
        transform.LookAt(player.transform);
        playerMovement.IsBenching(isBenching);
    }
}
