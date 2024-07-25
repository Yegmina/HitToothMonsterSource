using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;

public class money : MonoBehaviour
{
    public GlobalSettings globalSettings; // Reference to the global settings ScriptableObject
    public int currentMoney;
    public int moneyToAdd;
    public Text moneyText; // Reference to the UI Text or InputField

    public Button snailButton; // Reference to the button to be shown
    public int snailThreshold = 1000; // Threshold value

    public Button sleepButton; // Reference to the button to be shown
    public int sleepThreshold = 1000; // Threshold value

    public Button maxHpButton;
    public int maxHpThreshold = 10000;
    
    public Button moneypercoinButton;
    public int moneypercoinThreshold = 1000;

    private StartGame startGameScript;

    void Start()
    {
        startGameScript = FindObjectOfType<StartGame>();

        // Assuming testjs script will handle login, so we check if user is already logged in
        if (globalSettings.usePlayFab && PlayFabClientAPI.IsClientLoggedIn())
        {
            LoadMoneyAndMoneyToAdd();
        }
		AddMoney(0);
    }

    void LoadMoneyAndMoneyToAdd()
    {
        PlayFabClientAPI.GetPlayerStatistics(new GetPlayerStatisticsRequest(), OnPlayFabGetStatisticsSuccess, OnPlayFabGetStatisticsFailure);
    }

    void OnPlayFabGetStatisticsSuccess(GetPlayerStatisticsResult result)
    {
        bool moneyFound = false;
        bool moneyToAddFound = false;

        foreach (var stat in result.Statistics)
        {
            if (stat.StatisticName == "money")
            {
                currentMoney = stat.Value;
                moneyFound = true;
                Debug.Log("Money found in PlayFab storage: " + currentMoney);
            }
            else if (stat.StatisticName == "moneyToAdd")
            {
                moneyToAdd = stat.Value;
                moneyToAddFound = true;
                Debug.Log("moneyToAdd found in PlayFab storage: " + moneyToAdd);
            }
        }

        if (!moneyFound)
        {
            currentMoney = 0; // Default value if not found
            SaveMoney(currentMoney);
            Debug.Log("Money not found in PlayFab, setting to 0");
        }

        if (!moneyToAddFound)
        {
            moneyToAdd = 1; // Default value if not found
            SaveMoneyToAdd(moneyToAdd);
            Debug.Log("moneyToAdd not found in PlayFab, setting to 1");
        }

        UpdateMoneyText(); // Update the text field
        CheckMoneyThresholds(); // Check button visibility conditions
    }

    void OnPlayFabGetStatisticsFailure(PlayFabError error)
    {
        Debug.LogError("Error retrieving statistics from PlayFab: " + error.GenerateErrorReport());
    }

    void SaveMoney(int amount)
    {
        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate { StatisticName = "money", Value = amount }
            }
        };

        PlayFabClientAPI.UpdatePlayerStatistics(request, OnPlayFabUpdateStatisticsSuccess, OnPlayFabUpdateStatisticsFailure);
    }

    void OnPlayFabUpdateStatisticsSuccess(UpdatePlayerStatisticsResult result)
    {
        Debug.Log("Successfully set money in PlayFab storage: " + currentMoney);
        UpdateMoneyText(); // Update the text field
        CheckMoneyThresholds(); // Check button visibility conditions
    }

    void OnPlayFabUpdateStatisticsFailure(PlayFabError error)
    {
        Debug.LogError("Error setting user data in PlayFab: " + error.GenerateErrorReport());
    }

    // Public function to add money
    public void AddMoney(int addingMoney)
    {
		if (addingMoney<0){
		snailButton.gameObject.SetActive(false);
		sleepButton.gameObject.SetActive(false);
		maxHpButton.gameObject.SetActive(false);
		moneypercoinButton.gameObject.SetActive(false);
		}
		//snailButton.gameObject.SetActive(false);
		
        // Retrieve the current "money" value from PlayFab server before updating
        PlayFabClientAPI.GetPlayerStatistics(new GetPlayerStatisticsRequest(), (result) =>
        {
            bool moneyFound = false;
            foreach (var stat in result.Statistics)
            {
                if (stat.StatisticName == "money")
                {
                    currentMoney = stat.Value;
                    moneyFound = true;
                    break;
                }
            }

            if (!moneyFound)
            {
                currentMoney = 0; // Default value if not found
            }

            // Increase the "money" value
            currentMoney += addingMoney;
            SaveMoney(currentMoney); // Save the new value on the server
            Debug.Log("Added money: " + addingMoney + ". New total: " + currentMoney);
        }, OnPlayFabGetStatisticsFailure);
    }

    // Function to update the text in the text field
    void UpdateMoneyText()
    {
        if (moneyText != null)
        {
            moneyText.text = "Money: " + currentMoney;
        }
    }

    // Check button visibility conditions
    void CheckMoneyThresholds()
    {
        SnailButtonCheckMoneyThreshold();
        SleepButtonCheckMoneyThreshold();
        MaxHpButtonCheckMoneyThreshold();
        MoneypercoinCheckMoneyThreshold();
    }

    void SnailButtonCheckMoneyThreshold()
    {
        if (snailButton != null)
        {
            snailButton.gameObject.SetActive(currentMoney > snailThreshold);
        }
    }

    void SleepButtonCheckMoneyThreshold()
    {
        if (sleepButton != null)
        {
            sleepButton.gameObject.SetActive(currentMoney > sleepThreshold);
        }
    }

    void MaxHpButtonCheckMoneyThreshold()
    {
        if (maxHpButton != null && startGameScript != null)
        {
            maxHpButton.gameObject.SetActive(currentMoney > maxHpThreshold && startGameScript.maxhp < 4);
        }
    }

    void MoneypercoinCheckMoneyThreshold()
    {
        if (moneypercoinButton != null)
        {
            moneypercoinButton.gameObject.SetActive(currentMoney > moneypercoinThreshold);
        }
    }
	// New functions for moneyToAdd
    public void LoadMoneyToAdd()
    {
        PlayFabClientAPI.GetPlayerStatistics(new GetPlayerStatisticsRequest(), (result) =>
        {
            bool moneyToAddFound = false;
            foreach (var stat in result.Statistics)
            {
                if (stat.StatisticName == "moneyToAdd")
                {
                    moneyToAdd = stat.Value;
                    moneyToAddFound = true;
                    Debug.Log("moneyToAdd found in PlayFab storage: " + moneyToAdd);
                    break;
                }
            }

            if (!moneyToAddFound)
            {
                moneyToAdd = 1; // Default value if not found
                SaveMoneyToAdd(moneyToAdd);
                Debug.Log("moneyToAdd not found in PlayFab, setting to 1");
            }
        }, OnPlayFabGetStatisticsFailure);
    }
    // New functions for moneyToAdd
    public void SaveMoneyToAdd(int amount)
    {
        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate { StatisticName = "moneyToAdd", Value = amount }
            }
        };

        PlayFabClientAPI.UpdatePlayerStatistics(request, (result) =>
        {
            Debug.Log("Successfully set moneyToAdd in PlayFab storage: " + amount);
        }, OnPlayFabUpdateStatisticsFailure);
    }

    public void UpdateMoneyToAdd(int amount)
    {
        moneyToAdd = amount;
        SaveMoneyToAdd(moneyToAdd);
    }

    public void AddMoneyPerCoin(int amount)
    {
        moneyToAdd += amount;
        SaveMoneyToAdd(moneyToAdd);
    }
}
