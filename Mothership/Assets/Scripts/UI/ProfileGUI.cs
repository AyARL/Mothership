using UnityEngine;
using System.Collections;
using MothershipOS;
using MothershipUtility;
using System;
using UnityEngine.UI;
using Mothership;

namespace MothershipUI
{

    public class ProfileGUI : MonoBehaviour
    {
        [SerializeField]
        private GameObject content = null;
        [SerializeField]
        private WaitScreenGUI waitScreen = null;
        [SerializeField]
        private MainMenuGUI mainMenu = null;
        [SerializeField]
        private ServerSelectGUI serverSelection = null;

        // Profile
        [SerializeField]
        private Text displayName = null;
        [SerializeField]
        private Text totalEXP = null;
        [SerializeField]
        private Text gamesCount = null;
        [SerializeField]
        private Text winRate = null;

        //Last game played
        [SerializeField]
        private GameObject dataGroup = null;
        [SerializeField]
        private Text noDataLabel = null;
        [SerializeField]
        private Text datePlayed = null;
        [SerializeField]
        private Text gameResult = null;
        [SerializeField]
        private Text expEarned = null;

        [SerializeField]
        private Button playButton = null;
        [SerializeField]
        private Button logOutButton = null;

        private PlayerGameStats lastGameStats = null;

        private void Start()
        {
            playButton.onClick.AddListener(SwitchToServerSelection);
            logOutButton.onClick.AddListener(LogOut);
        }

        public void EnableScreen()
        {
            if (UserDataManager.userData.Profile == null)
            {
                waitScreen.Enable("Fetching Profile Data");
                StartCoroutine(GetProfile());
            }
            else
            {
                FillProfileData();
                FillLastGameData();
            }
            content.SetActive(true);
        }

        public void DisableScreen()
        {
            displayName.text = "";
            totalEXP.text = "";
            gamesCount.text = "";
            winRate.text = "";

            datePlayed.text = "";
            gameResult.text = "";
            expEarned.text = "";

            content.SetActive(false);
        }

        private IEnumerator GetProfile()
        {
            WWWForm loginForm = WWWFormUtility.GetLoginForm(UserDataManager.userData.User);
            WWW loginResponse = new WWW(WWWFormUtility.getProfileURL, loginForm);
            yield return loginResponse;

            if (loginResponse.error == null)
            {
                Debug.Log(loginResponse.text);
                if (ReadProfileResponse(loginResponse.text))
                {
                    FillProfileData();
                    StartCoroutine(GetLastGameData());
                }
            }
            else
            {
                waitScreen.SetMessage(loginResponse.error);
            }
        }

        private bool ReadProfileResponse(string input)
        {
            // Check if error code was returned, otherwise try to decode form Json
            int errorCode;
            if (Int32.TryParse(input, out errorCode))
            {
                Debug.Log(Enum.GetName(typeof(ResponseEnums.GetProfileResponse), errorCode));
                waitScreen.SetMessage(Enum.GetName(typeof(ResponseEnums.GetProfileResponse), errorCode));
                return false;
            }
            else
            {
                Profile profile = JsonUtility.ValidateJsonData<Profile>(input);
                if (profile != default(Profile))
                {
                    UserDataManager.userData.Profile = profile;
                    return true;
                }
                else
                {
                    Debug.LogError("Failed to deserialize as Profile: " + input);
                    return false;
                }

            }
        }

        private IEnumerator GetLastGameData()
        {
            WWWForm loginForm = WWWFormUtility.GetLoginForm(UserDataManager.userData.User);
            WWW gameDataResponse = new WWW(WWWFormUtility.getLastGameURL, loginForm);
            yield return gameDataResponse;

            if (gameDataResponse.error == null)
            {
                Debug.Log(gameDataResponse.text);
                if (ReadGameResponse(gameDataResponse.text))
                {
                    FillLastGameData();
                    waitScreen.Disable();
                }
            }
            else
            {
                waitScreen.Disable();
            }
        }

        private bool ReadGameResponse(string data)
        {
            // Check if error code was returned, otherwise try to decode form Json
            int errorCode;
            if (Int32.TryParse(data, out errorCode))
            {
                if ((ResponseEnums.GetLastGameResponse)errorCode == ResponseEnums.GetLastGameResponse.Error_NoData)
                {
                    return true;
                }
                else
                {
                    Debug.Log(Enum.GetName(typeof(ResponseEnums.GetLastGameResponse), errorCode));
                    return false;
                }
            }
            else
            {
                PlayerGameStats gameStats = JsonUtility.ValidateJsonData<PlayerGameStats>(data);
                if (gameStats != default(PlayerGameStats))
                {
                    lastGameStats = gameStats;
                    return true;
                }
                else
                {
                    Debug.LogError("Failed to deserialize as GameStats: " + data);
                    return false;
                }
            }
        }

        private void FillProfileData()
        {
            Profile profile = UserDataManager.userData.Profile;

            displayName.text = profile.DisplayName;
            totalEXP.text = profile.EXP.ToString();
            gamesCount.text = profile.GamesPlayed.ToString();

            if (profile.GamesPlayed > 0)
            {
                float rate = ((float)profile.GamesWon / (float)profile.GamesPlayed) * 100f;
                winRate.text = string.Format("{0:F2}%", rate);
            }
            else
            {
                winRate.text = "0%";
            }
        }

        private void FillLastGameData()
        {
            if (lastGameStats != null)
            {
                noDataLabel.gameObject.SetActive(false);
                dataGroup.SetActive(true);
                datePlayed.text = lastGameStats.DatePlayed;
                gameResult.text = lastGameStats.Winner == lastGameStats.Team ? "Win" : "Loss";
                expEarned.text = lastGameStats.EXPEarned.ToString();
            }
            else
            {
                noDataLabel.gameObject.SetActive(true);
                dataGroup.SetActive(false);
            }
        }

        private void SwitchToServerSelection()
        {
            DisableScreen();
            serverSelection.EnableScreen();
        }

        private void LogOut()
        {
            UserDataManager.userData.Clear();

            if (NetworkManager.networkManager != null)
            {
                NetworkManager.networkManager.RemoveNetworkManager();
            }

            DisableScreen();
            mainMenu.EnableScreen();
        }

    }
}