using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnknownWorld.Utility.Methods
{
    class List
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
}
