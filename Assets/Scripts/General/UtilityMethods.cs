using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnknownWorld.Utility.Methods
{
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

    class AreaEqualityComparer : IEqualityComparer<UnknownWorld.Area.Target.TracingArea>
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
