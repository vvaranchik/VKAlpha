namespace Alpha.SharedLibrary.Realisation
{
    using Alpha.SharedLibrary.Interfaces;

    public class Singleton<T> where T : IBase
    {
        static bool isDestroyed = false;
        static volatile T? instance;
        static object locker = new object();

        // Possible deadlocks, need to use carefully
        public static bool IsOneThreadAccess { get; set; } = false;

        /// <summary>
        /// Is instance destroyed
        /// </summary>
        /// <returns></returns>
        public static bool IsAvailable() => !isDestroyed;
        
        /// <summary>
        /// Gets or creates and gets instance of class
        /// </summary>
        public static T Instance
        {
            get
            {
                if (!instance && !isDestroyed)
                {
                    lock (locker)
                    {
                        if (!instance && !isDestroyed)
                        {
                            instance = Activator.CreateInstance<T>();
                        }

                    }
                }
                if (IsOneThreadAccess)
                    lock (locker)
                        return instance;
                return instance;
            }
        }

        public static void Destroy()
        {
            if (isDestroyed) return;
            lock (locker)
            {
                isDestroyed = true;
                instance = null;
            }
        }
    }
}
