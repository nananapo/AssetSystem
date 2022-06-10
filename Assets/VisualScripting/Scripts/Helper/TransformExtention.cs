using UnityEngine;

namespace VisualScripting.Scripts.Helper
{
    public static class TransformExtention
    {
        public static Transform[] GetChildren(this Transform transform)
        {
            var children = new Transform[transform.childCount];
            for (int i = 0; i < transform.childCount; i++)
            {
                children[i] = transform.GetChild(i);
            }

            return children;
        }

        public static float GetGlobalPositionX(this Transform transform)
        {
            return transform.position.x;
        }

        public static float GetGlobalPositionY(this Transform transform)
        {
            return transform.position.y;
        }

        public static float GetGlobalPositionZ(this Transform transform)
        {
            return transform.position.z;
        }

        public static float GetLocalPositionX(this Transform transform)
        {
            return transform.localPosition.x;
        }

        public static float GetLocalPositionY(this Transform transform)
        {
            return transform.localPosition.y;
        }

        public static float GetLocalPositionZ(this Transform transform)
        {
            return transform.localPosition.z;
        }

        public static void SetGlobalPositionX(this Transform transform, float value)
        {
            var pos = transform.position;
            pos.x = value;
            transform.position = pos;
        }

        public static void SetGlobalPositionY(this Transform transform, float value)
        {
            var pos = transform.position;
            pos.y = value;
            transform.position = pos;
        }

        public static void SetGlobalPositionZ(this Transform transform, float value)
        {
            var pos = transform.position;
            pos.z = value;
            transform.position = pos;
        }

        public static void SetLocalPositionX(this Transform transform, float value)
        {
            var pos = transform.localPosition;
            pos.x = value;
            transform.position = pos;
        }

        public static void SetLocalPositionY(this Transform transform, float value)
        {
            var pos = transform.localPosition;
            pos.y = value;
            transform.position = pos;
        }

        public static void SetLocalPositionZ(this Transform transform, float value)
        {
            var pos = transform.localPosition;
            pos.z = value;
            transform.position = pos;
        }

        public static float GetLocalScaleX(this Transform transform)
        {
            return transform.localScale.x;
        }

        public static float GetLocalScaleY(this Transform transform)
        {
            return transform.localScale.y;
        }

        public static float GetLocalScaleZ(this Transform transform)
        {
            return transform.localScale.z;
        }
        
        public static void SetLocalScaleX(this Transform transform, float value)
        {
            var pos = transform.localScale;
            pos.x = value;
            transform.localScale = pos;
        }

        public static void SetLocalScaleY(this Transform transform, float value)
        {
            var pos = transform.localScale;
            pos.y = value;
            transform.localScale = pos;
        }

        public static void SetLocalScaleZ(this Transform transform, float value)
        {
            var pos = transform.localScale;
            pos.z = value;
            transform.localScale = pos;
        }
    }
}