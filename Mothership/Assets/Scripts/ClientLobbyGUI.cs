using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Linq;
using Pathfinding.Serialization.JsonFx;

public class ClientLobbyGUI : MonoBehaviour
{
    [SerializeField]
    private InputField username = null;
    [SerializeField]
    private InputField password = null;
    [SerializeField]
    private Button loginButton = null;

    private string key = "key";
    private string loginURL = "http://studentnet.kingston.ac.uk/~k1159960/login.php";

    void Start()
    {
        string login = loginURL + "?uname=default&pass=password&hash=key";
        StartCoroutine(Login(login));
    }

    private IEnumerator Login(string url)
    {
        Debug.Log("Attempting login");
        WWW login_get = new WWW(url);
        yield return login_get;

        Debug.Log("accessed remote service");
        if(login_get.error == null)
        {
            Debug.Log("Data: " + login_get.text);
            if (login_get.text != "LoginFailed")
            {
                Parse(login_get.text);
            }
        }
        else
        {
            Debug.Log(login_get.error);
        }
    }

    private void Parse(string data)
    {
        User du = ValidateJsonData<User>(data);
        Debug.Log(du.email);
    }

    private T ValidateJsonData<T>(string input)
    {
        T output;
        try
        {
            output = JsonReader.Deserialize<T>(input);
        }
        catch (System.Exception ex)
        {
            Debug.Log(ex.Message);
            output = default(T);
        }

        return output;
    }

    private class User
    {
        public string UserID;
        public string Username {get; set;}
        public string Password;
        public string email;
    }
}
