using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

public class StartGame : MonoBehaviour
{
    public float moveSpeed;
    public static StartGame startGameScript;

    float timeToStartGame;
    bool moveMask;
    bool timerFlag;
    bool moveUpMask;
    BoxCollider2D boxCollider;
    float maxPosY = 17f; // the maximum possible position of the mask
    float minPosY = -6f; // the minimum possible position of the mask
    float maskPosY;
    public int maxhp = 1;

    // Use this for initialization
    void Start()
    {
        boxCollider = gameObject.GetComponent<BoxCollider2D>();
        gameObject.transform.position = new Vector2(gameObject.transform.position.x, maxPosY);
        maskPosY = gameObject.transform.position.y;
        startGameScript = gameObject.GetComponent<StartGame>();

        StartCoroutine(CheckPlayFabLoginStatus());
    }

    IEnumerator CheckPlayFabLoginStatus()
    {
        while (!PlayFabClientAPI.IsClientLoggedIn())
        {
            yield return new WaitForSeconds(0.5f);
        }

        Debug.Log("User is logged in with PlayFab");
        LoadMaxHP();
    }

    void LoadMaxHP()
    {
        PlayFabClientAPI.GetPlayerStatistics(new GetPlayerStatisticsRequest(), OnPlayFabGetStatisticsSuccess, OnPlayFabGetStatisticsFailure);
    }

    void OnPlayFabGetStatisticsSuccess(GetPlayerStatisticsResult result)
    {
        bool maxHpFound = false;
        foreach (var stat in result.Statistics)
        {
            if (stat.StatisticName == "maxhp")
            {
                maxhp = stat.Value;
                maxHpFound = true;
                Debug.Log("MaxHP found in PlayFab storage: " + maxhp);
                break;
            }
        }

        if (!maxHpFound)
        {
            maxhp = 1; // Default value if not found
            SaveMaxHP(maxhp);
            Debug.Log("MaxHP not found in PlayFab, setting to 1");
        }
    }

    void OnPlayFabGetStatisticsFailure(PlayFabError error)
    {
        Debug.LogError("Error retrieving statistics from PlayFab: " + error.GenerateErrorReport());
    }

    void SaveMaxHP(int amount)
    {
        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate { StatisticName = "maxhp", Value = amount }
            }
        };

        PlayFabClientAPI.UpdatePlayerStatistics(request, OnPlayFabUpdateStatisticsSuccess, OnPlayFabUpdateStatisticsFailure);
    }

    void OnPlayFabUpdateStatisticsSuccess(UpdatePlayerStatisticsResult result)
    {
        Debug.Log("Successfully set maxhp in PlayFab storage: " + maxhp);
    }

    void OnPlayFabUpdateStatisticsFailure(PlayFabError error)
    {
        Debug.LogError("Error setting user data in PlayFab: " + error.GenerateErrorReport());
    }

    // Update is called once per frame
    void Update()
    {
        StartMoveMask(); // The method processes the movement of the mask and is responsible for starting the game
        TimeToStartGame(); // The timer counts down the time before the game starts
    }

    void StartMoveMask()
    {
        maskPosY = Mathf.Clamp(maskPosY, minPosY, maxPosY);
        if (moveMask)
        {
            if (maskPosY > minPosY && !moveUpMask) // Move the mask down
            {
                maskPosY -= Time.deltaTime * moveSpeed;
                if (maskPosY <= minPosY)
                {
                    moveUpMask = true;
                    moveMask = false;
                }
            }
            if (maskPosY < maxPosY && moveUpMask) // Move the mask up
            {
                maskPosY += Time.deltaTime * moveSpeed;
                if (maskPosY >= maxPosY)
                {
                    // Returning data to its original position, when the mask reaches its maximum height
                    boxCollider.enabled = true;
                    moveUpMask = false;
                    moveMask = false;
                    Score.scoreScript.SetMaxScore();
                    AudioManager.audioManager.PlayAudio(EnumAudioName.MAIN_MUSIC, false);
                }
            }
            gameObject.transform.position = new Vector2(gameObject.transform.position.x, maskPosY);
        }
    }

    public void HitMe(bool flag) // Method for processing the pressure on the mask
    {
        if (flag)
        {
            AudioManager.audioManager.PlayAudio(EnumAudioName.BUTTON_CLICK_MUSIC, true);
            HealPoint.healPointScript.ChangeHealPoint(maxhp); // health
            boxCollider.enabled = false;
            moveMask = true;
            timerFlag = true;
            timeToStartGame = 1.5f;
        }
    }

    public void LoseTheGame()
    {
        moveMask = true;
    }

    void TimeToStartGame() // time to the beginning of the game
    {
        if (timerFlag)
        {
            if (timeToStartGame > 0)
            {
                timeToStartGame -= Time.deltaTime;
            }
            else
            {
                HealPoint.GameOver = false; // The variable is responsible for the start and end of the game
                AudioManager.audioManager.PlayAudio(EnumAudioName.MAIN_MUSIC, true);
                timerFlag = false;
            }
        }
    }

    // Public function to set maxhp
    public void SetMaxHP(int newMaxHP) // newMaxHP добавление, а не нью, переделал
    {
        if (maxhp < 4)
        {
            maxhp += newMaxHP;
            SaveMaxHP(maxhp); // Сохраняем новое значение maxhp на сервере
        }
    }
}
