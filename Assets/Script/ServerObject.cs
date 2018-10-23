using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

[Serializable]
public class ServerObject : ISerializable
{
    public int test { get; set; }
    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        info.AddValue("test", test);
    }

    public ServerObject()
    {

    }

    public ServerObject(SerializationInfo info, StreamingContext context)
    {
        test = info.GetInt32("test");
    }


}
