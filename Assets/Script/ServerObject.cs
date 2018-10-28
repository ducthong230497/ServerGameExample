using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

[Serializable]
public class ServerObject : ISerializable
{
    public int err_code { get; set; }
    public string err_msg { get; set; }

    //private Dictionary<string, int> intDict;
    //private Dictionary<string, long> longDict;
    //private Dictionary<string, string> stringDict;
    //private Dictionary<string, bool> boolDict;
    public Dictionary<string, object> dictionary;
    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        info.AddValue("err_code", err_code);
        info.AddValue("err_msg", err_msg);
        //info.AddValue("intDict", intDict, typeof(Dictionary<string, int>));
        //info.AddValue("longDict", longDict, typeof(Dictionary<string, long>));
        //info.AddValue("stringDict", stringDict, typeof(Dictionary<string, string>));
        //info.AddValue("boolDict", boolDict, typeof(Dictionary<string, bool>));
        info.AddValue("dictionary", dictionary, typeof(Dictionary<string, object>));
    }

    public ServerObject()
    {
        //intDict = new Dictionary<string, int>();
        //longDict = new Dictionary<string, long>();
        //stringDict = new Dictionary<string, string>();
        //boolDict = new Dictionary<string, bool>();
        dictionary = new Dictionary<string, object>();
    }

    public ServerObject(SerializationInfo info, StreamingContext context)
    {
        err_code = info.GetInt32("err_code");
        err_msg = info.GetString("err_msg");
        dictionary = (Dictionary<string, object>)info.GetValue("dictionary", typeof(Dictionary<string, object>));
    }

    public void PutInt(string key, int value)
    {
        dictionary.Add(key, value);
    }

    public int GetInt(string key)
    {
        return (int)dictionary[key];
    }

    public void PutString(string key, string value)
    {
        dictionary.Add(key, value);
    }

    public string GetString(string key)
    {
        return (string)dictionary[key];
    }

    public void PutList<T>(string key, List<T> values)
    {
        dictionary.Add(key, values);
    }

    public List<T> GetList<T>(string key)
    {
        return  (List<T>)dictionary[key];
    }
}
