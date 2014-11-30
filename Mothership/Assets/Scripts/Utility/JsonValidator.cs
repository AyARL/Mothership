using UnityEngine;
using System.Collections;
using Pathfinding.Serialization.JsonFx;

namespace MothershipUtility
{
    public static class JsonValidator
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
            return JsonWriter.Serialize(input);
        }
    }
}
