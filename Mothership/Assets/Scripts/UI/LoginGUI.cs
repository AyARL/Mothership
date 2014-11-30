using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using MothershipUtility;
using System;
using MothershipOS;

namespace MothershipUI
{

    public class LoginGUI : MonoBehaviour
    {
        [SerializeField]
        private GameObject content = null;

        [SerializeField]
        private string loginBaseURL = "http://studentnet.kingston.ac.uk/~k1159960/login.php";

        [SerializeField]
        private InputField emailField = null;
        [SerializeField]
        private InputField passwordField = null;
        [SerializeField]
        private Button submitButton = null;
        [SerializeField]
        private Text message = null;

        // Use this for initialization
        void Start()
        {
            emailField.characterValidation = InputField.CharacterValidation.EmailAddress;
            emailField.contentType = InputField.ContentType.EmailAddress;

            passwordField.contentType = InputField.ContentType.Password;
            passwordField.characterValidation = InputField.CharacterValidation.Alphanumeric;

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
            if (emailField.text.Length > 0 && passwordField.text.Length > 0)
            {
                submitButton.interactable = false;
                message.text = "Contacting Server...";
                WWWForm form = CreateForm();
                StartCoroutine(Login(form));
            }
        }

        private WWWForm CreateForm()
        {
            WWWForm form = new WWWForm();
            form.AddField("Email", emailField.text);
            form.AddField("Password", HashUtility.GetMD5Hash(passwordField.text));
            form.AddField("Hash", HashUtility.GetMD5Hash(emailField.text + AppKey.appKey));

            return form;
        }

        private IEnumerator Login(WWWForm form)
        {
            WWW response = new WWW(loginBaseURL, form);
            yield return response;

            if (response.error == null)
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
            if (Int32.TryParse(input, out errorCode))
            {
                Debug.Log(Enum.GetName(typeof(ResponseEnums.LoginResponse), errorCode));
                message.text = Enum.GetName(typeof(ResponseEnums.LoginResponse), errorCode);
            }
            else
            {
                User user = JsonValidator.ValidateJsonData<User>(input);
                if (user != default(User))
                {
                    Debug.Log(user);
                    message.text = "Logged In...";
                }
                else
                {
                    Debug.LogError("Failed to deserialize as User: " + input);
                }
            }
        }
    }
}
