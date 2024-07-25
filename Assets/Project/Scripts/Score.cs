using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    public static Score scoreScript;
    public static string maxScoreString = "MAX_SCORE";
    Text textScore;
    int score;
    int maxScore;

    // Use this for initialization
    void Start()
    {
        textScore = GameObject.Find("Score").GetComponent<Text>();
        scoreScript = gameObject.GetComponent<Score>();

        StartCoroutine(CheckPlayFabLoginStatus());
    }

    IEnumerator CheckPlayFabLoginStatus()
    {
        while (!PlayFabClientAPI.IsClientLoggedIn())
        {
            yield return new WaitForSeconds(0.5f);
        }

        Debug.Log("User is logged in with PlayFab");
        LoadMaxScore();
    }

    void LoadMaxScore()
    {
        PlayFabClientAPI.GetPlayerStatistics(new GetPlayerStatisticsRequest(), OnPlayFabGetStatisticsSuccess, OnPlayFabGetStatisticsFailure);
    }

    void OnPlayFabGetStatisticsSuccess(GetPlayerStatisticsResult result)
    {
        bool maxScoreFound = false;
        foreach (var stat in result.Statistics)
        {
            if (stat.StatisticName == maxScoreString)
            {
                maxScore = stat.Value;
                maxScoreFound = true;
                Debug.Log("MaxScore found in PlayFab storage: " + maxScore);
                break;
            }
        }

        if (!maxScoreFound)
        {
            maxScore = 0; // Default value if not found
            SaveMaxScore(maxScore);
            Debug.Log("MaxScore not found in PlayFab, setting to 0");
        }
        textScore.text = maxScore.ToString();
    }

    void OnPlayFabGetStatisticsFailure(PlayFabError error)
    {
        Debug.LogError("Error retrieving statistics from PlayFab: " + error.GenerateErrorReport());
    }

    void SaveMaxScore(int amount)
    {
        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate { StatisticName = maxScoreString, Value = amount }
            }
        };

        PlayFabClientAPI.UpdatePlayerStatistics(request, OnPlayFabUpdateStatisticsSuccess, OnPlayFabUpdateStatisticsFailure);
    }

    void OnPlayFabUpdateStatisticsSuccess(UpdatePlayerStatisticsResult result)
    {
        Debug.Log("Successfully set maxScore in PlayFab storage: " + maxScore);
    }

    void OnPlayFabUpdateStatisticsFailure(PlayFabError error)
    {
        Debug.LogError("Error setting user data in PlayFab: " + error.GenerateErrorReport());
    }

    public void AddScore(int count) // The method is responsible for adding points and writing the maximum value
    {
        score += count;
        if (maxScore < score)
        {
            maxScore = score;
            SaveMaxScore(maxScore);
        }
        if (score < 0)
            score = 0;
        textScore.text = score.ToString();
    }

    public void SetMaxScore()
    {
        // Получаем значение maxScore из хранилища и обновляем UI
        LoadMaxScore();
    }
}
