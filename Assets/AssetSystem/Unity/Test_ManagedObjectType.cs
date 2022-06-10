using System;
using System.Collections.Generic;
using UnityEngine;

namespace AssetSystem.Unity
{
    [Serializable]
    public class Test_ManagedObjectType
    {
        public string id;
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;
        public List<string> childrenIds;
        public List<Test_KVString> strGlobal;
        public List<Test_KVInt> intGlobal;
        public List<Test_KVFloat> floatGlobal;
        public List<Test_KVBool> boolGlobal;
    }
    
    [Serializable]
    public class Test_ResourceType
    {
        public string id;
        public string type;
        public List<Test_KVString> data;
    }

    [Serializable]
    public class Test_KVString
    {
        public string id;
        public string value;
    }

    [Serializable]
    public class Test_KVInt
    {
        public string id;
        public int value;
    }

    [Serializable]
    public class Test_KVFloat
    {
        public string id;
        public float value;
    }

    [Serializable]
    public class Test_KVBool
    {
        public string id;
        public bool value;
    }
}