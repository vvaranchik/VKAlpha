using System;

namespace VKAlpha.Helpers
{
    public abstract class BaseSingleton<T>
    {
        static bool is_destroyed = false;

        static protected T instance;
        static private object instance_locker = new object();

        public static T Get
        {
            get
            {
                if (instance == null && !is_destroyed)
                {
                    lock (instance_locker)
                    {
                        if (instance == null && !is_destroyed)
                        {
                            instance = Activator.CreateInstance<T>();
                        }
                    }
                }
                return instance;
            }
        }

        public static bool IsDestroyed() => is_destroyed;

        public static void Destroy()
        {
            if (is_destroyed) return;
            lock (instance_locker)
            {
                is_destroyed = true;
                instance = default;
            }
        }
    }
}
