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
        }

        public enum LoginResponse
        {
            NoError,
            Error_InvalidHash,
            Error_IncorrectCredentials
        }

        public enum GetProfileResponse
        {
            NoError,
            Error_InvalidHash,
            Error_IncorrectCredentials,
            Error_NoProfile
        }

        public enum GetLastGameResponse
        {
            NoError,
            Error_InvalidHash,
            Error_IncorrectCredentials,
            Error_NoData
        }
    }
}