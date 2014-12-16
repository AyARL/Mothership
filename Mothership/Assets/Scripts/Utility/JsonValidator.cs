using UnityEngine;
using System.Collections;
using Pathfinding.Serialization.JsonFx;
using System;

namespace MothershipUtility
{
    public static class JsonUtility
    {
        public static T ValidateJsonData<T>(string input)
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

        public static string SerializeToJson<T>(T input)
        {
            Type type = typeof(T);
            if(type.IsInterface)
            {
                Debug.LogError(string.Format("{0} is an interface type, which is not valid for serialisation.", typeof(T).ToString()));
                return "";
            }
            else if(typeof(T).IsAbstract)
            {
                Debug.LogWarning(string.Format("{0} is an abstract type. You should use the actual object type to make sure no data is lost.", typeof(T).ToString()));
            }
            return JsonWriter.Serialize(input);
        }
    }
}
