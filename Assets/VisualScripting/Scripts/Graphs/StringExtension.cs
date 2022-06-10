using System;

namespace VisualScripting.Scripts.Graphs
{
    public static class StringExtension
    {
        public static String Random(this String str,int size)
        {
            var result = "";

            var rand = new System.Random();
            var len = str.Length;
            
            for (int i = 0; i < size; i++)
            {
                result += str[rand.Next(0, len)];
            }

            return result;
        }
    }
}