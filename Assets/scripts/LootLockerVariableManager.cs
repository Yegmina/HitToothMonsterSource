using System;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic; // Добавлено для использования List<>


public class LootLockerVariableManager : MonoBehaviour
{
    public void IncrementVariable(string variableName)
    {
        PlayFabClientAPI.GetPlayerStatistics(new GetPlayerStatisticsRequest(), (result) =>
        {
            bool variableFound = false;
            int currentValue = 0;

            // Проверка существования переменной
            foreach (var stat in result.Statistics)
            {
                if (stat.StatisticName == variableName)
                {
                    variableFound = true;
                    currentValue = stat.Value;
                    break;
                }
            }

            if (variableFound)
            {
                // Увеличение значения переменной на 1
                currentValue++;
                UpdateVariable(variableName, currentValue);
            }
            else
            {
                // Создание новой переменной со значением 1
                CreateVariable(variableName, 1);
            }
        }, (error) =>
        {
            Debug.LogError("Error getting player statistics: " + error.GenerateErrorReport());
        });
    }

    private void UpdateVariable(string variableName, int newValue)
    {
        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate { StatisticName = variableName, Value = newValue }
            }
        };

        PlayFabClientAPI.UpdatePlayerStatistics(request, (result) =>
        {
            Debug.Log($"Successfully updated variable '{variableName}' with value '{newValue}'");
        }, (error) =>
        {
            Debug.LogError("Error updating variable: " + error.GenerateErrorReport());
        });
    }

    private void CreateVariable(string variableName, int initialValue)
    {
        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate { StatisticName = variableName, Value = initialValue }
            }
        };

        PlayFabClientAPI.UpdatePlayerStatistics(request, (result) =>
        {
            Debug.Log($"Successfully created variable '{variableName}' with value '{initialValue}'");
        }, (error) =>
        {
            Debug.LogError("Error creating variable: " + error.GenerateErrorReport());
        });
    }
}
