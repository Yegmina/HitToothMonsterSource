using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using PlayFab;
using PlayFab.ClientModels;

public class testjs : MonoBehaviour {
    [DllImport("__Internal")]
    private static extern string GetUserName();

    public Text userNameText;
    public GlobalSettings globalSettings; // Reference to the global settings ScriptableObject
    public bool isEditorTest; // Public variable to check if running in Unity Editor
    public string editorUserName; // Public variable to set username in Unity Editor

    void Start() {
        string userName;

        #if UNITY_EDITOR
        if (isEditorTest) {
            userName = editorUserName;
        } else {
            userName = "DefaultEditorUser"; // Default username if not set
        }
        #else
        userName = GetUserName();
        #endif

        userNameText.text = userName;

        if (!string.IsNullOrEmpty(userName))
        {
           
                // Use PlayFab for user management
                LoginWithPlayFab(userName);
           

        }
    }

    // PlayFab Methods
    void LoginWithPlayFab(string userName)
    {
        var request = new LoginWithCustomIDRequest
        {
            CustomId = userName,
            CreateAccount = true
        };

        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccessPlayFab, OnLoginFailurePlayFab);
    }

    void OnLoginSuccessPlayFab(LoginResult result)
    {
        Debug.Log("PlayFab login successful");
        SetPlayerNamePlayFab(result.PlayFabId, userNameText.text);
    }

    void OnLoginFailurePlayFab(PlayFabError error)
    {
        Debug.LogError("PlayFab login failed: " + error.GenerateErrorReport());
    }

    void SetPlayerNamePlayFab(string playFabId, string userName)
    {
        var request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = userName
        };

        PlayFabClientAPI.UpdateUserTitleDisplayName(request, OnDisplayNameUpdatePlayFab, OnDisplayNameUpdateFailurePlayFab);
    }

    void OnDisplayNameUpdatePlayFab(UpdateUserTitleDisplayNameResult result)
    {
        Debug.Log("Username successfully saved: " + result.DisplayName);
        userNameText.text = "Username: " + result.DisplayName;
    }

    void OnDisplayNameUpdateFailurePlayFab(PlayFabError error)
    {
        Debug.LogError("Error saving username: " + error.GenerateErrorReport());
    }

    
}
