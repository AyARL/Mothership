using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using MothershipUtility;
using MothershipOS;
using System;

namespace MothershipUI
{
    public class RegisterGUI : MonoBehaviour
    {
        [SerializeField]
        private GameObject content = null;

        [SerializeField]
        private string registerBaseURL = "http://studentnet.kingston.ac.uk/~k1159960/register.php";

        [SerializeField]
        private InputField emailField = null;
        [SerializeField]
        private InputField passwordField = null;
        [SerializeField]
        private InputField displayName = null;
        [SerializeField]
        private Button submitButton = null;
        [SerializeField]
        private Text message = null;

        private void Start()
        {
            emailField.characterValidation = InputField.CharacterValidation.EmailAddress;
            emailField.contentType = InputField.ContentType.EmailAddress;

            passwordField.contentType = InputField.ContentType.Password;
            passwordField.characterValidation = InputField.CharacterValidation.Alphanumeric;

            displayName.characterValidation = InputField.CharacterValidation.Alphanumeric;
            displayName.contentType = InputField.ContentType.Alphanumeric;

            submitButton.onClick.AddListener(Submit);
        }

        public void EnableScreen()
        {
            content.SetActive(true);
        }

        public void DisableScreen()
        {
            content.SetActive(false);
        }

        private void Submit()
        {
            if (passwordField.text.Length >= 6 && displayName.text.Length >= 2)
            {
                submitButton.interactable = false;
                message.text = "Attempting Registration...";
                WWWForm form = CreateForm();
                StartCoroutine(Register(form));
            }
            else
            {
                message.text = "Incorrect input";
            }
        }

        private WWWForm CreateForm()
        {
            WWWForm form = new WWWForm();
            form.AddField("Email", emailField.text);
            form.AddField("Password", HashUtility.GetMD5Hash(passwordField.text));
            form.AddField("DisplayName", displayName.text);
            form.AddField("Hash", HashUtility.GetMD5Hash(emailField.text + AppKey.appKey));

            return form;
        }

        private IEnumerator Register(WWWForm form)
        {
            WWW response = new WWW(registerBaseURL, form);
            yield return response;

            if(response.error == null)
            {
                Debug.Log(response.text);
                ReadResponse(response.text);
            }
            else
            {
                message.text = response.error;
            }
        }

        private void ReadResponse(string input)
        {
            // Check if error code was returned, otherwise try to decode form Json
            int errorCode;
            if(Int32.TryParse(input, out errorCode))
            {
                Debug.Log(Enum.GetName(typeof(ResponseEnums.AccountCreationResponse), errorCode));
                message.text = Enum.GetName(typeof(ResponseEnums.AccountCreationResponse), errorCode);
            }
            else
            {
                User user = JsonValidator.ValidateJsonData<User>(input);
                if(user != default(User))
                {
                    message.text = "Registered. Logging in..";
                }
                else
                {
                    Debug.LogError("Failed to deserialize as User: " + input);
                }
            }
        }
    }
}
