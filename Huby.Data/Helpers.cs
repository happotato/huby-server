using System;

namespace Huby.Data
{
    public static class Helpers
    {
        public static T FindMax<T>(params T[] items) where T: IComparable {
            T result = default;

            foreach (var item in items)
            {
                if (item.CompareTo(result) > 0)
                    result = item;
            }

            return result;
        }
    }
}
