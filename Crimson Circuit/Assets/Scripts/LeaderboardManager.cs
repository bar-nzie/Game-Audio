using UnityEngine;
using Steamworks;
using System.Collections.Generic;

public class LeaderboardEntryData
{
    public int Rank;
    public int Score;
    public CSteamID SteamID;
    public string Username;
}

public class LeaderboardManager : MonoBehaviour
{
    public static LeaderboardManager Instance;

    private SteamLeaderboard_t leaderboard;
    private CallResult<LeaderboardFindResult_t> findResult;
    private CallResult<LeaderboardScoreUploaded_t> uploadResult;
    private List<LeaderboardEntryData> downloadedEntries = new List<LeaderboardEntryData>();
    private CallResult<LeaderboardScoresDownloaded_t> downloadResult;
    public System.Action<List<LeaderboardEntryData>> OnTopScoresDownloaded;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        FindLeaderboard("CrimsonCircuitTest");
    }

    // Find or create leaderboard
    public void FindLeaderboard(string leaderboardName)
    {
        findResult = CallResult<LeaderboardFindResult_t>.Create(OnLeaderboardFound);
        SteamAPICall_t handle = SteamUserStats.FindOrCreateLeaderboard(
            leaderboardName,
            ELeaderboardSortMethod.k_ELeaderboardSortMethodDescending,
            ELeaderboardDisplayType.k_ELeaderboardDisplayTypeNumeric
        );
        findResult.Set(handle);
    }

    private void OnLeaderboardFound(LeaderboardFindResult_t result, bool failure)
    {
        if (!failure && result.m_bLeaderboardFound != 0)
        {
            leaderboard = result.m_hSteamLeaderboard;
            Debug.Log("Leaderboard found: " + leaderboard.m_SteamLeaderboard);
        }
        else
        {
            Debug.LogError("Failed to find/create leaderboard");
        }
    }

    // Upload player score
    public void UploadScore(int score)
    {
        if (leaderboard.m_SteamLeaderboard == 0)
        {
            Debug.LogError("Leaderboard not set yet!");
            return;
        }

        uploadResult = CallResult<LeaderboardScoreUploaded_t>.Create(OnScoreUploaded);
        SteamAPICall_t handle = SteamUserStats.UploadLeaderboardScore(
            leaderboard,
            ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodKeepBest,
            score,
            null,
            0
        );
        uploadResult.Set(handle);
    }

    private void OnScoreUploaded(LeaderboardScoreUploaded_t result, bool failure)
    {
        if (!failure && result.m_bSuccess != 0)
        {
            Debug.Log("Score uploaded: " + result.m_nScore);
        }
        else
        {
            Debug.LogError("Failed to upload score");
        }
    }

    public void DownloadTopScores(int count = 10)
    {
        if (leaderboard.m_SteamLeaderboard == 0)
        {
            Debug.LogError("Leaderboard not initialized yet.");
            return;
        }

        downloadResult = CallResult<LeaderboardScoresDownloaded_t>.Create(OnLeaderboardScoresDownloaded);

        SteamAPICall_t handle = SteamUserStats.DownloadLeaderboardEntries(
            leaderboard,
            ELeaderboardDataRequest.k_ELeaderboardDataRequestGlobal,
            1, // Start at rank 1
            count
        );

        downloadResult.Set(handle);
    }

    private void OnLeaderboardScoresDownloaded(LeaderboardScoresDownloaded_t result, bool failure)
    {
        if (failure)
        {
            Debug.LogError("Failed to download leaderboard scores.");
            return;
        }

        downloadedEntries.Clear();

        for (int i = 0; i < result.m_cEntryCount; i++)
        {
            LeaderboardEntry_t entry;
            int[] details = new int[0]; // Not using additional score details here

            bool success = SteamUserStats.GetDownloadedLeaderboardEntry(
                result.m_hSteamLeaderboardEntries,
                i,
                out entry,
                details,
                details.Length
            );

            if (success)
            {
                string username = SteamFriends.GetFriendPersonaName(entry.m_steamIDUser);

                LeaderboardEntryData data = new LeaderboardEntryData
                {
                    Rank = entry.m_nGlobalRank,
                    Score = entry.m_nScore,
                    SteamID = entry.m_steamIDUser,
                    Username = username
                };

                downloadedEntries.Add(data);

                Debug.Log($"#{data.Rank} - {data.Username} - Score: {data.Score}");
            }
        }
        OnTopScoresDownloaded?.Invoke(new List<LeaderboardEntryData>(downloadedEntries));
    }
    public List<LeaderboardEntryData> GetDownloadedEntries()
    {
        return new List<LeaderboardEntryData>(downloadedEntries);
    }
}