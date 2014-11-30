using UnityEngine;
using System.Collections;

namespace MothershipOS
{
    public static class ResponseEnums
    {
        public enum AccountCreationResponse 
        { 
            NoError, 
            Error_InvalidHash,
            Error_EmailEmpty, 
            Error_EmailInUse, 
            Error_PasswordEmpty,
            Error_NameInUse
        };

        public enum LoginResponse
        {
            Norror,
            Error_InvalidHash,
            Error_IncorrectCredentials,
            Error_NoProfile
        }
    }
}