using UnityEngine;
using System.Collections;
using MothershipUtility;
using MothershipOS;

namespace MothershipOS
{
    public static class WWWFormUtility 
    {
        public static string registerURL = "http://studentnet.kingston.ac.uk/~k1159960/register.php";
        public static string loginURL = "http://studentnet.kingston.ac.uk/~k1159960/login.php";
        public static string getProfileURL = "http://studentnet.kingston.ac.uk/~k1159960/get_profile.php";
        public static string getLastGameURL = "http://studentnet.kingston.ac.uk/~k1159960/get_last_game.php";
        public static string recordGameScoresURL = "http://studentnet.kingston.ac.uk/~k1159960/insert_game.php";

        public static WWWForm GetLoginForm(string email, string password)
        {
            WWWForm form = new WWWForm();
            form.AddField("Email", email);
            form.AddField("Password", HashUtility.GetMD5Hash(password));
            form.AddField("Hash", HashUtility.GetMD5Hash(email + AppKey.appKey));

            return form;
        }

        public static WWWForm GetLoginForm(User user)
        {
            WWWForm form = new WWWForm();
            form.AddField("Email", user.Email);
            form.AddField("Password", user.Password);
            form.AddField("Hash", HashUtility.GetMD5Hash(user.Email + AppKey.appKey));

            return form;
        }
    }
}
