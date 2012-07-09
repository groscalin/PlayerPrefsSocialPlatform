/**
* @file PlayerPrefsSerializer.cs
* @brief Code snippet from UnityForum (http://forum.unity3d.com/threads/72156-C-Serialization-PlayerPrefs-mystery)
* @author mindlube+FizixMan  
* @version 1.0
* @date 2012-06-15
*/

using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using CloningExtension;
 
public class PlayerPrefsSerializer  
{
    public static BinaryFormatter bf = new BinaryFormatter ();

    public static bool handleNonSerializable
    {
        set 
        {
            PlayerPrefsSerializer.bf.SurrogateSelector = new SurrogateSelector();
            PlayerPrefsSerializer.bf.SurrogateSelector.ChainSelector( new NonSerialiazableTypeSurrogateSelector() );
        }
    }

    // serializableObject is any struct or class marked with [Serializable]
    public static void Save (string prefKey, object serializableObject)
    {
        MemoryStream memoryStream = new MemoryStream ();
        bf.Serialize (memoryStream, serializableObject);
        string tmp = System.Convert.ToBase64String (memoryStream.ToArray ());
        PlayerPrefs.SetString ( prefKey, tmp);
    }
    
    public static T Load<T>(string prefKey)
    {
        if (!PlayerPrefs.HasKey(prefKey))
            return default(T);
        
        string serializedData = PlayerPrefs.GetString(prefKey);
        MemoryStream dataStream = new MemoryStream(System.Convert.FromBase64String(serializedData));
        T deserializedObject = (T)bf.Deserialize(dataStream);
        return deserializedObject;
    }

    public static string Encode(object serializableObject)
    {
        MemoryStream memoryStream = new MemoryStream ();
        bf.Serialize (memoryStream, serializableObject);
        string tmp = System.Convert.ToBase64String (memoryStream.ToArray ());
        return tmp;
    }

    public static T Decode<T>(string serialized)
    {
        MemoryStream dataStream = new MemoryStream(System.Convert.FromBase64String(serialized));
        T deserializedObject = (T)bf.Deserialize(dataStream);
        return deserializedObject;
    }
}
