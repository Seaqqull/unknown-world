using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

namespace UnknownWorld.Utility.Methods
{
    public static class Hasher
    {
        private static readonly System.DateTime Jan1st1970 = new System.DateTime
            (1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);


        public static string GenerateHash()
        {
            MD5 md5Hasher = MD5.Create();
            byte[] data = md5Hasher.ComputeHash(
                System.Text.Encoding.Default.GetBytes(GenerateString())
            );

            return System.BitConverter.ToString(data);
        }

        public static string GenerateString()
        {
            return System.Guid.NewGuid().ToString() + CurrentTimeMillis();
        }

        public static long CurrentTimeMillis()
        {
            return (long)(System.DateTime.UtcNow - Jan1st1970).TotalMilliseconds;
        }

        public static string GenerateHash(string input)
        {
            MD5 md5Hasher = MD5.Create();
            byte[] data = md5Hasher.ComputeHash(
                System.Text.Encoding.Default.GetBytes(input)
            );

            return System.BitConverter.ToString(data);
        }

    }

    public static class VectorOperations
    {
        public static float Map(float value, float istart, float istop, float ostart, float ostop)
        {
            return ostart + (ostop - ostart) * ((value - istart) / (istop - istart));
        }
    }

    public static class List
    {
        public static void ClearExceptOne<T>(List<T> list, ref int index)
        {
            if (index > 0)
                list.RemoveRange(0, index);
            index = 0;
            if (list.Count > 1)
                list.RemoveRange(index + 1, list.Count - 1);
        }

        public static List<T> GetAllExceptOne<T>(List<T> list, int index)
        {
            List<T> temp = new List<T>(list);
            temp.RemoveAt(index);
            return temp;
        }
    }


    public class AreaEqualityComparer : IEqualityComparer<UnknownWorld.Area.Target.TracingArea>
    {
        public int GetHashCode(UnknownWorld.Area.Target.TracingArea obj)
        {
            return obj.Id.GetHashCode();
        }

        public bool Equals(UnknownWorld.Area.Target.TracingArea x, UnknownWorld.Area.Target.TracingArea y)
        {
            // Two items are equal if their keys are equal.
            return x.Id == y.Id;
        }
    }

}
