using System;
using System.Collections.Generic;

namespace Utils
{
    public static class CollectionUtils
    {
        public static int IndexOf<TSource>(this IEnumerable<TSource> list, TSource value) where TSource : IEquatable<TSource>
        {
            return list.IndexOf<TSource>(value, EqualityComparer<TSource>.Default);
        }
        
        public static int IndexOf<TSource>(this IEnumerable<TSource> list, TSource value, IEqualityComparer<TSource> comparer)
        {
#if (UNITY_IPHONE || UNITY_IOS)
			var listArray = list.ToArray();
			for (var i = 0; i < listArray.Length; i++)
			{
				if (comparer.Equals(listArray[i], value))
				{
					return i;
				}
			}
#else
            int index = 0;
            foreach (TSource item in list)
            {
                if (comparer.Equals(item, value))
                {
                    return index;
                }

                index++;
            }
#endif
            return -1;
        }
    }
}