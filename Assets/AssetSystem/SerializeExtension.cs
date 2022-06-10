using UnityEngine;

namespace AssetSystem
{
    public static class SerializeExtension
    {
        public static Vector3 ToVector3(this string str)
        {
            var strings = str.Split(',');

            if (strings.Length != 3 ||
                !float.TryParse(strings[0], out var x) ||
                !float.TryParse(strings[1], out var y) ||
                !float.TryParse(strings[2], out var z))
                return Vector3.zero;

            return new Vector3(x, y, z);
        }
        
        public static Vector4 ToVector4(this string str)
        {
            var strings = str.Split(',');

            if (strings.Length != 4 ||
                !float.TryParse(strings[0], out var x) ||
                !float.TryParse(strings[1], out var y) ||
                !float.TryParse(strings[2], out var z) ||
                !float.TryParse(strings[3], out var w))
                return Vector4.zero;

            return new Vector4(x, y, z,w);
        }
        
        public static Quaternion ToQuaternion(this string str)
        {
            var strings = str.Split(',');

            if (strings.Length != 4 ||
                !float.TryParse(strings[0], out var x) ||
                !float.TryParse(strings[1], out var y) ||
                !float.TryParse(strings[2], out var z) ||
                !float.TryParse(strings[3], out var w))
                return Quaternion.identity;

            return new Quaternion(x, y, z, w);
        }

        public static Color ToColor(this string str)
        {
            var strings = str.Split(',');

            if (strings.Length != 4 ||
                !float.TryParse(strings[0], out var r) ||
                !float.TryParse(strings[1], out var g) ||
                !float.TryParse(strings[2], out var b) ||
                !float.TryParse(strings[3], out var a))
                return Color.clear;

            return new Color(r,g,b,a);
        }

        public static string Serialize(this Vector3 vec)
        {
            return $"{vec.x},{vec.y},{vec.z}";
        }


        public static string Serialize(this Vector4 vec)
        {
            return $"{vec.x},{vec.y},{vec.z},{vec.w}";
        }

        public static string Serialize(this Quaternion q)
        {
            return $"{q.x},{q.y},{q.z},{q.w}";
        }

        public static string Serialize(this Color c)
        {
            return $"{c.r},{c.g},{c.b},{c.a}";
        }
    }
}