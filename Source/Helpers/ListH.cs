using System.Collections.Generic;
using System.Linq;

namespace Helpers {
	public static class ListH {
		public static void Resize<T>(this List<T> list, int size, T element = default) {
			int count = list.Count;

			if (size < count) {
				list.RemoveRange(size, count - size);
			} else if (size > count) {
				if (size > list.Capacity) // Optimization
					list.Capacity = size;

				list.AddRange(Enumerable.Repeat(element, size - count));
			}
		}

		public static T GetRandomElement<T>(this List<T> list) {
			return list[RandomH.GetRandom(0, list.Count - 1)];
		}
	}
}