using System;
using System.Linq;
using System.Collections.Generic;

    public static class ListExtension
    {
        public static int Sum(this IList<int> list)
        {
            int s = 0;
            for (int i = 0; i < list.Count; i++)
                s += list[i];
            return s;
        }

        public static float Sum(this IList<float> list)
        {
            float s = 0;
            for (int i = 0; i < list.Count; i++)
                s += list[i];
            return s;
        }

        public static T SelectWithProbably<T>(this IList<T> list,IList<float> prob)
        {
            float rand = UnityEngine.Random.Range(0f, prob.Sum());
            float s = 0;

            for (int i = 0; i < list.Count; i++)
            {
                var p = prob[i];
                if(s <= rand && rand <= s + p)
                {
                    return list[i];
                }
                s += p;
            }

            return list[list.Count-1];
        }

        public static T Random<T>(this IList<T> list)
        {
            if (list.Count == 0)
                return default(T);

            var count = UnityEngine.Random.Range(0, list.Count);
            return list[count];
        }
        
        public static IEnumerable<TResult> SelectMany<TResult>(this IEnumerable<IEnumerable<TResult>> source)
        {
            return source.SelectMany(s => s);
        }

        public static Dictionary<TKey, TElement> ToDictionary<TKey, TElement>(
            this IEnumerable<KeyValuePair<TKey,TElement>> source)
        {
            return source.ToDictionary(kv=>kv.Key,kv => kv.Value);
        }

        public static TResult GetOrNull<TResult>(this IList<TResult> source,int index,TResult defaultValue = default)
        {
            if (source.Count > index && source.Count > -1)
            {
                return source[index];
            }
            return defaultValue;
        }
        
        // UNIXエポックを表すDateTimeオブジェクトを取得
        private static readonly DateTime UnixEpoch =
            new(1970, 1, 1, 0, 0, 0, 0);

        public static long ToUnixTime(this DateTime time)
        {
            // UTC時間に変換
            time = time.ToUniversalTime();

            // UNIXエポックからの経過時間を取得
            TimeSpan elapsedTime = time - UnixEpoch;
   
            // 経過秒数に変換
            return (long)elapsedTime.TotalSeconds;
        }
    }