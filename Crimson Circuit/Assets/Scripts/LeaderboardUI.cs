using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class LeaderboardUI : MonoBehaviour
{
    [System.Serializable]
    public class EntryUI
    {
        public TextMeshProUGUI UsernameText;
        public TextMeshProUGUI ScoreText;
    }

    public List<EntryUI> entriesUI = new List<EntryUI>();

    private void Start()
    {
        // Wait 10 seconds, then download
        StartCoroutine(DownloadScoresAfterDelay());
    }

    private IEnumerator DownloadScoresAfterDelay()
    {
        yield return new WaitForSeconds(10f); 

        LeaderboardManager.Instance.DownloadTopScores(10);

        // Optionally wait another second before fetching entries
        yield return new WaitForSeconds(1f); // Wait for scores to be downloaded

        List<LeaderboardEntryData> entries = LeaderboardManager.Instance.GetDownloadedEntries();

        for (int i = 0; i < entriesUI.Count; i++)
        {
            if (i < entries.Count)
            {
                entriesUI[i].UsernameText.text = entries[i].Username;
                entriesUI[i].ScoreText.text = entries[i].Score.ToString();
            }
            else
            {
                entriesUI[i].UsernameText.text = "---";
                entriesUI[i].ScoreText.text = "---";
            }
        }
    }
}
