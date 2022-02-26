using System;
using System.Collections.ObjectModel;

namespace VKAlpha.Extensions
{
    public static class ObservableCollectionExtensions
    {
        private readonly static int seed = Environment.TickCount;
        private readonly static Random random = new Random(seed);

        public static void Shuffle<T>(this Collection<T> data)
        {       
            int num = data.Count;
            while (num > 1)
            {
                int index = random.Next(num--);
                T value = data[num];
                data[num] = data[index];
                data[index] = value;
            }
        }

        /// <summary>
        /// Gets random element from collection
        /// </summary>
        /// <returns>Copy of random element from collection</returns>
        public static T GetRandomElement<T>(this Collection<T> @this)
        {
            var elem = @this[random.Next(@this.Count)];
            return elem;
        }
    }
}
