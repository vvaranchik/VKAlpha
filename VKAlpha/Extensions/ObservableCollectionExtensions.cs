using System;
using System.Collections.ObjectModel;

namespace VKAlpha.Extensions
{
    public static class ObservableCollectionExtensions
    {
        private static int seed = Environment.TickCount;
        private static Random random = new Random(seed);

        public static void Shuffle<T>(this ObservableCollection<T> data)
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
    }
}
