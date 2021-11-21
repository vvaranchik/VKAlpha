namespace Alpha.SharedLibrary.Interfaces
{
    public abstract class IBase
    {
        public static bool operator !(IBase @this) => @this == null;
        public static bool operator true(IBase @this) => !@this != true;
        public static bool operator false(IBase @this) => !@this == true;
    }
}
