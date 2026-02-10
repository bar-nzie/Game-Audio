using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Score : MonoBehaviour
{
    public TextMeshProUGUI level;
    public TextMeshProUGUI coins;

    private int levelScore;
    private int coinsValue;

    public LeaderboardManager manager;

    // Start is called before the first frame update
    void Start()
    {
        level.text = "Level:";
        coins.text = "Coins:";
    }

    public void LevelUpdate(int value)
    {
        levelScore += value;
        level.text = "Level: " + levelScore.ToString();
        manager.UploadScore(levelScore);
    }

    public void CoinsUpdate(int value)
    {
        coinsValue += value;
        coins.text = "Coins: " + coinsValue.ToString();
    }

    public int GetCoins() { return coinsValue; }

    public void SetCoins(int value)
    {
        coinsValue = value;
        coins.text = "Coins: " + coinsValue.ToString();
    }
}
