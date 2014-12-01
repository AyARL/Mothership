using UnityEngine;
using System.Collections;
using MothershipOS;
using MothershipUtility;
using System;

public class ProfileGUI : MonoBehaviour
{
    [SerializeField]
    private GameObject content = null;
    [SerializeField]
    private WaitScreenGUI waitScreen = null;


    public void EnableScreen()
    {
        if (UserDataManager.userData.Profile == null)
        {
            waitScreen.Enable("Fetching Profile Data");
            StartCoroutine(GetProfile());
        }
        content.SetActive(true);
    }

    public void DisableScreen()
    {
        content.SetActive(false);
    }

    private IEnumerator GetProfile()
    {
        WWWForm loginForm = WWWFormUtility.GetLoginForm(UserDataManager.userData.User);
        WWW response = new WWW(WWWFormUtility.getProfileURL, loginForm);
        yield return response;


        if (response.error == null)
        {
            Debug.Log(response.text);
            if (ReadResponse(response.text))
            {
                waitScreen.Disable();
                FillData();
            }
        }
        else
        {
            waitScreen.SetMessage(response.error);
        }
    }

    private bool ReadResponse(string input)
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
            Profile profile = JsonValidator.ValidateJsonData<Profile>(input);
            if(profile != default(Profile))
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

    private void FillData()
    {

    }

}
