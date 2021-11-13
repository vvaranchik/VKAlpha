using System;

namespace VKAlpha.Helpers
{
    public abstract class CClass
    {
        public static bool operator! (CClass @this) => @this == null;
        public static bool operator false(CClass @this) => !@this || @this == default;
        public static bool operator true(CClass @this) => !!@this || @this != default;
    }

    public abstract class BaseSingleton<T> : CClass, IDisposable where T : CClass
    {
        static bool is_destroyed = false;

        static protected volatile T instance;
        static protected object instance_locker = new object();

        public static T Get
        {
            get
            {
                if (!instance && !is_destroyed)
                {
                    lock (instance_locker)
                    {
                        if (!instance && !is_destroyed)
                        {
                            instance = Activator.CreateInstance<T>();
                        }
                    }
                }
                return instance;
            }
        }

        static object one_way_locker = new object();

        public static T GetOneWay
        {
            get
            {
                lock (one_way_locker)
                {
                    return Get;
                }
            }
        }

        public static bool IsDestroyed() => is_destroyed;

        public static void Destroy()
        {
            if (is_destroyed) return;
            lock (instance_locker)
            {
                is_destroyed = true;
                instance = null;
            }
        }

        public void Dispose()
        {
            Destroy();
        }
    }
}
