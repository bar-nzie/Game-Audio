using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class SteamStuff : MonoBehaviour
{
    public static SteamStuff Instance { get; private set; }
    public bool Initialized { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        try
        {
            Initialized = SteamAPI.Init();
            if (!Initialized)
            {
                Debug.LogError("SteamAPI.Init() failed.");
            }
        }
        catch (System.DllNotFoundException e)
        {
            Debug.LogError("Steamworks native DLLs not found: " + e);
        }
    }

    private void Update()
    {
        if (Initialized)
            SteamAPI.RunCallbacks();
    }

    private void OnApplicationQuit()
    {
        if (Initialized)
            SteamAPI.Shutdown();
    }
}
